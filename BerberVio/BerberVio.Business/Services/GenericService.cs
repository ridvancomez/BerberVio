using System.Collections.Generic;
using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.Business.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _genericRepository;

        public GenericService(IGenericRepository<T> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public List<T> GetAll()
        {
            return _genericRepository.GetAll();
        }

        public T? GetById(int id)
        {
            return _genericRepository.GetById(id);
        }

        public void Add(T entity)
        {
            _genericRepository.Add(entity);
        }

        public void Update(T entity)
        {
            _genericRepository.Update(entity);
        }

        public void Delete(int id)
        {
            _genericRepository.Delete(id);
        }
    }
}
