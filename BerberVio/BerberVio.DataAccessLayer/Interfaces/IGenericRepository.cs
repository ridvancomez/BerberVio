using System.Collections.Generic;
using BerberVio.Entities;

namespace BerberVio.DataAccessLayer.Interfaces
{
public interface IGenericRepository<T> where T : class {
 List<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
}
}
