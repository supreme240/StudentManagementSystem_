using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Services
{
    public class ForgotPasswordService : IForgotPassword
    {
        private readonly ApplicationDbContext _db;

        public ForgotPasswordService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Checks email + phone against Registrations table
        public int? ValidateUser(string email, long phoneNumber)
        {
            var user = _db.Registrations
                          .FirstOrDefault(r => r.Email == email && r.PhoneNumber == phoneNumber);

            return user != null ? (int?)user.Id : null;
        }

        //saves the new password
        public bool ResetPassword(int userId, string newPassword)
        {
            var user = _db.Registrations.Find(userId);
            if (user == null)
                return false;

            user.Password = newPassword;
            _db.SaveChanges();
            return true;
        }
    }
}