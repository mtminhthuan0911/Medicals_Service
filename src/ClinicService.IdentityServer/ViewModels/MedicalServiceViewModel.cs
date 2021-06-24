using System;
using System.Collections.Generic;
using ClinicService.IdentityServer.Data.Entities;

namespace ClinicService.IdentityServer.ViewModels
{
    public class MedicalServiceViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Cost { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string MedicalServiceTypeId { get; set; }

        public IEnumerable<MedicalServiceAttachment> Attachments { get; set; }
    }
}
