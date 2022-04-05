using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Server.Models;

namespace Server.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public SiteUser SiteUser { get; set; } 

        public int SiteUserId { get; set; }
    }
}