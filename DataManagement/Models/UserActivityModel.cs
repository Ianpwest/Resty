using System;

namespace DataManagement.Models
{
    public class UserActivityModel
    {
        public string UserId { get; set; }

        public string BeaconIdentifier { get; set; }

        public double BeaconDistance { get; set; }

        public DateTime DateReceived { get; set; }
    }
}