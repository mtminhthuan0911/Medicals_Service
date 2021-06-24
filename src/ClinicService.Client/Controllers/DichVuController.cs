using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ClinicService.Client.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using ClinicService.Client.Services;
using ClinicService.Client.Constants;

namespace ClinicService.Client.Controllers
{
    public class DichVuController : Controller
    {
        private readonly IMedicalServiceApiClient _medicalServiceApiClient;
        private readonly IMedicalServiceTypeApiClient _medicalServiceTypeApiClient;

        public DichVuController(IMedicalServiceApiClient medicalServiceApiClient, IMedicalServiceTypeApiClient medicalServiceTypeApiClient)
        {
            _medicalServiceApiClient = medicalServiceApiClient;
            _medicalServiceTypeApiClient = medicalServiceTypeApiClient;
        }

        public async Task<IActionResult>  Index(string medicalServiceTypeId = SystemsConstant.MedicalServiceType.TYPE_PHOBIEN)
        {
            ViewBag.MedicalServiceTypes = await _medicalServiceTypeApiClient.GetAll();
            return View(await _medicalServiceApiClient.GetAllByTypeId(medicalServiceTypeId));
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            return View(await _medicalServiceApiClient.GetById(id));
        }
    }
}
