using System;
using AutoMapper;
using ClinicService.IdentityServer.Data.Entities;
using ClinicService.IdentityServer.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ClinicService.IdentityServer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityRole, RoleViewModel>();
            CreateMap<RoleRequestModel, IdentityRole>();

            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserRequestModel, ApplicationUser>();

            CreateMap<Command, CommandViewModel>();
            CreateMap<CommandRequestModel, Command>();

            CreateMap<Function, FunctionViewModel>();
            CreateMap<FunctionRequestModel, Function>();

            CreateMap<FunctionCommand, FunctionCommandViewModel>();

            CreateMap<WebsiteSection, WebsiteSectionViewModel>();
            CreateMap<WebsiteSectionRequestModel, WebsiteSection>();

            CreateMap<Specialty, SpecialtyViewModel>();
            CreateMap<SpecialtyRequestModel, Specialty>();

            CreateMap<StatusCategory, StatusCategoryViewModel>();
            CreateMap<StatusCategoryRequestModel, StatusCategory>();

            CreateMap<Permission, PermissionViewModel>();

            CreateMap<AppointmentRequestModel, Appointment>();

            CreateMap<AppointmentPaymentRequestModel, Appointment>();

            CreateMap<ReappointmentRequestModel,Reappointment>();

            CreateMap<MedicalExamination, MedicalExaminationViewModel>();
            CreateMap<MedicalExaminationRequestModel, MedicalExamination>();

            CreateMap<MedicalExaminationDetail, MedicalExaminationDetailViewModel>();
            CreateMap<MedicalExaminationDetailRequestModel, MedicalExaminationDetail>();

            CreateMap<MedicalExaminationFullRequestModel, MedicalExamination>();

            CreateMap<Prescription, PrescriptionViewModel>();                   
            CreateMap<PrescriptionRequestModel, Prescription>();

            CreateMap<PaymentMethod, PaymentMethodViewModel>();
            CreateMap<PaymentMethodRequestModel, PaymentMethod>();

            CreateMap<Payment, PaymentViewModel>();
            CreateMap<PaymentRequestModel, Payment>();

            CreateMap<ClinicBranch, ClinicBranchViewModel>();
            CreateMap<ClinicBranchRequestModel, ClinicBranch>();

            CreateMap<MedicalService, MedicalServiceViewModel>();
            CreateMap<MedicalServiceRequestModel, MedicalService>();

            CreateMap<MedicalServiceType, MedicalServiceTypeViewModel>();
            CreateMap<MedicalServiceTypeRequestModel, MedicalServiceType>();
        }
    }
}
