using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicService.Client.ViewComponents
{
    public class HomeDSKViewComponent : ViewComponent
    {
        private readonly IMedicalServiceApiClient _medicalServiceApiClient;

        public HomeDSKViewComponent(IMedicalServiceApiClient medicalServiceApiClient)
        {
            _medicalServiceApiClient = medicalServiceApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _medicalServiceApiClient.GetAll());
        }
    }
}
