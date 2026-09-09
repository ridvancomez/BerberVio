using System.Collections.Generic;
using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.Business.Services
{
    public class ServiceService : GenericService<Service>, IServiceService
    {
        public ServiceService(IGenericRepository<Service> dal) : base(dal)
        {
        }
    }
}
