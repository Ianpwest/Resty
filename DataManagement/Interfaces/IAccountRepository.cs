using DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Interfaces
{
    public interface IAccountRepository : IDisposable
    {
        bool CheckUsernameUnique(string username);

        bool AddUser(AccountModel accountModel);

        ServiceCallResultModel ValidateUserLogOn(LogOnModel model);

        bool UpdateUserToken(string username, string token);

        bool ValidateTokenExists(string token);

        string GetUserNameFromToken(string token);
        string GetSaltForUser(string username);
        bool RegisterAccount(AccountModel account);
    }
}
