using Social_Media_Exchanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Social_Media_Exchanger.ViewModels
{
    public class PromoteViewModel
    {
        public string Username { get; set; }

        public List<Refferal> Refferals { get; set; }
    }
}