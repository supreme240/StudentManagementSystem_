using ApplicationStudentManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;

namespace ApplicationStudentManagement.Services
{
    public class RolesService : IRolesService
    {
        private readonly ApplicationDbContext _context;

        public RolesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Roles>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Roles?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(bool success, string? error)> AddRoleAsync(Roles role)
        {
            bool roleExists = await _context.Roles
                .AnyAsync(r => r.RoleName.ToLower() == role.RoleName.ToLower());

            if (roleExists)
                return (false, $"Role '{role.RoleName}' already exists.");

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task UpdateRoleAsync(Roles role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}