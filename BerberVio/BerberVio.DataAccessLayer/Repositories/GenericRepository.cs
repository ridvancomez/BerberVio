using System.Collections.Generic;
using System.Linq;
using BerberVio.DataAccessLayer.Context;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BerberVio.DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            var idValue = _context.Entry(entity).Property("Id").CurrentValue;
            var tracked = _context.Set<T>().Local
                .FirstOrDefault(e => !ReferenceEquals(e, entity)
                    && Equals(_context.Entry(e).Property("Id").CurrentValue, idValue));

            if (tracked != null)
            {
                _context.Entry(tracked).State = EntityState.Detached;
            }

            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
