using System;
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
        internal static ServiceCallResultModel ValidateUserLogOn(LogOnModel model)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                //Get the salt for this user
                string strSaltResult = repo.GetSaltForUser(model.Username);

                if(strSaltResult == null)
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "User password salt not found" };

                //Get the hash for this user given what they typed for their password and their salt
                string strHashToCheck = GetHashPassword(model.Password, strSaltResult);

                //Set the hash as the password to check the database
                model.Password = strHashToCheck;

                //Validate the user
                ServiceCallResultModel returnModel = repo.ValidateUserLogOn(model);

                //If successfully validated generate new token for user
                if(returnModel.bSuccessful)
                {
                    returnModel.Token = AccountUtilities.GenerateToken();

                    //Store Token for user
                    if(repo.UpdateUserToken(model.Username, returnModel.Token))
                    {
                        return returnModel;
                    }
                    else
                    {
                        return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Unable to get token." };
                    }
                }
                
                return returnModel;
            }
        }

        internal static ServiceCallResultModel RegisterAccount(AccountModel account)
        {
            account.Salt = GetSalt();
            account.Password = Encrypt(account.Salt + account.Password);

            using (IAccountRepository repo = new AccountRepository())
            {
                //Verify username not already in use
                if (!repo.IsUsernameUnique(account.Username))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Username already exists. Please enter another." };
                }

                if(!repo.RegisterAccount(account))
                {
                    return new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Cannot contact database at this time. Please try again." };
                }

                return new ServiceCallResultModel() { bSuccessful = true };
            }
        }

        internal static string GetUserNameFromToken(string token)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.GetUserNameFromToken(token);
            }
        }

        internal static bool ValidateTokenExists(string token)
        {
            using (IAccountRepository repo = new AccountRepository())
            {
                return repo.ValidateTokenExists(token);
            }
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
                strEncryptedPassword = shaM.ComputeHash(bytes).ToString();
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