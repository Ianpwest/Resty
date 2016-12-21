using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class ServiceCallResultModel
    {
        public bool bSuccessful { get; set; }

        public string FailureReason { get; set;}

        public string Token { get; set; }
    }
}