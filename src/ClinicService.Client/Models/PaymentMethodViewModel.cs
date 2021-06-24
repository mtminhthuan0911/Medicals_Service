using System;
using System.Collections.Generic;

namespace ClinicService.Client.Models
{
    public class PaymentMethodViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public int? SortOrder { get; set; }

        public IEnumerable<PaymentMethodAttachmentViewModel> Attachments { get; set; }
    }
}
