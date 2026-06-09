using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationStudentManagement.Interfaces {
    public interface IRoleService {
        Task AddRolesAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRolesByIdAsync(int id);
        Task UpdateRolesAsync(Role role);
        Task DeleteRolesAsync(int id);
    }
}
