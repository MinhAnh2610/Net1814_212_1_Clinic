
using Clinic.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Data.Repository
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository()
        {

        }

        public CustomerRepository(Net1814_212_1_ClinicContext context) => _context = context;

    }
}
