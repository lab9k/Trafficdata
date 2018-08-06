using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database_Access_Object;
using DocumentModels.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/traveltimes")]
    public class TraveltimesController : Controller
    {
        public IConfiguration Configuration { get; }
        private QueryManager<GenericTraveltimeSegment> _queryManager;

        public TraveltimesController(IQueryManager<GenericTraveltimeSegment> queryManager, IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            _queryManager = (QueryManager<GenericTraveltimeSegment>) queryManager;
            if (_queryManager.Client == null)
            {
                _queryManager.DatabaseUri = Configuration["database:endpoint"];
                _queryManager.DatabaseKey = Configuration["database:key"];
                _queryManager.DatabaseName = Configuration["database:name"];
                _queryManager.CollectionName = Configuration["database:collections:genericOutput"];
                _queryManager.Init();
            }

        }
        [Route("")]
        public IEnumerable<GenericTraveltimeSegment> Get()
        {
            
            return _queryManager.GetAll(100);
        }
        [Route("query")]
        public IEnumerable<GenericTraveltimeSegment> GetSegmentBetween([FromQuery]string segment, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (fromDate > toDate)
            {
                DateTime temp = fromDate;
                fromDate = toDate;
                toDate = temp;
            }

            var query = $"select TOP 10000 * from c where c.FromPoint = \"{segment}\" AND c.Timestamp.FullDate >= \"{fromDate.ToUniversalTime():O}\" AND c.Timestamp.FullDate < \"{toDate.ToUniversalTime():O}\" ";
            Console.WriteLine(query);
            return _queryManager.GetAllResults(query);
        }

        [Route("segments")]
        public string[] GetAllSegments()
        {

           QueryManager<TraveltimeSegments> queryManager = new QueryManager<TraveltimeSegments>();
            if (queryManager.Client == null)
            {
                queryManager.DatabaseUri = Configuration["database:endpoint"];
                queryManager.DatabaseKey = Configuration["database:key"];
                queryManager.DatabaseName = Configuration["database:name"];
                queryManager.CollectionName = Configuration["database:collections:genericOutput"];
                queryManager.Init();
            }


            return queryManager.Get("TravelTimeSegments").Result.Segments; //todo: change hardcoded string
        }

        [Route("status")]
        public Object Status()
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                List<GenericTraveltimeSegment> res = _queryManager.GetAll(1);

                return
                    $"{{ 'name': 'TraveltimesEndpoint', 'Status': {(res != null ? (ApiStatus.Online) : ApiStatus.Error)} }}";
            }
            catch (Exception ex)
            {
                return $"{{ 'name': 'TraveltimesEndpoint', 'Status': {ApiStatus.Error} }}";
            }
           
        }

    }
}