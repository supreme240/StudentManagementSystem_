using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Interfaces
{
    public interface ILogIn
    {
          Task<Registration?> ValidateUserAsync(string userNameOrEmail, string password);
    }
}