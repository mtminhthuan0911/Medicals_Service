using System;
using System.Threading.Tasks;

namespace ClinicService.IdentityServer.Services
{
    public interface IEmailSenderService
    {
        Task SendAsync(string clientEmailAddress, string subject, string body);
    }
}
