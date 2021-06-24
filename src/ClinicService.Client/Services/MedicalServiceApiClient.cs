using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class MedicalServiceApiClient : BaseApiClient, IMedicalServiceApiClient
    {
        public MedicalServiceApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<MedicalServiceViewModel>> GetAll()
        {
            return await GetListAsync<MedicalServiceViewModel>("/api/medical-services");
        }

        public async Task<IEnumerable<MedicalServiceViewModel>> GetAllByTypeId(string medicalServiceTypeId)
        {
            return await GetListAsync<MedicalServiceViewModel>($"/api/medical-services/types/{medicalServiceTypeId}");
        }

        public async Task<MedicalServiceViewModel> GetById(int id)
        {
            return await GetAsync<MedicalServiceViewModel>("/api/medical-services/" + id);
        }
    }
}
