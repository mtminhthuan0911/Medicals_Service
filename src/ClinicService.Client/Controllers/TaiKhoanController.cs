using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.Client.Controllers
{
    [Authorize]
    public class TaiKhoanController : Controller
    {
        [HttpGet]
        public IActionResult DangNhap(string returnUrl)
        {
            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DangXuat(string returnUrl)
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
