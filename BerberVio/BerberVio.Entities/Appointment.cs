using System;

namespace BerberVio.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
