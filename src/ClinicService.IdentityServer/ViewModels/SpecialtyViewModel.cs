using System;
using System.Collections.Generic;
using ClinicService.IdentityServer.Data.Entities;

namespace ClinicService.IdentityServer.ViewModels
{
    public class SpecialtyViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string SeoAlias { get; set; }

        public IEnumerable<SpecialtyAttachment> Attachments { get; set; }
    }
}
