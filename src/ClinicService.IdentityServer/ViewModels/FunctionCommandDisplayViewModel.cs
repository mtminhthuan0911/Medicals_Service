using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class FunctionCommandDisplayViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public int HasCreated { get; set; }

        public int HasUpdated { get; set; }

        public int HasDeleted { get; set; }

        public int HasViewed { get; set; }
    }
}
