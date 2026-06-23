using ApplicationStudentManagement.DTOs;
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
        public async Task AddRolesAsync(RoleViewModel roleViewModel)
        {
            try
            {
                if (roleViewModel == null)
                    throw new ArgumentNullException(nameof(roleViewModel));

                var role = new Role {
                    Id = roleViewModel.Id,
                    EachRole = roleViewModel.EachRole
                };
                
                await _rolesRepository.AddAsync(role);
                await _rolesRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while adding roles.", ex);
            }
        }

        public async Task DeleteRolesAsync(int id)
        {
            try
            {
                await _rolesRepository.DeleteAsync(id);
                await _rolesRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while deleting roles with ID {id}.", ex);
            }
        }

        public async Task<List<RoleViewModel>> GetAllRolesAsync()
        {
            try
            {
                var role = await _rolesRepository.GetAllAsync();

                if (role == null) return null;

                return role.Select(r => new RoleViewModel 
                { 
                    Id = r.Id,
                    EachRole = r.EachRole,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetching all roles.", ex);
            }
        }

        public async Task<RoleViewModel?> GetRolesByIdAsync(int id)
        {
            try
            {
                var role = await _rolesRepository.GetByIdAsync(id);

                if (role == null) return null;

                return new RoleViewModel
                {
                    Id = role.Id,
                    EachRole = role.EachRole,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetching roles with ID {id}.", ex);
            }
        }

        public async Task UpdateRolesAsync(RoleViewModel roleViewModel)
        {
            try
            {
                if (roleViewModel == null)
                    throw new ArgumentNullException(nameof(roleViewModel));

                var role = new Role {
                    Id = roleViewModel.Id,
                    EachRole = roleViewModel.EachRole
                };
                
                _rolesRepository.Update(role);
                await _rolesRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while updating roles.", ex);
            }
        }
    }
}
