using DataManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement.Models;

namespace DataManagement.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private WithUDBEntities db { get; set; }

        public AccountRepository()
        {
            db = new WithUDBEntities();
        }

        public bool IsEmailUnique(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var user = (from r in db.User
                        where r.EmailAddress == email
                        select r).FirstOrDefault();

            //No user found
            if (user == null)
                return true;

            return false;
        }

        public LogOnResultModel ValidateUserLogOn(LogOnModel model)
        {
            var user = (from r in db.User
                        where r.EmailAddress == model.Email
                        && r.Password == model.Password
                        select r).FirstOrDefault();

            if (user == null)
                return new LogOnResultModel() { bSuccessful = false, FailureReason = "Username/Password incorrect. Please try again." };

            if(!user.IsActivated)
                return new LogOnResultModel() { bSuccessful = false, FailureReason = "Please check your email to activate your account." };

            return new LogOnResultModel() { bSuccessful = true, FirstName = user.FirstName, LastName = user.LastName };
        }

        public bool UpdateUserToken(string email, string token)
        {
            var user = (from r in db.User
                       where r.EmailAddress == email
                        select r).FirstOrDefault();

            if (user == null)
                return false;

            user.Token = token;

            return SaveChanges(db);
        }

        public bool ValidateTokenExists(string token)
        {
            var user = (from r in db.User
                        where r.Token == token
                        select r).FirstOrDefault();

            if (user == null)
                return false;

            return true;
        }

        public string GetEmailFromToken(string token)
        {
            var emailAddress = (from r in db.User
                            where r.Token == token
                            select r.EmailAddress).FirstOrDefault();

            return emailAddress;
        }

        public string GetSaltForUser(string email)
        {
            var salt = (from r in db.User
                        where r.EmailAddress == email
                        select r.Salt).FirstOrDefault();

            return salt;
        }

        public bool RegisterAccount(AccountModel account)
        {
            if (account == null)
                return false;

            User user = new DataManagement.User();

            user.FirstName = account.FirstName;
            user.LastName = account.LastName;
            user.Password = account.Password;
            user.Salt = account.Salt;
            user.EmailAddress = account.Email;
            user.DateCreated = DateTime.Now;
            user.ActivationToken = account.ActivationToken;

            db.User.Add(user);

            return SaveChanges(db);

        }

        public bool ValidateEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var user = (from r in db.User
                        where r.EmailAddress == email
                        select r).FirstOrDefault();

            if (user == null)
                return false;

            return true;
        }

        public bool UpdateUsersResetPasswordToken(string email, string resetToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetToken))
                return false;

            var user = (from r in db.User
                        where r.EmailAddress == email
                        select r).FirstOrDefault();

            if (user == null)
                return false;

            user.ResetPasswordToken = resetToken;

            return SaveChanges(db);
        }

        public ServiceCallResultModel ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel == null)
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Invalid parameter" };

            var user = (from r in db.User
                        where r.EmailAddress == resetPasswordModel.Email
                        && r.ResetPasswordToken == resetPasswordModel.ResetToken
                        select r).FirstOrDefault();

            if (user == null)
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Invalid code. Please try again." };

            user.ResetPasswordToken = null;
            user.Password = resetPasswordModel.Password;
            user.Salt = resetPasswordModel.Salt;

            if(SaveChanges(db))
            {
                return new ServiceCallResultModel() { bSuccessful = true};
            }
            else
            {
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Unable to update database. Please try again." };
            }

        }

        public ServiceCallResultModel ActivateAccount(string activationToken)
        {
            if (string.IsNullOrEmpty(activationToken))
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Invalid parameters" };

            var user = (from r in db.User
                        where r.ActivationToken == activationToken
                        select r).FirstOrDefault();

            if (user == null)
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Token not valid" };

            user.IsActivated = true;

            if (SaveChanges(db))
            {
                return new ServiceCallResultModel() { bSuccessful = true };
            }

            return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed to update database. Please try again" };
        }

        public string GetActivationTokenForUser(string email)
        {
            var user = (from r in db.User
                        where r.EmailAddress == email
                        select r).FirstOrDefault();

            if (user == null)
                return string.Empty;

            return user.ActivationToken;
        }

        public LogOnResultModel UpdateProfileInformation(AccountModel account)
        {
            if (account == null)
                return new LogOnResultModel() { bSuccessful = false, FailureReason = "Argument null" };

            var user = (from r in db.User
                        where r.EmailAddress == account.Email
                        select r).FirstOrDefault();

            if(user == null)
                return new LogOnResultModel() { bSuccessful = false, FailureReason = "User not found" };

            user.LastName = account.LastName;
            user.FirstName = account.FirstName;

            if(!SaveChanges(db))
            {
                return new LogOnResultModel() { bSuccessful = false, FailureReason = "Unable to update database" };
            }

            return new LogOnResultModel() { bSuccessful = true, FirstName = account.FirstName, LastName = account.LastName, Token = user.Token };
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public string GetUsersProfileImageFileName(string email)
        {
            var user = (from r in db.User_File
                        where r.UserId == email
                        && r.IsProfile == true
                        select r).FirstOrDefault();

            if (user == null)
                return string.Empty;

            return user.FileId + Constants.Constants.PROFILE_IMAGE_SUFFIX;
        }
    }
}
