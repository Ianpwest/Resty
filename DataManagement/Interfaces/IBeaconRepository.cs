using DataManagement.Models;
using System;

namespace DataManagement.Interfaces
{
    public interface IBeaconRepository : IDisposable
    {
        void LogUserActivity(UserActivityModel userActivityModel);
    }
}
