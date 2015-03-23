using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FTIUploadServer.Util;

namespace FTIUploadServer.Controllers.V1
{
    /// <summary>
    /// API that handles Directory operations
    /// </summary>
    public class DirectoryController : ApiController
    {
        /// <summary>
        /// Get: api/{namespace}/Directory
        /// Gets the paths of the directories under the root directory
        /// </summary>
        /// <returns>list of strings - paths of directories under the target path </returns>
        public IEnumerable<string> Get()
        {
            // Set the base directory in the configuration
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];

            if (!Directory.Exists(baseSaveLocation))
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Path not found"));
                yield break;
            }

            if (Directory.Exists(baseSaveLocation))
            {
                DirectoryInfo di = new DirectoryInfo(baseSaveLocation);
                foreach (var directoryPath in di.GetDirectories())
                {
                    yield return NetworkPath.CleanPath(directoryPath.Name);
                }
            }
        }

        /// <summary>
        /// Get: api/{namespace}/GetDirectoryNames under the root folder
        /// </summary>
        /// <returns>list of strings - directory names</returns>
        [Route("api/{namespace}/GetDirectoryNames")]
        public IEnumerable<string> GetNames()
        {

            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];

            if (!Directory.Exists(baseSaveLocation))
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Path not found"));
                yield break;
            }

            if (Directory.Exists(baseSaveLocation))
            {
                DirectoryInfo di = new DirectoryInfo(baseSaveLocation);
                foreach (var directoryPath in di.GetDirectories())
                {
                    yield return directoryPath.Name;
                }
            }

        }

        /// <summary>
        /// GET: api/{namespace}/Directory?Path={path}
        // Example path:  /root/path/to/directory/
        /// Gets the directory paths under the specified path
        /// </summary>
        /// <param name="path">string - sub path of direcory to get listing for.</param>
        /// <returns>list of strings - paths of directories under the target path </returns>
        public IEnumerable<string> Get(string path)
        {
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            if (!Directory.Exists(baseSaveLocation))
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Path not found"));
                yield break;
            }

            var combine = NetworkPath.Combine(baseSaveLocation, path);
            
            if (Directory.Exists(combine))
            {
                DirectoryInfo di = new DirectoryInfo(combine);
                foreach (var directoryPath in di.GetDirectories())
                {
                    yield return NetworkPath.Combine(path, directoryPath.Name);
                }
            }
        }

        /// <summary>
        /// GET api/{namespace}/GetDirectoryNames
        /// /// Gets the directory paths names the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>list of strings - directory names for the target path</returns>
        [Route("api/{namespace}/GetDirectoryNames")]
        public IEnumerable<string> GetDirNames(string path)
        {
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            if (!Directory.Exists(baseSaveLocation))
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Path not found"));
                yield break;
            }

            var combine = NetworkPath.Combine(baseSaveLocation, path);

            if (Directory.Exists(combine))
            {
                DirectoryInfo di = new DirectoryInfo(combine);
                foreach (var directoryPath in di.GetDirectories())
                {
                    yield return directoryPath.Name;
                }
            }
        }

        /// <summary>
        /// PUT: Create a directory at the specified path
        /// </summary>
        /// <param name="path">string - path to create</param>
        /// <returns>HttpResponseMessage</returns>
        public HttpResponseMessage Put(string path)
        {
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            string dirPath = NetworkPath.Combine(baseSaveLocation, path);
            try
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            catch (Exception exception)
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }

            return Request.CreateResponse(HttpStatusCode.OK, path);
            
        }

        /// <summary>
        /// DELETE: DELETE a directory at the specified path
        /// </summary>
        /// <param name="path">string - path to create</param>
        /// <returns>HttpResponseMessage</returns>
        public HttpResponseMessage Delete(string path)
        {
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            string dirPath = NetworkPath.Combine(baseSaveLocation, path);
            try
            {
                if(Directory.Exists(dirPath))
                    Directory.Delete(dirPath,true);
            }
            catch (Exception exception)
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, path);
        }
    }
}
