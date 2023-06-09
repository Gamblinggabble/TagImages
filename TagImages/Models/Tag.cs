using System.Diagnostics.CodeAnalysis;

namespace TagImages.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [NotNull]
        public string Title { get; set; }
        public ICollection<ImageTags> Images { get; set; }
    }
}
