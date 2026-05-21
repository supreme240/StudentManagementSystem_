using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Services
{
    public class ChildStudentService : IStudentChild
    {
        public Student GetChildStudentInformation()
        {
            Student s1 = new Student();
            s1.Id = 1;
            s1.Name = "raj";
            s1.Age = 21;
            return s1;
        }
    }
}

