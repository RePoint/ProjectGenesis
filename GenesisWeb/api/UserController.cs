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
    public class UserController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetCurrent([FromBody] dynamic config)
        {
            try
            {
                User user;              
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(config["SPHostUrl"].ToString(), config["SPAppToken"].ToString()))
                {
                    user = clientContext.Web.CurrentUser;
                    clientContext.Load(user);
                    clientContext.Load(user.Groups);                  
                    clientContext.ExecuteQuery();
                }

                var result = new
                {
                    username = user.Title,
                    groups = user.Groups.Select(g => g.Title).ToArray()                  
                };


                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, APIHelper.WriteErrorInfo(ex));
            }
        }

    }
}
