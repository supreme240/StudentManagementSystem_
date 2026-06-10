using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ApplicationStudentManagement.Services
{
    public class LogInService : ILogIn
    {
        private readonly ApplicationDbContext _context;

        public LogInService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> ValidateUserAsync(string userNameOrEmail, string password)
        {
            return await _context.Registrations
                .FirstOrDefaultAsync(u =>
                    (u.UserName == userNameOrEmail || u.Email == userNameOrEmail)
                    && u.Password == password);
        }
    }
}