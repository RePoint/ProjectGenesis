using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.SharePoint.Client;
using System.Net;

namespace GenesisWeb.api
{
    public class ListsController : ApiController
    {    
        [HttpPost]
        public dynamic GetLists([FromBody] dynamic config)
        {
            try
            {
                ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString());

                var lists = clientContext.Web.Lists;
                clientContext.Load(lists);
                clientContext.ExecuteQuery();

                var array = from list in lists
                            select new
                            {
                                title = list.Title,
                                id = list.Id.ToString()
                            };


                return Request.CreateResponse(HttpStatusCode.OK, new { lists = array });
            } catch(Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }
    }
}
