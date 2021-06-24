using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class UserRequestModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string DateOfBirth { get; set; }

        public string Avatar { get; set; }
    }
}
