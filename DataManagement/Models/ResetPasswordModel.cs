using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class ResetPasswordModel
    {
        public string ResetToken { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }
    }
}