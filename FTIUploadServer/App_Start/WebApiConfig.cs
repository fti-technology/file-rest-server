using System.Web.Http;
using System.Web.Http.Hosting;

namespace FTIUploadServer
{
    /// <summary>
    /// The purpose of this web app is to expose a basic API for uploading files and mirroring a local directory structure on the remote machine.
    /// In our case we use this application to mirror a file share between two interal networks that don't have direct file share access.
    /// Given that point, there isn't much security built into this WebApi application, as it is used internally on LAN side and not exposed to internet.
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Services.Replace(typeof(IHostBufferPolicySelector), new CustomWebHostBufferPolicySelector());
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Use the namespace in the API to handle future breaking changes in the API
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{namespace}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
