using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IMedicalServiceApiClient
    {
        Task<IEnumerable<MedicalServiceViewModel>> GetAll();
        Task<IEnumerable<MedicalServiceViewModel>> GetAllByTypeId(string medicalServiceTypeId);
        Task<MedicalServiceViewModel> GetById(int id);
    }
}
