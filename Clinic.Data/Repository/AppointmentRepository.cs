using Clinic.Data.Models;
namespace Clinic.Data.Repository
{
    public class AppointmentRepository : GenericRepository<Appointment>
    {
        public AppointmentRepository() { }
        public AppointmentRepository(Net1814_212_1_ClinicContext context) => _context = context;
    }
    
}
