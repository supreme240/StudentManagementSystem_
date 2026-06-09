using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.RolesRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationStudentManagement.Services {
    public class RoleService : IRoleService {
        private readonly IRolesRepository _rolesRepository;

        public RoleService(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }
        public async Task AddRolesAsync(Role role)
        {
            if(role == null)
                throw new ArgumentNullException(nameof(role));

            await _rolesRepository.AddAsync(role);
            await _rolesRepository.SaveChangesAsync();
        }

        public async Task DeleteRolesAsync(int id)
        {
            await _rolesRepository.DeleteAsync(id);
            await _rolesRepository.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _rolesRepository.GetAllAsync();
        }

        public async Task<Role?> GetRolesByIdAsync(int id)
        {
            return await _rolesRepository.GetByIdAsync(id);
        }

        public async Task UpdateRolesAsync(Role role)
        {
            _rolesRepository.Update(role);
            await _rolesRepository.SaveChangesAsync();
        }
    }
}
