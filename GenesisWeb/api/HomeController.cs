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
    public class HomeController : ApiController
    {
        const string HEADER = "Genesis_HeaderScript";
        const string LOAD_SCRIPT = "Genesis_LoadScript";
        const string LOAD_CSS = "Genesis_LoadCSS";

        [HttpPost]
        public dynamic GetConfigurations([FromBody] dynamic config)
        {
            try
            {   
                Configurations configurations = new Configurations();
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
                    configurations = GetConfigurations(clientContext, out customActions);

                }
              
                return Request.CreateResponse(HttpStatusCode.OK, configurations);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }

        [HttpPost]
        public dynamic Addscript([FromBody] dynamic config)
        {
            try
            {
                ScriptLinkAction action;
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    var title = config["Title"].ToString();
                    var scriptSrc = config["ScriptSrc"].ToString();
                    var sequence = config["Sequence"].ToString();

                    action = SaveCustomActionScriptLink(clientContext, title, scriptSrc, sequence, true);                 
                }

                return Request.CreateResponse(HttpStatusCode.OK, action);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }

        [HttpPost]
        public dynamic Updatescript([FromBody] dynamic config)
        {
            try
            {
                ScriptLinkAction action;
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    var title = config["Title"].ToString();
                    var scriptSrc = config["ScriptSrc"].ToString();
                    var sequence = config["Sequence"].ToString();

                    action = SaveCustomActionScriptLink(clientContext, title, scriptSrc, sequence,false);
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
            foreach (UserCustomAction customAction in customActions)
            {
                if (customAction.Title == HEADER)
                {
                    configurations.HeaderScripts = customAction.ScriptBlock;
                }
                else if (customAction.Title != LOAD_SCRIPT &&
                    !String.IsNullOrEmpty(customAction.ScriptSrc))
                {
                    configurations.ScriptLinks.Add(new ScriptLinkAction { Title = customAction.Title, ScriptSrc = customAction.ScriptSrc, Sequence = customAction.Sequence });
                }
            }

            return configurations;
        }

        private void SaveCustomActionScriptBlock(ClientContext clientContext, string configurations)
        {
            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
            Configurations existing = GetConfigurations(clientContext, out customActions);

            var headerScriptAction = customActions.FirstOrDefault(x => x.Title.Equals(HEADER));
            var loadScriptAction = customActions.FirstOrDefault(x => x.Title.Equals(LOAD_SCRIPT));
            var loadCSSAction = customActions.FirstOrDefault(x => x.Title.Equals(LOAD_CSS));

            //Register our help Load Script so people can load external scripts
            if (loadScriptAction == null)
            {
                string loadScript = "function LoadScript(scriptUrl) { " +
                                    "   var genesisLoadScript = document.createElement('SCRIPT'); " +
                                    "   genesisLoadScript.type = 'text/javascript'; " +
                                    "   genesisLoadScript.src = scriptUrl; " +
                                    "   document.getElementsByTagName('head')[0].appendChild(genesisLoadScript);" +
                                    "}";

                UserCustomAction customLoadScriptAction = customActions.Add();
                customLoadScriptAction.Location = "ScriptLink";
                customLoadScriptAction.ScriptBlock = loadScript;
                customLoadScriptAction.Title = LOAD_SCRIPT;
                customLoadScriptAction.Update();
            }

            //Register our help Load CSS so people can load external scripts
            if (loadCSSAction == null)
            {
                string loadCSS = "function LoadCSS(scriptUrl) { " +
                                    "   var genesisLoadScript = document.createElement('link'); " +
                                    "   genesisLoadScript.rel = 'stylesheet'; " +
                                    "   genesisLoadScript.type = 'text/css'; " +
                                    "   genesisLoadScript.href = scriptUrl; " +
                                    "   document.getElementsByTagName('head')[0].appendChild(genesisLoadScript);" +
                                    "}";

                UserCustomAction customLoadCSSAction = customActions.Add();
                customLoadCSSAction.Location = "ScriptLink";
                customLoadCSSAction.ScriptBlock = loadCSS;
                customLoadCSSAction.Title = LOAD_CSS;
                customLoadCSSAction.Update();
            }

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

        /// <summary>
        /// Useful helper function in case we make a mistake and need to delete the actions we've created
        /// for the script block
        /// </summary> 
        private void CleanupScriptBlock(ClientContext clientContext, UserCustomAction headerScriptAction, UserCustomAction loadScriptAction, UserCustomAction loadCSSAction)
        {
            if (headerScriptAction != null)
            {
                headerScriptAction.DeleteObject();
                clientContext.Load(headerScriptAction);
            }

            if (loadScriptAction != null)
            {
                loadScriptAction.DeleteObject();
                clientContext.Load(loadScriptAction);
            }

            if (loadCSSAction != null)
            {
                loadCSSAction.DeleteObject();
                clientContext.Load(loadCSSAction);
            }
            clientContext.ExecuteQuery();
        }


        private ScriptLinkAction SaveCustomActionScriptLink(ClientContext clientContext, string title, string scriptSrc, string sequence, bool isNew)
        {          
            //TODO: duplicate check

            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;     

            UserCustomAction customAction = null;
            if (isNew)
            {
                customAction = customActions.Add();
            }
            else {
                clientContext.Load(customActions);
                clientContext.ExecuteQuery();
                customAction = customActions.FirstOrDefault(x => x.Title.Equals(title));
            }
         
            customAction.Location = "ScriptLink";
            customAction.ScriptSrc = scriptSrc;
            customAction.Title = title;

            int sequenceNum = 0;
            int.TryParse(sequence, out sequenceNum);
            customAction.Sequence = sequenceNum;

            customAction.Update();  

            clientContext.Load(customAction);
            clientContext.ExecuteQuery();

            return new ScriptLinkAction { 
                Title = customAction.Title, 
                ScriptSrc = customAction.ScriptSrc, 
                Sequence = customAction.Sequence 
            };

        }

        private void DeleteCustomActionScriptLink(ClientContext clientContext, ScriptLinkAction scriptLink)
        {

            UserCustomActionCollection customActions = clientContext.Site.UserCustomActions;
            Configurations existing = GetConfigurations(clientContext, out customActions);
            var headerScriptAction = customActions.FirstOrDefault(x => x.Title.Equals(scriptLink.Title));

            if (headerScriptAction != null)
            {
                headerScriptAction.DeleteObject();
            }

            clientContext.Load(headerScriptAction);
            clientContext.ExecuteQuery();
        }
      
      
    }


    public class Configurations
    {
        public string HeaderScripts { get; set; }
        public List<ScriptLinkAction> ScriptLinks { get; set; }
    }

    public class ScriptLinkAction {
        public string Title { get; set; }
        public string ScriptSrc { get; set; }
        public int Sequence { get; set; }
        public bool IsEditing { get; set; }
    }
}
