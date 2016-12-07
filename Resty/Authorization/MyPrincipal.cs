using System;
using System.Security.Principal;

namespace Resty.Authorization
{
    public class MyPrincipal : IMyPrincipal
    {
        public string UserName { get; set; }

        public MyPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public System.Security.Principal.IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}