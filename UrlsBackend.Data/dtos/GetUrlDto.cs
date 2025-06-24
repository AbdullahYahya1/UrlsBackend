using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.Models;

namespace UrlsBackend.Data.dtos
{
    public class GetUrlDto
    {
        public int UrlId { get; set; }
        public string newUrl { get; set; }
        public string oldUrl { get; set; }
        public int? MaxClicks { get; set; }
        public int ClicksCounter { get; set; } = 0;
        public DateTime? ExpairDate { get; set; }
        public int? UserID { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
