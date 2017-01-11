using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class LogOnResultModel
    {
        public bool bSuccessful { get; set; }

        public string FailureReason { get; set; }

        public string Token { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileURI { get; set; }
    }
}
