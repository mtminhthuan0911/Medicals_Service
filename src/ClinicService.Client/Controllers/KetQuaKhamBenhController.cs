using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClinicService.Client.Controllers
{
    public class KetQuaKhamBenhController : Controller
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IReappointmentApiClient _reappointmentApiClient;
        private readonly IMedicalExaminationApiClient _medicalExaminationApiClient;

        private string _patientId = "";

        public KetQuaKhamBenhController(IAppointmentApiClient appointmentApiClient, IMedicalExaminationApiClient medicalExaminationApiClient, IReappointmentApiClient reappointmentApiClient)
        {
            _appointmentApiClient = appointmentApiClient;
            _reappointmentApiClient = reappointmentApiClient;
            _medicalExaminationApiClient = medicalExaminationApiClient;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.HasClaim(s => s.Type == ClaimTypes.NameIdentifier))
                _patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return View(await _medicalExaminationApiClient.GetAllByPatientId(_patientId));
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            if (HttpContext.User.HasClaim(s => s.Type == ClaimTypes.NameIdentifier))
                _patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var viewModel = await _medicalExaminationApiClient.GetById(id);

            if (viewModel.PatientId != _patientId)
            {
                ViewBag.AccessDeniedMessage = "Bạn không được phép xem thông tin này.";
                return View();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> LichKham()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                _patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            ViewBag.PatientId = _patientId;

            return View(await _appointmentApiClient.GetAllByPatientId(_patientId));
        }

        public async Task<IActionResult> LichTaiKham()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                _patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            ViewBag.PatientId = _patientId;

            return View(await _reappointmentApiClient.GetAllByPatientId(_patientId));
        }
    }
}
