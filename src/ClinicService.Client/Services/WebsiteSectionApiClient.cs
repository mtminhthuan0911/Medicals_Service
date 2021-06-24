using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClinicService.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Client.Services
{
    public class WebsiteSectionApiClient : BaseApiClient, IWebsiteSectionApiClient
    {
        public WebsiteSectionApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<WebsiteSectionViewModel> GetById(string id)
        {
            return await GetAsync<WebsiteSectionViewModel>("/api/website-sections/" + id);
        }

        public async Task<IEnumerable<WebsiteSectionViewModel>> GetChildrenByParentId(string parentId)
        {
            return await GetListAsync<WebsiteSectionViewModel>("/api/website-sections/" + parentId + "/children");
        }
    }
}
