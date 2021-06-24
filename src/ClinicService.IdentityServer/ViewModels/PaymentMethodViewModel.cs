using System;
using System.Collections.Generic;
using ClinicService.IdentityServer.Data.Entities;

namespace ClinicService.IdentityServer.ViewModels
{
    public class PaymentMethodViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public int? SortOrder { get; set; }

        public IEnumerable<PaymentMethodAttachment> Attachments { get; set; }
    }
}
