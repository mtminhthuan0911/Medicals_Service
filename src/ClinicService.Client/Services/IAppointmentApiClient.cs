using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IAppointmentApiClient
    {
        Task<IEnumerable<AppointmentViewModel>> GetAllByPatientId(string patientId);

        Task<bool> PostAppointmentPayment(AppointmentPaymentRequestModel requestModel);
    }
}
