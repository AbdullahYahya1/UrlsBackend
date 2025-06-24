using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlsBackend.Data.dtos
{
    public class PostUrlDto
    {
        public string newUrl { get; set; }
        public string oldUrl { get; set; }
        public int? MaxClicks { get; set; }
        public string? Password { get; set; }
        public DateTime? ExpairDate { get; set; }
    }
}
