using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class AccountModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }
    }
}
