using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicService.Client.Services
{
    public class AppointmentApiClient : BaseApiClient, IAppointmentApiClient
    {
        private readonly ILogger<AppointmentApiClient> _logger;

        public AppointmentApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AppointmentApiClient> logger)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetAllByPatientId(string patientId)
        {
            return await GetListAsync<AppointmentViewModel>("/api/appointments/patients/" + patientId, true);
        }

        public async Task<bool> PostAppointmentPayment(AppointmentPaymentRequestModel requestModel)
        {
            try
            {
                await PostAsync<AppointmentPaymentRequestModel, AppointmentPaymentRequestModel>("/api/appointments/payments", requestModel);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
