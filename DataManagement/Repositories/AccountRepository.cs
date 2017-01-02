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

        public bool AddUser(AccountModel accountModel)
        {
            throw new NotImplementedException();
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

            db.User.Add(user);

            return SaveChanges(db);

        }

        public void Dispose()
        {
            db.Dispose();
        }

        
    }
}
