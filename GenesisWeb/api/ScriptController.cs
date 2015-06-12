using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.SharePoint.Client;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.SharePoint.Client.Publishing;

namespace GenesisWeb.api
{
    public class ScriptController : ApiController
    {
      
        const string HEADER = "Genesis_HeaderScript";
        const string LOAD_SCRIPT = "Genesis_LoadScript";
        const string LOAD_CSS = "Genesis_LoadCSS";
        const string JS = "JS";
        const string CSS = "CSS";
        const string GENESIS_JS = "GENESIS_JS";
        const string GENESIS_CSS = "GENESIS_CSS";

        [HttpPost]
        public dynamic GetConfigurations([FromBody] dynamic config)
        {
            try
            {   
                Configurations configurations = new Configurations();
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {                    
                    UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
                    configurations = GetConfigurations(clientContext,  out customActions);
                }
              
                return Request.CreateResponse(HttpStatusCode.OK, configurations);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }

        //Depricated
        [HttpPost]        
        public dynamic ValidateUniqueScriptName([FromBody] dynamic config)
        {
            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    string title = config["Title"] != null ? config["Title"].ToString() : null;
                    var customActions = clientContext.Site.UserCustomActions;
                    clientContext.Load(customActions);
                    clientContext.ExecuteQuery();

                    var scriptAction = customActions.FirstOrDefault(x => x.Title.Equals(title));

                    if (scriptAction == null)
                        return true;
                    else
                        return false;
                }               

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }   


        [HttpPost]
        public dynamic SaveScript([FromBody] dynamic config)
        {
            try
            {
                ScriptLinkAction action;
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    ScriptLinkAction scriptLinkAction = JsonConvert.DeserializeObject<ScriptLinkAction>(config["ScriptLink"].ToString());
                    action = SaveCustomActionScriptLink(clientContext, scriptLinkAction);
                }

                return Request.CreateResponse(HttpStatusCode.OK, action);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }


        [HttpPost]
        public dynamic DeleteScript([FromBody] dynamic config)
        {
            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    ScriptLinkAction scriptLinkAction = JsonConvert.DeserializeObject<ScriptLinkAction>(config["ScriptLink"].ToString());
                    DeleteCustomActionScriptLink(clientContext, scriptLinkAction);
                }

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }       
        

