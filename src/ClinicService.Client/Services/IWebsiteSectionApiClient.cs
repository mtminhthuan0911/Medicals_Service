using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Models;

namespace ClinicService.Client.Services
{
    public interface IWebsiteSectionApiClient
    {
        Task<WebsiteSectionViewModel> GetById(string id);

        Task<IEnumerable<WebsiteSectionViewModel>> GetChildrenByParentId(string parentId);
    }
}
