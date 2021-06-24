using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicService.Client.ViewComponents
{
    public class PatientInfoViewComponent : ViewComponent
    {
        private readonly IUserApiClient _userApiClient;

        public PatientInfoViewComponent(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null ? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value : null;
            var viewModel = new UserViewModel();
            if (!string.IsNullOrEmpty(userId))
                viewModel = await _userApiClient.GetById(userId);

            return View(viewModel);
        }
    }
}
