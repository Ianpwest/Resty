using System;

namespace Resty.Authorization
{
    public static class TokenManager
    {
        public static string GenerateToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return token;
        }

        public static bool StoreTokenForUser(string token, string userId, DateTime tokenExpires)
        {
            //Add token to database against user.
            return true;
        }

        public static int CheckValidToken(string token)
        {
            //Returns userid if valid token

            return -1;
        }
    }
}