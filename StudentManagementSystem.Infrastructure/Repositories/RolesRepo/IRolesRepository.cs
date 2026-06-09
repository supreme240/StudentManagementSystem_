using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem.Infrastructure.Repositories.RolesRepo {
    public interface IRolesRepository : IGenericRepository<Role>{
    }
}
