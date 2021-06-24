using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class ReappointmentApiClient : BaseApiClient, IReappointmentApiClient
    {
        public ReappointmentApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<ReappointmentViewModel>> GetAllByPatientId(string patientId)
        {
            return await GetListAsync<ReappointmentViewModel>("/api/re-appointments/patients/" + patientId, true);
        }
    }
}
