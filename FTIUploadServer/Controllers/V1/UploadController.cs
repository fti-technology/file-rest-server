using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using FTIUploadServer.Util;

namespace FTIUploadServer.Controllers.V1
{
    /// <summary>
    /// Handle uploads using MultipartFormDataStreamProvider
    /// </summary>
    public class UploadController : ApiController
    {
        /// <summary>
        /// Upload a file - path is the local folder path to save the file to which is relative to the local base directory.
        /// Not really interested in handling multiple files at the same time.
        /// </summary>
        /// <param name="path">string - a path in the form /Dir1/Dir2 to store the file into.</param>
        /// <returns>HttpResponse</returns>
        public async Task<HttpResponseMessage> Post(string path)
        {
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string baseSaveLocation = ConfigurationManager.AppSettings["DataDirectoryPath"];
            if (string.IsNullOrEmpty(baseSaveLocation))
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Server Configuration not correct, DataDirectoryPath not set."));
            }

            if (string.IsNullOrEmpty(path))
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Path can not be an empty value."));
            }

            path = NetworkPath.Combine(baseSaveLocation, path);

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(path);
            var files = new List<string>();
            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                //await streamContent.ReadAsMultipartAsync(provider);

                files.AddRange(provider.FileData.Select(file => Path.GetFileName(file.LocalFileName)));

                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }

        // We implement MultipartFormDataStreamProvider to override the filename of File which
        // will be stored on server, or else the default name will be of the format like Body-
        // Part_{GUID}. In the following implementation we simply get the FileName from 
        // ContentDisposition Header of the Request Body.
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
}

