using System;
using System.Collections.Generic;

namespace ClinicService.Client.Models
{
    public class NavigationViewModel
    {
        public WebsiteSectionViewModel Item { get; set; }

        public IEnumerable<NavigationViewModel> Children { get; set; }
    }
}
