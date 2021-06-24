using ClinicService.Client.Constants;
using ClinicService.Client.Models;
using ClinicService.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicService.Client.Controllers
{
    [Authorize]
    public class DatLichHenController : Controller
    {
        private const string APPOINTMENT_FULL_FORM_KEY = "AppointmentFullForm";
        private readonly IMedicalServiceApiClient _medicalServiceApiClient;
        private readonly IClinicBranchApiClient _clinicBranchApiClient;
        private readonly IPaymentMethodApiClient _paymentMethodApiClient;
        private readonly IAppointmentApiClient _appointmentApiClient;

        public DatLichHenController(IMedicalServiceApiClient medicalServiceApiClient, IClinicBranchApiClient clinicBranchApiClient, IAppointmentApiClient appointmentApiClient, IPaymentMethodApiClient paymentMethodApiClient)
        {
            _medicalServiceApiClient = medicalServiceApiClient;
            _clinicBranchApiClient = clinicBranchApiClient;
            _appointmentApiClient = appointmentApiClient;
            _paymentMethodApiClient = paymentMethodApiClient;
        }

        public async Task<IActionResult> Index(int medicalServiceId)
        {
            ViewBag.MedicalService = await _medicalServiceApiClient.GetById(medicalServiceId);
            return View(new AppointmentRequestModel() { MedicalServiceId = medicalServiceId });
        }

        [HttpPost]
        public IActionResult Index(AppointmentRequestModel requestModel)
        {
            var sessionAppointment = JsonConvert.SerializeObject(requestModel);
            HttpContext.Session.SetString(APPOINTMENT_FULL_FORM_KEY, sessionAppointment);

            return RedirectToAction("ChonPhongKham", new { medicalServiceId = requestModel.MedicalServiceId });
        }

        public async Task<IActionResult> ChonPhongKham(int medicalServiceId)
        {
            ViewBag.MedicalService = await _medicalServiceApiClient.GetById(medicalServiceId);
            ViewBag.ClinicBranches = await _clinicBranchApiClient.GetAll();

            return View();
        }

        [HttpPost]
        public IActionResult PostChonPhongKham(int clinicBranchId)
        {
            var appointmentFromSession = JsonConvert.DeserializeObject<AppointmentRequestModel>(HttpContext.Session.GetString(APPOINTMENT_FULL_FORM_KEY));
            appointmentFromSession.ClinicBranchId = clinicBranchId;

            var sessionAppointment = JsonConvert.SerializeObject(appointmentFromSession);
            HttpContext.Session.SetString(APPOINTMENT_FULL_FORM_KEY, sessionAppointment);

            return RedirectToAction("Checkout", new { medicalServiceId = appointmentFromSession.MedicalServiceId });
        }

        public async Task<IActionResult> Checkout(int medicalServiceId)
        {
            ViewBag.MedicalService = await _medicalServiceApiClient.GetById(medicalServiceId);
            ViewBag.PaymentMethods = await _paymentMethodApiClient.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string paymentMethodId)
        {
            var appointmentFromSession = JsonConvert.DeserializeObject<AppointmentRequestModel>(HttpContext.Session.GetString(APPOINTMENT_FULL_FORM_KEY));

            var requestModel = new AppointmentPaymentRequestModel
            {
                AppointmentDate = DateTime.Parse(appointmentFromSession.AppointmentDate),
                ClinicBranchId = appointmentFromSession.ClinicBranchId,
                MedicalServiceId = appointmentFromSession.MedicalServiceId,
                Note = appointmentFromSession.Note,
                PatientFullName = appointmentFromSession.PatientFullName,
                PatientId = appointmentFromSession.PatientId,
                PatientPhoneNumber = appointmentFromSession.PatientPhoneNumber,
                PaymentMethodId = paymentMethodId,
                PaymentStatusCategoryId = SystemsConstant.StatusCategory.STA_CATE_PAYMENT_UNPAID,
                StatusCategoryId = SystemsConstant.StatusCategory.STA_CATE_GENERAL_RECEIVED
            };

            var result = await _appointmentApiClient.PostAppointmentPayment(requestModel);
            if(result)
            {
                return RedirectToAction("ThanhToanThanhCong");
            }

            TempData["ErrorMessage"] = MessagesConstant.DEFAULT_BAD_REQUEST;
            return RedirectToAction("Checkout", new { medicalServiceId = appointmentFromSession.MedicalServiceId });
        }

        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }
    }
}
