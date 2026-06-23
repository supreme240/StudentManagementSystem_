using ApplicationStudentManagement.DTOs;
using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationStudentManagement.Interfaces {
    public interface IRoleService {
        Task AddRolesAsync(RoleViewModel roleViewModel);
        Task<List<RoleViewModel>> GetAllRolesAsync();
        Task<RoleViewModel?> GetRolesByIdAsync(int id);
        Task UpdateRolesAsync(RoleViewModel roleViewModel);
        Task DeleteRolesAsync(int id);
    }
}
