using System;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataManagement.Models;
using Resty.Models;
using DataManagement.Interfaces;
using DataManagement.Repositories;
using System.Security.Cryptography;

namespace Resty.Utilities
{
    public static class AccountUtilities
    {
        internal static LogOnResultModel ValidateUserLogOn(LogOnModel model)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                //Get the salt for this user
                string strSaltResult = repo.GetSaltForUser(model.Email);

                if(strSaltResult == null)
                    return new LogOnResultModel() { bSuccessful = false, FailureReason = "Account not found. Please try again." };

                //Get the hash for this user given what they typed for their password and their salt
                string strHashToCheck = GetHashPassword(model.Password, strSaltResult);

                //Set the hash as the password to check the database
                model.Password = strHashToCheck;

                //Validate the user
                LogOnResultModel returnModel = repo.ValidateUserLogOn(model);

                //If successfully validated generate new token for user
                if(returnModel.bSuccessful)
                {
                    returnModel.Token = AccountUtilities.GenerateToken();

                    //Store Token for user
                    if(repo.UpdateUserToken(model.Email, returnModel.Token))
                    {
                        return returnModel;
                    }
                    else
                    {
                        return new LogOnResultModel() { bSuccessful = false, FailureReason = "Unable to get token." };
                    }
                }
                
                return returnModel;
            }
        }

        internal static ServiceCallResultModel RegisterAccount(AccountModel account)
        {
            account.Salt = GetSalt();
            account.Password = Encrypt(account.Salt + account.Password);
            account.ActivationToken = GenerateToken();

            using (IAccountRepository repo = new AccountRepository())
            {
                //Verify username not already in use
                if (!repo.IsEmailUnique(account.Email))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Email already exists. Please enter another." };
                }

                if(!repo.RegisterAccount(account))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Cannot contact database at this time. Please try again." };
                }

                if (!SendActivationEmail(account.Email, account.ActivationToken))
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed to send activation email" };

                return new ServiceCallResultModel() { bSuccessful = true };
            }
        }

        internal static ServiceCallResultModel GetResetPasswordTokenForUser(string email)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                if(!repo.ValidateEmailExists(email))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Email not found" };
                }
            }

            var resetToken = GenerateToken().Substring(0, 5);

            //Store the reset token against the user.
            using (IAccountRepository repo = new AccountRepository())
            {
                if (!repo.UpdateUsersResetPasswordToken(email, resetToken))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failure. Please try again." };
                }
            }

            //Send the reset password email
            if (!SendResetPasswordEmail(email, resetToken))
            {
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed to send email. Please try again." };
            }

            return new ServiceCallResultModel() { bSuccessful = true };
        }

        internal static LogOnResultModel UpdateProfileInformation(AccountModel account)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.UpdateProfileInformation(account);
            }
        }

        internal static void ResendActivationEmail(string email)
        {
            string activationToken;

            using (IAccountRepository repo = new AccountRepository())
            {
                activationToken = repo.GetActivationTokenForUser(email);
            }

            SendActivationEmail(email, activationToken);
        }

        internal static ServiceCallResultModel ActivateAccount(string activationToken)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.ActivateAccount(activationToken);
            }
        }

        internal static ServiceCallResultModel ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel == null)
                return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Failed. Please try again." };

            resetPasswordModel.Salt = GetSalt();
            resetPasswordModel.Password = Encrypt(resetPasswordModel.Salt + resetPasswordModel.Password);

            ServiceCallResultModel result = new ServiceCallResultModel();
            using (IAccountRepository repo = new AccountRepository())
            {
               return repo.ResetPassword(resetPasswordModel);
            }
        }

        internal static string GetUserNameFromToken(string token)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.GetEmailFromToken(token);
            }
        }

        internal static bool ValidateTokenExists(string token)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.ValidateTokenExists(token);
            }
        }

        private static bool SendActivationEmail(string email, string activationToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(activationToken))
                return false;

            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("AccountActivation@WithU.com");

            myMessage.AddTo(email);

            myMessage.Subject = "Account activation";

            //Add the HTML and Text bodies
            myMessage.Html = "<h4>Welcome to WithU! Please activate your account by clicking the link below</h4><br/>";
            myMessage.Html += "<a href='https://resty.azurewebsites.net/Activation/ActivateAccount?activationToken=" + activationToken + "'>Click me to activate</a>";

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential("azure_d6838b6e2dbb20423623ce113e70b114@azure.com", "Snowheyoh1");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email, which returns an awaitable task.
            transportWeb.DeliverAsync(myMessage);

            return true;
        }

        private static bool SendResetPasswordEmail(string email, string resetToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetToken))
                return false;

            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("PasswordReset@WithU.com");

            myMessage.AddTo(email);

            myMessage.Subject = "Password Reset Token: " + resetToken;

            //Add the HTML and Text bodies
            myMessage.Text = "Password reset token: " + resetToken;

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential("azure_d6838b6e2dbb20423623ce113e70b114@azure.com", "Snowheyoh1");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email, which returns an awaitable task.
            transportWeb.DeliverAsync(myMessage);

            return true;
        }

        private static string GenerateToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return token;
        }

        private static string GetHashPassword(string password, string strSaltResult)
        {
            string strPasswordToCompare = strSaltResult + password;

            return Encrypt(strPasswordToCompare);
        }

        private static String Encrypt(string strToEncrypt)
        {
            string strEncryptedPassword = string.Empty;

            var bytes = System.Text.Encoding.UTF8.GetBytes(strToEncrypt);

            using (SHA512 shaM = new SHA512Managed())
            {
                strEncryptedPassword = System.Text.Encoding.UTF8.GetString(shaM.ComputeHash(bytes)); 
            }
            
            return strEncryptedPassword;
        }

        private static String GetSalt()
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[16];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }
    }
}