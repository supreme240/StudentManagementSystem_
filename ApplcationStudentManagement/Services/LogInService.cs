using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Services
{
    public class LogInService : ILogIn
    {
        private readonly ApplicationDbContext _context;

        public LogInService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Registration? ValidateUser(string userNameOrEmail, string password)
        {
            return _context.Registrations
                .FirstOrDefault(u =>
                    (u.UserName == userNameOrEmail || u.Email == userNameOrEmail)
                    && u.Password == password);
        }
    }
}
