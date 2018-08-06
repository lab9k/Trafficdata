using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace Database_Access_Object
{
    public interface IQueryManager<T>
    {

        List<T> GetAllResults(string Query, FeedOptions options);
        List<T> GetAll(int? limit = null);
    }
}
