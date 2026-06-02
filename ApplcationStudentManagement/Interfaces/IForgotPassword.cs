using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IForgotPassword
    {
        int? ValidateUser(string email, long phoneNumber);
        bool ResetPassword(int userId, string newPassword);
    }
}