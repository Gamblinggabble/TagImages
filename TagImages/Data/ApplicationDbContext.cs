using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TagImages.Data.Migrations;
using TagImages.Models;

namespace TagImages.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		public DbSet<Image> Image { get; set; } = default!;

		public DbSet<Tag> Tag { get; set; } = default!;

		public DbSet<ImageTags> ImageTags { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ImageTags>()
                .HasKey(it => new { it.ImageId, it.TagId });

            builder.Entity<Image>()
                .HasMany<ImageTags>(i => i.Tags);

            builder.Entity<Tag>()
                .HasMany<ImageTags>(t => t.Images);

            builder.Entity<ImageTags>()
                .HasOne(it => it.Image)
                .WithMany(i => i.Tags)
                .HasForeignKey(it => it.ImageId);

            builder.Entity<ImageTags>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.Images)
                .HasForeignKey(it => it.TagId);
        }

	}

}
