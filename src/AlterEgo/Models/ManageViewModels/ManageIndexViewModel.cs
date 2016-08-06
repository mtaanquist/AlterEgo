using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlterEgo.Models.ManageViewModels
{
    public class ManageIndexViewModel
    {
        [Display(Name = "Local Time Zone")]
        public string LocalTimeZoneInfoId { get; set; }
        public List<SelectListItem> TimeZones { get; set; }

        public List<Character> Characters { get; set; }
        public bool HasPassword { get; set; }

        public DateTime UserAccessTokenExpiry { get; set; }
    }
}
