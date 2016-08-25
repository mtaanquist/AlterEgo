using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlterEgo.Models.ManageViewModels
{
    public class ManageViewModel
    {
        [Display(Name = "Local Time Zone")]
        public string LocalTimeZoneInfoId { get; set; }

        public List<SelectListItem> TimeZones { get; set; }

        public List<Character> Characters { get; set; }

        public bool HasPassword { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime UserAccessTokenExpiry { get; set; }
    }
}
