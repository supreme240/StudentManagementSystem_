using ApplicationStudentManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;

namespace ApplicationStudentManagement.Services
{
    public class ForgotPasswordService : IForgotPassword
    {
        private readonly ApplicationDbContext _db;

        public ForgotPasswordService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int?> ValidateUserAsync(string email, long phoneNumber)
        {
            var user = await _db.Registrations
                          .FirstOrDefaultAsync(r => r.Email == email && r.PhoneNumber == phoneNumber);

            return user?.Id;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _db.Registrations.FindAsync(userId);
            if (user == null)
                return false;

            user.Password = newPassword;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}