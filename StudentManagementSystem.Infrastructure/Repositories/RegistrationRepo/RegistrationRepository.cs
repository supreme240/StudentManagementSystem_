using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;

namespace StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo {
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<Registration>> GetByStudentCourseAsync(string course)
            => await _context.Set<Registration>()
                .Where(r => r.Course == course)
                .ToListAsync();
    }
}