using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Database_Access_Object
{
    public interface ICRUDManager<T>
    {
        void Init();
        Task Create(T obj, int tryCount, int maxTries);
        Task<T> Get(string id);
        Task<T> Update (string id,T obj);
        Task Delete(string id);
    }
}
