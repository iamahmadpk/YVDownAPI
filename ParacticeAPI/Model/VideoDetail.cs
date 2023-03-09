using System.ComponentModel.DataAnnotations.Schema;

namespace ParacticeAPI.Model
{
    public class VideoDetail
    {
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int RequestId { get; set; }
        public string VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string? Status { get; set; }
    }
}
