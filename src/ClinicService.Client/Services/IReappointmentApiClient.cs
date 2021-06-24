using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IReappointmentApiClient
    {
        Task<IEnumerable<ReappointmentViewModel>> GetAllByPatientId(string patientId);
    }
}
