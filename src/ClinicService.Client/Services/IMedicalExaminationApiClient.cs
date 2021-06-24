using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IMedicalExaminationApiClient
    {
        Task<IEnumerable<MedicalExaminationViewModel>> GetAllByPatientId(string patientId);

        Task<MedicalExaminationViewModel> GetById(int id);
    }
}
