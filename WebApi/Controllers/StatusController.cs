using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public IEnumerable<ApiEndpoint> Get([FromQuery]string name)
        {
            if (name == null)
            {
                List<ApiEndpoint> endPoints = new List<ApiEndpoint>();
                endPoints.Add(new ApiEndpoint()
                {
                    Name = "TravelTimes",
                    NeedsAuthentication = true,
                    Url = "http://localhost:8080/api/traveltimes"
                });
                endPoints.Add(new ApiEndpoint()
                {
                    Name = "Database",
                    NeedsAuthentication = true,
                    Url = "http://localhost:8081"
                });
                return endPoints;
            }
            else
            {
                List<ApiEndpoint> endPoints = new List<ApiEndpoint>();
                endPoints.Add(new ApiEndpoint()
                {
                    Name = (name.ToLower() == "traveltimes" || name == "database") ? name : "Unknown",
                    Status = ApiStatus.Offline
                });
                return endPoints;
            }
            
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
