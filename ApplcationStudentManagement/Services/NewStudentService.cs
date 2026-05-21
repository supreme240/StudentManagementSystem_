using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using System.Security.Cryptography;

namespace ApplicationStudentManagement.Services
{
    public class NewStudentService : IStudentInterface
    {
        public Student GetNewStudentInformation()
        {
            Student s2=new Student();
            s2.Id = 1;
            s2.Name = "ram";
            s2.Age = 22;
            return s2;
        }

        public Student GetStudentInformation()
        {
            Student s2=new Student();
          s2.Id= 1;
            s2.Name= "ram";
            s2.Age= 22;
            return s2;
        }
    }
}
