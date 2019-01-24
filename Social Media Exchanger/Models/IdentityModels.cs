using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social_Media_Exchanger.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            DailyBonusTaken = false;
            ClicksToday = 0;
            Points = 50;
        }


        public int Points { get; set; }

        [Range(0, Int32.MaxValue)]
        [Required]
        public int ClicksToday { get; set; }

        public bool DailyBonusTaken { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Refferal> Refferals { get; set; }
        public DbSet<FacebookPage> Facebooks { get; set; }
        public DbSet<FacebookLiked> FacebookLikes { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<TwitterPage> Twitters { get; set; }
        public DbSet<TwitterFollowers> Follows { get; set; }
        public DbSet<YoutubePage> Youtubes { get; set; }
        public DbSet<YoutubeSubscribtion> YtSubscribes { get; set; }
    }
}