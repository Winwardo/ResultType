using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultType;

namespace Students
{
    public class Student
    {
        public int age { get; private set; }
        public int mark { get; private set; }
        public Student(int age, int mark)
        {
            this.age = age;
            this.mark = mark;
        }
    }
    public enum StudentError { NotOldEnough, MarkIsTooLow };
}

namespace ResultTypeTests.Examples
{
    using NUnit.Framework;
    using Students;
    using StudentIResult = IResult<Students.Student, Students.StudentError>;
    using StudentResult = Result<Students.Student, Students.StudentError>;

    [TestFixture]
    class ValidateStudent
    {
        [Test]
        public void IntegrationValidateStudents()
        {
            Student alex = new Student(20, 30);
            Student tom = new Student(12, 70);
            Student steve = new Student(12, 30);
            Student topher = new Student(20, 70);

            Assert.AreEqual(StudentError.MarkIsTooLow, validateStudent(alex).UnwrapErrorUnsafe());
            Assert.AreEqual(StudentError.NotOldEnough, validateStudent(tom).UnwrapErrorUnsafe());
            Assert.AreEqual(StudentError.NotOldEnough, validateStudent(steve).UnwrapErrorUnsafe());
            Assert.AreEqual(topher, validateStudent(topher).UnwrapUnsafe());

            StudentIResult validatedTopherResult = validateStudent(topher);
            if (validatedTopherResult.IsOk())
            {
                Student validatedTopher = validatedTopherResult.Unwrap();
                // Do interesting stuff
            }
            else
            {
                // Topher isn't valid
            }
        }

        private StudentIResult validateStudent(Student student)
        {
            return StudentResult.Ok(student)
                .AndThen(validateStudentAge)
                .AndThen(validateStudentMark);
        }

        private StudentIResult validateStudentAge(Student student)
        {
            if (student.age >= 18)
            {
                return StudentResult.Ok(student);
            }
            else
            {
                return StudentResult.Error(StudentError.NotOldEnough);
            }
        }

        private StudentIResult validateStudentMark(Student student)
        {
            if (student.mark >= 50)
            {
                return StudentResult.Ok(student);
            }
            else
            {
                return StudentResult.Error(StudentError.MarkIsTooLow);
            }
        }
    }
}
