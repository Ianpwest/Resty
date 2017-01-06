using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class ImageModel : ServiceCallModel
    {
        public string UserId { get; set; }

        public string URI { get; set; }

        public string FileName { get; set; }

        public decimal FileSize { get; set; }

        public string MimeType { get; set; }
    }
}
