using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Services
{
    public class StudentService : IStudentInterface
    {
        private readonly IStudentChild studentChild;

        public StudentService(IStudentChild studentChild)
        {
            this.studentChild = studentChild;
        }
        public Student GetNewStudentInformation()
        {
           var result= studentChild.GetChildStudentInformation();
            return result;
        }

        public Student GetStudentInformation()
        {
            Student s1= new Student();
            s1.Id = 1;
            s1.Name = "supreme";
            s1.Age = 22;
            return s1;
        }

        Student IStudentInterface.GetNewStudentInformation()
        {
            throw new NotImplementedException();
        }

        Student IStudentInterface.GetStudentInformation()
        {
            throw new NotImplementedException();
        }
    }
}
