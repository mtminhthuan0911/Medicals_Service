using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class PaymentMethodApiClient : BaseApiClient, IPaymentMethodApiClient
    {
        public PaymentMethodApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<PaymentMethodViewModel>> GetAll()
        {
            return await GetListAsync<PaymentMethodViewModel>("/api/payment-methods"); 
        }
    }
}
