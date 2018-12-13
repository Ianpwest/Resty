using DataManagement.Models;
using DataManagement.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resty.Utilities
{
    public static class BeaconUtilities
    {
        public static void LogUserActivity(UserActivityModel userActivityModel)
        {
            using (var repo = new BeaconRepository())
            {
                repo.LogUserActivity(userActivityModel);
            }
        }
    }
}