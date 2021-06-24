using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ClinicService.IdentityServer.ViewModels
{
    public class MedicalServiceRequestModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Cost { get; set; }

        public IEnumerable<IFormFile> Attachments { get; set; }

        public string MedicalServiceTypeId { get; set; }
    }
}
