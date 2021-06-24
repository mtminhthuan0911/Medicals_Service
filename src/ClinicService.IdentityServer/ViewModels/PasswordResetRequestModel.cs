using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicService.IdentityServer.ViewModels
{
    public class PasswordResetRequestModel
    {

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu nhập lại chưa chính xác.")]
        public string ConfirmedPassword { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }
    }
}
