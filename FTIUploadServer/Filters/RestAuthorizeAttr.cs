using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using WebMatrix.WebData;

namespace FTIUploadServer.Filters
{
    /// <summary>
    /// Basic start of simple authentication.  
    /// <remarks>(Currently not used or needed, but maybe necessary in the future.)</remarks>
    /// </summary>
    public class RestAuthorizeAttr : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    var credsRaw = authHeader.Parameter;
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var creds = encoding.GetString(Convert.FromBase64String(credsRaw));
                    var splitcreds = creds.Split(':');
                    var userName = splitcreds[0];
                    var password = splitcreds[1];

                    if (WebSecurity.Login(userName, password))
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                        Thread.CurrentPrincipal = principal;
                        return;
                    }
                }
            }

            HandleUnAuthorized(actionContext);
        }

        private void HandleUnAuthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate","Basic Scheme='FTIUploader' location=''");
        }
    }
}