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
        bool IsEmailUnique(string email);

        LogOnResultModel ValidateUserLogOn(LogOnModel model);

        bool UpdateUserToken(string email, string token);

        bool ValidateTokenExists(string token);

        string GetEmailFromToken(string token);
        string GetSaltForUser(string email);
        bool RegisterAccount(AccountModel account);
        bool ValidateEmailExists(string email);
        bool UpdateUsersResetPasswordToken(string email, string resetToken);
        ServiceCallResultModel ResetPassword(ResetPasswordModel resetPasswordModel);
        ServiceCallResultModel ActivateAccount(string activationToken);
    }
}
