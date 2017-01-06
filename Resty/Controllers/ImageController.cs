using DataManagement.Models;
using Resty.Authorization;
using Resty.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Resty.Controllers
{
    public class ImageController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetImageURI([FromBody]ImageModel model)
        {
            if (model == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new ImageModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            string imageURI = ImageUtilities.GetImageSASShortExpirationURI(model.FileName);

            if (string.IsNullOrEmpty(imageURI)) //Check valid user
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });
            }
           
            return Ok(new ImageModel() {bSuccessful = true, URI = imageURI });
            
        }

        
        [HttpPost]
        [AuthenticateTokenAttribute]
        public async Task<HttpResponseMessage> UploadProfileImage()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Unsupported Media Type" });
            }
            

            var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());//This works for azure local file system temp disk space.

            try
            {
                // Read the form data.
                var result = await Request.Content.ReadAsMultipartAsync(provider);

                if(provider.FileData.Count > 1)
                    return Request.CreateResponse(HttpStatusCode.OK, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Only one file at a time is permitted" });

                Guid uniqueFileIdentifier = new Guid();
                string friendlyFileName = string.Empty;

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    uniqueFileIdentifier = Guid.NewGuid();

                    friendlyFileName = file.Headers.ContentDisposition.FileName.Replace('"', ' ').Trim();
                    string contentType = file.Headers.ContentType.MediaType;
                    string localFilePath = file.LocalFileName;

                    //Upload image to database
                    if (!ImageUtilities.UploadProfileImageMetadataToDatabase(friendlyFileName, contentType, uniqueFileIdentifier, (User as MyPrincipal).UserName))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed to add to database. Please try again." });
                    }

                    //Upload image to cloud storage
                    if (!ImageUtilities.UploadImageToCloud(friendlyFileName, contentType, uniqueFileIdentifier, localFilePath))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed to add to cloud db. Please try again." });
                    }
                }

                //Get SAS string for image. 
                string URI = ImageUtilities.GetImageSASLongExpirationURI(uniqueFileIdentifier.ToString() + friendlyFileName);
                return Request.CreateResponse(HttpStatusCode.OK, new ImageModel() { bSuccessful = true, URI = URI });
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.ToString());
            }
        }
    }
}
