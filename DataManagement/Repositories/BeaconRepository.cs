using DataManagement.Interfaces;
using DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Repositories
{
    public class BeaconRepository : BaseRepository, IBeaconRepository
    {
        private CLPTrackerEntities db { get; set; }

        public BeaconRepository()
        {
            db = new CLPTrackerEntities();
        }

        public void LogUserActivity(UserActivityModel userActivityModel)
        {
            if (userActivityModel == null)
            {
                throw new ArgumentNullException(nameof(userActivityModel));
            }


        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
