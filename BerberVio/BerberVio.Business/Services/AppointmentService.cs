using System.Collections.Generic;
using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.Business.Services
{
    public class AppointmentService : GenericService<Appointment>, IAppointmentService
    {
        public AppointmentService(IGenericRepository<Appointment> dal) : base(dal)
        {
        }
    }
}
