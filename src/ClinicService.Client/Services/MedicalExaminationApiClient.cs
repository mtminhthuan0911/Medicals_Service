using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class MedicalExaminationApiClient : BaseApiClient, IMedicalExaminationApiClient
    {
        public MedicalExaminationApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<MedicalExaminationViewModel>> GetAllByPatientId(string patientId)
        {
            return await GetListAsync<MedicalExaminationViewModel>("/api/medical-examinations/patients/" + patientId, true);
        }

        public async Task<MedicalExaminationViewModel> GetById(int id)
        {
            return await GetAsync<MedicalExaminationViewModel>("/api/medical-examinations/" + id, true);
        }
    }
}
