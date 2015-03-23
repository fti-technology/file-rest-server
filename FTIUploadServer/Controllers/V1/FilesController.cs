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
    // API that handles File level operations
    /// </summary>
    public class FilesController : ApiController
    {
        // GET: api/Files
        public void Get()
        {
            Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ("Not implemented"));
        }

        /// <summary>
        /// Gets a list of file names in a target directory.
        /// We assume that branch is the root folder and dir is the sub-directory.
        /// The decimal validation is done becuase TFS directory paths are in the form of : yyyymmdd.vv
        /// </summary>
        /// <param name="Branch">Path of sub-directory to operation on</param>
        /// <param name="Dir">Specific directory to get the file listing</param>
        /// <returns>List of strings - file names</returns>
        [Route("api/{namespace}/Files/{Branch}/{Dir:decimal}")]
        public IEnumerable<string> Get(string Branch, decimal Dir)
        {
            var subDir = Convert.ToString(Dir);
            return FilesListing(Branch, subDir);
        }

        /// <summary>
        /// Gets a list of file names in a target directory.
        /// We assume that branch is the root folder and dir is the sub-directory.
        /// This version handles a normal string Dir parameter.
        /// </summary>
        /// <param name="Branch">Path of sub-directory to operation on</param>
        /// <param name="Dir">Specific directory to get the file listing</param>
        /// <returns>List of strings - file names</returns>
        [Route("api/{namespace}/Files/{Branch}/{Dir}")]
        public IEnumerable<string> Get(string Branch, string Dir)
        {
            return FilesListing(Branch, Dir);
        }

        /// <summary>
        /// Get the file list for the directory
        /// /BRANCH/Dir
        /// </summary>
        /// <param name="Branch">Path of sub-directory to operation on</param>
        /// <param name="Dir">Specific directory to get the file listing</param>
        /// <returns>List of strings - file names</returns>
        private IEnumerable<string> FilesListing(string Branch, string Dir)
        {
            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            if (Directory.Exists(baseSaveLocation))
            {   
                var combine = NetworkPath.Combine(baseSaveLocation, Branch);
                combine = NetworkPath.Combine(combine, Dir);
                if (Directory.Exists(combine))
                {
                    DirectoryInfo di = new DirectoryInfo(combine);
                    foreach (var filePath in di.GetFiles())
                    {
                        yield return filePath.Name;
                    }
                }
                else
                {
                    Request.CreateErrorResponse(HttpStatusCode.SeeOther, new HttpError("Path not found"));
                }
            }
        }

        // POST: api/Files
        public void Post([FromBody]string value)
        {
            Request.CreateErrorResponse(HttpStatusCode.NotImplemented,("Not implemented"));
        }

        // PUT: api/Files/5
        public void Put(int id, [FromBody]string value)
        {
            Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ("Not implemented"));
        }

        // DELETE: api/Files/5
        public void Delete(int id)
        {
            Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ("Not implemented"));
        }
    }
}