        [HttpPost]
        public dynamic SaveConfigurations([FromBody] dynamic config)
        {
            try
            {              
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    SaveCustomActionScriptBlock(clientContext, config["configurations"].ToString());                  
                }

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }

       
        public Configurations GetConfigurations(ClientContext clientContext, out UserCustomActionCollection customActions)
        {
            Configurations configurations = new Configurations();

            customActions = clientContext.Site.UserCustomActions;
            clientContext.Load(customActions);
            clientContext.ExecuteQuery();


            configurations.ScriptLinks = new List<ScriptLinkAction>();
            configurations.CSSLinks = new List<ScriptLinkAction>();

            foreach (UserCustomAction customAction in customActions)
            {
                if (customAction.Title == HEADER)
                {
                    configurations.HeaderScripts = customAction.ScriptBlock;
                }
                else if (customAction.Title != LOAD_SCRIPT)
                {
                    if (customAction.Title.StartsWith(GENESIS_CSS) &&
                         !String.IsNullOrEmpty(customAction.ScriptBlock))
                    {      
                        string scriptSrc = customAction.ScriptBlock.Split(new string[] { "LoadCSS('" }, StringSplitOptions.None)[1];
                        string exclude = "";

                        if (scriptSrc.Contains(","))
                        {
                            //means we have an Exclude
                            var scriptScrSplit = scriptSrc.Split(new string[] { "'," }, StringSplitOptions.None);
                            scriptSrc = scriptScrSplit[0];
                            exclude = scriptScrSplit[1].Split(new string[] { "');" }, StringSplitOptions.None)[0].Replace("'", "");
                        }
                        else
                        {
                            scriptSrc = scriptSrc.Split(new string[] { "');" }, StringSplitOptions.None)[0];
                        }

                        configurations.CSSLinks.Add(new ScriptLinkAction
                        {
                            Id = customAction.Id.ToString(),
                            Title = customAction.Title.Replace(GENESIS_CSS, ""),
                            ScriptSrc = scriptSrc,
                            Excludes = exclude,
                            Sequence = customAction.Sequence.ToString(),
                            Type = CSS
                        });  
                    }
                    else if (customAction.Title.StartsWith(GENESIS_JS) &&
                            !String.IsNullOrEmpty(customAction.ScriptBlock))
                    {
                        string scriptSrc = customAction.ScriptBlock.Split(new string[] { "LoadScript('" }, StringSplitOptions.None)[1];
                        string exclude = "";
                        string loadAction = "";

                        if (scriptSrc.Contains(","))
                        {
                            //means we have an Exclude and/or a LoadAction
                            var scriptScrSplit = scriptSrc.Split(new string[] { "'," }, StringSplitOptions.None);
                            scriptSrc = scriptScrSplit[0]; 
                          
                            if (scriptScrSplit.Length == 2) {
                                exclude = scriptScrSplit[1].Split(new string[] { "');" }, StringSplitOptions.None)[0].Replace("'", "");
                            }
                            else if (scriptScrSplit.Length == 3)
                            {
                                exclude = scriptScrSplit[1].Replace("'", "");
                                loadAction = scriptScrSplit[2].Split(new string[] { "');" }, StringSplitOptions.None)[0].Replace("'", "");
                            }
                        }
                        else
                        {
                            scriptSrc = scriptSrc.Split(new string[] { "');" }, StringSplitOptions.None)[0];
                        }

                        configurations.ScriptLinks.Add(new ScriptLinkAction
                        {
                            Id = customAction.Id.ToString(),
                            Title = customAction.Title.Replace(GENESIS_JS, ""),
                            ScriptSrc = scriptSrc,
                            Excludes = exclude,
                            CustomAction = loadAction,
                            Sequence = customAction.Sequence.ToString(),
                            Type = JS
                        });  
                    }
                }
            }

            return configurations;
        }

     
        private void RegisterHelpers(ClientContext clientContext)
        {
            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
            clientContext.Load(customActions);
            clientContext.ExecuteQuery();

            //Delete the old one in case we made any updates
            var loadScriptActions = customActions.Where(x => x.Title.Equals(LOAD_SCRIPT)).ToList(); 
            if (loadScriptActions != null) {
                foreach(var loadScriptAction in loadScriptActions)
                    loadScriptAction.DeleteObject();

                clientContext.ExecuteQuery();
            }

            //Register our help Load Script so people can load external scripts          
            UserCustomAction customLoadScriptAction = customActions.Add();
            customLoadScriptAction.Location = "ScriptLink";
            customLoadScriptAction.ScriptBlock = LoadScript();
            customLoadScriptAction.Title = LOAD_SCRIPT;
            customLoadScriptAction.Update();

            //Delete the old one in case we made any updates
            var loadCSSActions = customActions.Where(x => x.Title.Equals(LOAD_CSS)).ToList();
            if (loadCSSActions != null)
            {
                foreach (var loadCSSAction in loadCSSActions)
                    loadCSSAction.DeleteObject();

                clientContext.ExecuteQuery();
            }

            //Register our help Load CSS so people can load external scripts         
            UserCustomAction customLoadCSSAction = customActions.Add();
            customLoadCSSAction.Location = "ScriptLink";
            customLoadCSSAction.ScriptBlock = LoadCSS();
            customLoadCSSAction.Title = LOAD_CSS;
            customLoadCSSAction.Update();        

            clientContext.ExecuteQuery();
        }

        private void SaveCustomActionScriptBlock(ClientContext clientContext, string configurations)
        {           
            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
            clientContext.Load(customActions);
            clientContext.ExecuteQuery();

            RegisterHelpers(clientContext);

            var headerScriptAction = customActions.FirstOrDefault(x => x.Title.Equals(HEADER));  

            //Register the users custom scripts
            UserCustomAction customAction = customActions.Add();
            bool isNew = true;
            if (headerScriptAction != null)
            {
                isNew = false;
                customAction = headerScriptAction;
            }

            Configurations configurationsToSave = JsonConvert.DeserializeObject<Configurations>(configurations);
            string script = configurationsToSave.HeaderScripts;

            if (!string.IsNullOrEmpty(script))
            {
                customAction.Location = "ScriptLink";
                customAction.ScriptBlock = script;
                customAction.Title = HEADER;
                customAction.Update();
            }
            else if (!isNew)
            {
                customAction.DeleteObject();
            }


            clientContext.Load(customAction);
            clientContext.ExecuteQuery();

            //Debugging helper...
            //CleanupScriptBlock(clientContext, headerScriptAction, loadScriptAction, loadCSSAction);           
        }
               

