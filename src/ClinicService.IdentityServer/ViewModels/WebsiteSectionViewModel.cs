using System;
using System.Collections.Generic;
using ClinicService.IdentityServer.Data.Entities;

namespace ClinicService.IdentityServer.ViewModels
{
    public class WebsiteSectionViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string SeoAlias { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public IEnumerable<WebsiteSectionAttachment> Attachments { get; set; }
    }
}
