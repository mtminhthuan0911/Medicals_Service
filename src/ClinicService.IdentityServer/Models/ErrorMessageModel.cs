using System;
using Newtonsoft.Json;

namespace ClinicService.IdentityServer.Models
{
    public class ErrorMessageModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
