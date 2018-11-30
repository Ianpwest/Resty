namespace DataManagement.Models
{
    public class LogOnResultModel
    {
        public bool bSuccessful { get; set; }

        public string FailureReason { get; set; }

        public string Token { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
