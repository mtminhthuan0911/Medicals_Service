using System;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IUserApiClient
    {
        Task<UserViewModel> GetById(string id);
    }
}
