using System;
using System.Collections.Generic;

namespace ClinicService.IdentityServer.ViewModels
{
    public class Pagination<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalRecords { get; set; }
    }
}
