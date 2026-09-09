using System.Collections.Generic;
using System.Linq;
using BerberVio.DataAccessLayer.Context;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.DataAccessLayer.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
