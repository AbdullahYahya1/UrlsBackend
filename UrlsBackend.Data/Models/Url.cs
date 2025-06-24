using System.ComponentModel.DataAnnotations;
namespace UrlsBackend.Data.Models
{
    public class Url
    {
        [Key]
        public int UrlId { get; set; }
        public string newUrl { get; set; }
        public string oldUrl { get; set; }
        public int? MaxClicks { get; set; }
        public int ClicksCounter { get; set; } = 0; 
        public string? Password { get; set; }
        public DateTime? ExpairDate { get; set; }
        public int? UserID { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
    }
}
