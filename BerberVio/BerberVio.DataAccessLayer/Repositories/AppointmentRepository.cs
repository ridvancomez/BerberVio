using System.Collections.Generic;
using System.Linq;
using BerberVio.DataAccessLayer.Context;
using BerberVio.Entities;
using BerberVio.DataAccessLayer.Interfaces;

namespace BerberVio.DataAccessLayer.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
