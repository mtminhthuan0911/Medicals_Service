using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class FunctionViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }
    }
}
