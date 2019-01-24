using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Social_Media_Exchanger.Models
{
    public class SocialPage
    {
        public int Id { get; set; }

        public SocialPage()
        {
            Cpc = 10;
            Active = true;
            NumberOfClicks = 0;
            Deleted = false;
        }


        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(350)]
        public virtual string Url { get; set; }

        [Range(2, 10)]
        public int Cpc { get; set; }

        [Range(0, Int32.MaxValue)]
        public int NumberOfClicks { get; set; }

        [Required]
        public bool Active { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public bool Deleted { get; set; }
    }
}
