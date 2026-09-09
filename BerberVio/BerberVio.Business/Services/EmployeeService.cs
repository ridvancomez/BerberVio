using System.Collections.Generic;
using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.Business.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {
        public EmployeeService(IGenericRepository<Employee> dal) : base(dal)
        {
        }
    }
}
