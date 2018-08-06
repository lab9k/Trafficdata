using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public enum ApiStatus
    {
        Online,
        Offline,
        Error
    }
    public class ApiEndpoint
    {

        public string Name { get; set; }
        public string Url { get; set; }
        public bool? NeedsAuthentication { get; set; }
        public ApiStatus Status { get; set; }
    }
}
