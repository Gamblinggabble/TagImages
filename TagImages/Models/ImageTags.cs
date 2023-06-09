using System.Diagnostics.CodeAnalysis;

namespace TagImages.Models
{
	public class ImageTags
	{
		public int ImageId { get; set; }
		public int TagId { get; set; }
		public Image Image { get; set; }
		public Tag Tag { get; set; }
	}
}
