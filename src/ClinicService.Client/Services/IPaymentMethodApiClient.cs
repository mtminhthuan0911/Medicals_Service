using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IPaymentMethodApiClient
    {
        Task<IEnumerable<PaymentMethodViewModel>> GetAll();
    }
}
