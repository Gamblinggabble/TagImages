namespace TagImages.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string? url { get; set; }
        public ICollection<ImageTags> Tags { get; set; }
    }
}
