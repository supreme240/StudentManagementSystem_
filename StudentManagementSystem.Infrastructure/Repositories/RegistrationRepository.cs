using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repository;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
        }

        public async Task<List<Registration>> GetAllAsync()
        {
            return await _context.Registrations.ToListAsync();
        }

        public async Task<Registration?> GetByIdAsync(int id)
        {
            return await _context.Registrations.FindAsync(id);
        }

        public async Task UpdateAsync(Registration registration)
        {
            _context.Registrations.Update(registration);
        }

        public async Task DeleteAsync(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration != null)
            {
                _context.Registrations.Remove(registration);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}//