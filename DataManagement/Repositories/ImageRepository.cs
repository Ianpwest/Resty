using DataManagement.Interfaces;
using DataManagement.Utilities;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Repositories
{
    public class ImageRepository : BaseRepository, IImageRepository
    {
        private WithUDBEntities db { get; set; }

        private CloudBlobContainer imageBlobContainer { get; set; }

        public ImageRepository()
        {
            db = new WithUDBEntities();

            imageBlobContainer = ImageUtilities.GetCloudBlobContainer(Constants.Constants.CONTAINER_IMAGES); 
        }

        public string GetImageSASShortExpirationURI(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            return ImageUtilities.GetBlobSasUri(imageBlobContainer, fileName, Constants.Constants.SHORT_SAS_DURATION);
        }


        public string GetImageSASLongExpirationURI(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            return ImageUtilities.GetBlobSasUri(imageBlobContainer, fileName, Constants.Constants.LONG_SAS_DURATION);
        }

        public bool UploadImageToCloud(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string localFilePath)
        {
            if (string.IsNullOrEmpty(friendlyFileName) || string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(localFilePath))
                return false;

            CloudBlobContainer cloudBlobImageContainer = ImageUtilities.GetCloudBlobContainer(Constants.Constants.CONTAINER_IMAGES);

            CloudBlockBlob blob = cloudBlobImageContainer.GetBlockBlobReference(uniqueFileIdentifier.ToString() + friendlyFileName);

            try
            {
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = System.IO.File.OpenRead(localFilePath))
                {
                    blob.UploadFromStream(fileStream);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                try
                {
                    System.IO.File.Delete(localFilePath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }

            return true;
        }

        public bool UploadProfileImageMetadataToDatabase(string friendlyFileName, string contentType, Guid uniqueFileIdentifier, string userId)
        {
            if (string.IsNullOrEmpty(friendlyFileName) || string.IsNullOrEmpty(contentType))
                return false;

            //Add the file
            var profilePicture = new File();
            profilePicture.FileId = uniqueFileIdentifier;
            profilePicture.Name = friendlyFileName;
            profilePicture.ContentType = contentType;
            profilePicture.Container = Constants.Constants.CONTAINER_IMAGES;

            db.File.Add(profilePicture);

            if (!SaveChanges(db))
                return false;

            //Check to see if the user already has a profile picture
            var files = (from r in db.User_File
                         where r.UserId == userId
                         select r).ToList();

            foreach (User_File file in files)
            {
                file.IsProfile = false;
            }

            //Add the user file
            var userFile = new User_File();
            userFile.UserFileId = Guid.NewGuid();
            userFile.FileId = profilePicture.FileId;
            userFile.UserId = userId;
            userFile.IsProfile = true;

            db.User_File.Add(userFile);

            return SaveChanges(db);
        }
        
        public void Dispose()
        {
            db.Dispose();
        }

        
    }
}
