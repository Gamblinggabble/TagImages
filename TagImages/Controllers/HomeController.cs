using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Net;
using TagImages.Data;
using TagImages.Models;

namespace TagImages.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private static DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
									.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-TagImages;Trusted_Connection=True;MultipleActiveResultSets=true")
									.Options;
		private ApplicationDbContext dbContext = new ApplicationDbContext(options);

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Index(string imageUrl)
		{
			using (dbContext)
			{
				bool exists = dbContext.Image.Any(image => image.url == imageUrl);
				if (!exists)
				{
					List<Tag> tags = GetTags(imageUrl);
					Console.WriteLine("Image was successfully uploaded for tagging");
					SaveImageToDb(imageUrl, tags);
					Console.WriteLine("Image was successfully saved to db");
				}
				else
				{
					Console.WriteLine("Image already exists in the db");
				}
			}

			return View();
		}

		public IActionResult Library()
		{
			List<Image> images = dbContext.Image.Include(i => i.Tags).ToList();
			foreach (Image image in images)
			{
				foreach (ImageTags item in image.Tags)
				{
                    Console.WriteLine(dbContext.Tag.ToList().Find(t => t.Id == item.TagId).Title);
                }
			}
			//List<Tag> fakeTags = new List<Tag>
			//{
			//	new Tag { Id = 1, Title = "kitty" },
			//	new Tag { Id = 2, Title = "baby" },
			//	new Tag { Id = 3, Title = "small" },
			//	new Tag { Id = 4, Title = "cat" },
			//	new Tag { Id = 5, Title = "chubby" },
			//	new Tag { Id = 6, Title = "cute" },
			//};
			//ViewBag.Tags = fakeTags;
			return View(images);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private List<Tag> GetTags(string imageUrl)
		{
			// TODO fill credentials
			string apiKey = "";
			string apiSecret = "";

			string basicAuthValue = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));

			var client = new RestClient("https://api.imagga.com/v2/tags");
			client.Timeout = -1;

			var request = new RestRequest(Method.GET);
			request.AddParameter("image_url", imageUrl);
			request.AddHeader("Authorization", String.Format("Basic {0}", basicAuthValue));

			IRestResponse response = client.Execute(request);
			Console.WriteLine(response.Content);

			List<Tag> tags = new List<Tag>();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				JObject jsonObject = JObject.Parse(response.Content);

				// Find first 6 tags
				JArray resultsArray = (JArray)jsonObject["result"]["tags"];
				foreach (JToken tagToken in resultsArray.Take(6))
				{
					Console.WriteLine(tagToken["tag"]["en"]);
					tags.Add(new Tag { Title = (string)tagToken["tag"]["en"] });
				}
			}
			else
			{
                Console.WriteLine("Image with url" + imageUrl + "cannot be processed");
            }

			return tags.IsNullOrEmpty() ? null : tags;

		}

		private void SaveImageToDb(string imageUrl, List<Tag> tags)
		{
			Image image = new Image
			{
				url = imageUrl
			};
			dbContext.Image.Add(image);
			dbContext.SaveChanges();
			tags.ForEach(tag =>
			{
				if (!dbContext.Tag.Any(t => t.Title == tag.Title))
				{
					dbContext.Tag.Add(tag);
				}

                dbContext.ImageTags.Add(new ImageTags { ImageId = image.Id, TagId = tag.Id, Image = image, Tag = tag });
            });
			dbContext.SaveChanges();
		}
	}
}