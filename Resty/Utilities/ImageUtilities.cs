using DataManagement.Interfaces;
using DataManagement.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resty.Utilities
{
    public static class ImageUtilities
    {
        internal static string GetImageSASShortExpirationURI(string fileName)
        {
            string returnURI = string.Empty;

            using (IImageRepository repo = new ImageRepository())
            {
                returnURI = repo.GetImageSASShortExpirationURI(fileName);
            }

            return returnURI;
        }

        internal static string GetImageSASLongExpirationURI(string fileName)
        {
            string returnURI = string.Empty;

            using (IImageRepository repo = new ImageRepository())
            {
                returnURI = repo.GetImageSASLongExpirationURI(fileName);
            }

            return returnURI;
        }

        internal static bool UploadImageToCloud(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string localFilePath)
        {
            using (IImageRepository repo = new ImageRepository())
            {
                return repo.UploadImageToCloud(friendlyFileName, contentType, uniqueFileIdentifier, localFilePath);
            }
        }

        internal static bool UploadProfileImageMetadataToDatabase(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string userId)
        {
            using (IImageRepository repo = new ImageRepository())
            {
                return repo.UploadProfileImageMetadataToDatabase(friendlyFileName, contentType, uniqueFileIdentifier, userId);
            }
        }

        
    }
}