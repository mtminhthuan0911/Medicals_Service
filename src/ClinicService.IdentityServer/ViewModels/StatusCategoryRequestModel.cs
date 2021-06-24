using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class StatusCategoryRequestModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }
    }
}
