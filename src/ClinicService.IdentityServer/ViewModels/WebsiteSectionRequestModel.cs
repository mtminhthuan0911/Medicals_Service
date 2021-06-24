using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ClinicService.IdentityServer.ViewModels
{
    public class WebsiteSectionRequestModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string SeoAlias { get; set; }

        public IEnumerable<IFormFile> Attachments { get; set; }
    }
}
