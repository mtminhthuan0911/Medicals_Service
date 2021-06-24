using System;
using System.Collections.Generic;

namespace ClinicService.Client.Models
{
    public class MedicalServiceViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public int Cost { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public IEnumerable<MedicalServiceAttachmentViewModel> Attachments { get; set; }
    }
}