        private ScriptLinkAction SaveCustomActionScriptLink(ClientContext clientContext, ScriptLinkAction scriptLinkAction)
        {
            RegisterHelpers(clientContext);

            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;

            UserCustomAction customAction = null;
            Guid id;

            if (!scriptLinkAction.IsNew)
            {
                //update
                id = new Guid(scriptLinkAction.Id);
                clientContext.Load(customActions);
                clientContext.ExecuteQuery();
                customAction = customActions.FirstOrDefault(x => x.Id.Equals(id));
            }
            else
            {
                //insert
                id = Guid.NewGuid();
                customAction = customActions.Add();
            }          

            customAction.Location = "ScriptLink";
            customAction.Title = scriptLinkAction.Title;          

            if(scriptLinkAction.Type == JS) {
                var excludes = !string.IsNullOrEmpty(scriptLinkAction.Excludes) ? scriptLinkAction.Excludes : "";
                var loadAction = !string.IsNullOrEmpty(scriptLinkAction.CustomAction) ? scriptLinkAction.CustomAction : "";

                customAction.ScriptBlock = "LoadScript('" + scriptLinkAction.ScriptSrc + "','" + excludes + "','" + loadAction + "');";                            
                customAction.Title = GENESIS_JS + customAction.Title;
            }else {
                var excludes = !string.IsNullOrEmpty(scriptLinkAction.Excludes) ? scriptLinkAction.Excludes : "";

                customAction.ScriptBlock = "LoadCSS('" + scriptLinkAction.ScriptSrc + "','" + excludes + "');";
                customAction.Title = GENESIS_CSS + customAction.Title;        
            }           

            int sequenceNum = 0;
            int.TryParse(scriptLinkAction.Sequence, out sequenceNum);
            customAction.Sequence = sequenceNum;

            customAction.Update();  

            clientContext.Load(customAction);
            clientContext.ExecuteQuery();

            return new ScriptLinkAction { 
                Title = customAction.Title, 
                ScriptSrc = customAction.ScriptSrc,
                Sequence = customAction.Sequence.ToString(),
                Id = customAction.Id.ToString()
            };

        }

        private void DeleteCustomActionScriptLink(ClientContext clientContext, ScriptLinkAction scriptLink)
        {

            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
            clientContext.Load(customActions);
            clientContext.ExecuteQuery();

            var headerScriptAction = customActions.FirstOrDefault(x => x.Id.Equals(new Guid(scriptLink.Id)));

            if (headerScriptAction != null)
            {
                headerScriptAction.DeleteObject();
            }
          

            clientContext.Load(headerScriptAction);
            clientContext.ExecuteQuery();
        }


        private string LoadScript() {
            return @"function LoadScript(scriptUrl, excludes, nameOfEvent) { 
                        if((excludes && excludes != '' && window.location.href.indexOf(excludes) === -1) || (!excludes || excludes==='')) {
                            
                            document.write('<script type=""text/javascript"" src=""' + scriptUrl + '""></' + 'script>');

                           
                        }
                    }";

        }

        private string LoadCSS() {
            return @"function LoadCSS(cssUrl, excludes) {
                        if((excludes && excludes != '' && window.location.href.indexOf(excludes) === -1) || (!excludes || excludes==='')) {
                            var genesisLoadCSS= document.createElement('link'); 
                            genesisLoadCSS.rel = 'stylesheet';
                            genesisLoadCSS.type = 'text/css';
                            genesisLoadCSS.href = cssUrl;
                            document.getElementsByTagName('head')[0].appendChild(genesisLoadCSS);
                        }
                    }";
        }
      
    }


    public class Configurations
    {
        public string HeaderScripts { get; set; }
        public List<ScriptLinkAction> ScriptLinks { get; set; }
        public List<ScriptLinkAction> CSSLinks { get; set; }
    }

    public class ScriptLinkAction {
        public string Title { get; set; }
        public string ScriptSrc { get; set; }
        public string Sequence { get; set; }
        public string Excludes { get; set; }
        public string CustomAction { get; set; }
        public bool IsEditing { get; set; }
        public bool IsNew { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
