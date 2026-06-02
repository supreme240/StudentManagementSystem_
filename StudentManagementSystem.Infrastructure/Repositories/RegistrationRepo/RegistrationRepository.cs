using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;

namespace StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo {
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RegistrationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Registration?> CheckAuthenticationAsync(string email, string password)
            => await _dbContext.Registrations
                .FirstOrDefaultAsync(x =>
                    x.Email == email &&
                    x.Password == password);

        public async Task<Registration?> FindByEmailAndPhoneAsync(string email, string number)
        {
            return await _dbContext.Registrations
                .FirstOrDefaultAsync(x => x.Email == email && x.PhoneNumber == number);
        }

        public async Task<List<Registration>> GetByStudentCourseAsync(string course)
            => await _dbContext.Set<Registration>()
                .Where(r => r.Course == course)
                .ToListAsync();
    }
}



