using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ClinicService.IdentityServer.ViewModels
{
    public class PaymentMethodRequestModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public int? SortOrder { get; set; }

        public IEnumerable<IFormFile> Attachments { get; set; }
    }
}
