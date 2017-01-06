using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Interfaces
{
    public interface IImageRepository : IDisposable
    {
        string GetImageSASShortExpirationURI(string fileName);
        string GetImageSASLongExpirationURI(string fileName);

        bool UploadImageToCloud(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string localFilePath);
        bool UploadProfileImageMetadataToDatabase(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string userId);
      
    }
}
