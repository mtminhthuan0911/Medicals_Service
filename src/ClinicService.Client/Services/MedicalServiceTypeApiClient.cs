using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class MedicalServiceTypeApiClient: BaseApiClient, IMedicalServiceTypeApiClient
    {
        public MedicalServiceTypeApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<MedicalServiceTypeViewModel>> GetAll()
        {
            return await GetListAsync<MedicalServiceTypeViewModel>("/api/medical-service-types");
        }
    }
}
