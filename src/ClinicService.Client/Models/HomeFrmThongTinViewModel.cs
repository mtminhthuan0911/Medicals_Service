using System;
using System.Collections.Generic;

namespace ClinicService.Client.Models
{
    public class HomeFrmThongTinViewModel
    {
        public WebsiteSectionViewModel Parent { get; set; }

        public IEnumerable<WebsiteSectionViewModel> Children { get; set; }

        public IEnumerable<ClinicBranchViewModel> ClinicBranches { get; set; }
    }
}
