using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ClinicService.Client.ViewComponents
{
    public class LayoutFooterViewComponent : ViewComponent
    {
        public LayoutFooterViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
