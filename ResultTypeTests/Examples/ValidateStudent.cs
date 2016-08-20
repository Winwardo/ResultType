using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultType;

namespace Students
{
    using PotentialStudentResult = Result<Students.PotentialStudent, Students.StudentError>;
    using StudentIResult = IResult<Students.Student, Students.StudentError>;

    public abstract class AStudent
    {
        public int age { get; protected set; }
        public int mark { get; protected set; }
    }

    public sealed class PotentialStudent : AStudent
    {
        public PotentialStudent(int age, int mark)
        {
            this.age = age;
            this.mark = mark;
        }
    };

    public sealed class Student : AStudent
    {
        public static StudentIResult MakeStudent(PotentialStudent student)
        {
            return PotentialStudentResult.Ok(student)
                .IfThenElse(isStudentOfAge, StudentError.NotOldEnough)
                .IfThenElse(isMarkHighEnough, StudentError.MarkIsTooLow)
                .Map(value => new Student(value));
        }

        private static bool isStudentOfAge(AStudent student)
        {
            return student.age >= 18;
        }

        private static bool isMarkHighEnough(AStudent student)
        {
            return student.mark >= 50;
        }

        private Student(AStudent student)
        {
            this.age = student.age;
            this.mark = student.mark;
        }
    };

    public enum StudentError { NotOldEnough, MarkIsTooLow };
}

namespace ResultTypeTests.Examples
{
    using NUnit.Framework;
    using Students;
    using StudentIResult = IResult<Students.Student, Students.StudentError>;

    [TestFixture]
    class ValidateStudent
    {
        [Test]
        public void IntegrationValidateStudents()
        {
            PotentialStudent alex = new PotentialStudent(20, 30);
            PotentialStudent tom = new PotentialStudent(12, 70);
            PotentialStudent steve = new PotentialStudent(12, 30);
            PotentialStudent topher = new PotentialStudent(20, 70);

            Assert.AreEqual(StudentError.MarkIsTooLow, Student.MakeStudent(alex).UnwrapErrorUnsafe());
            Assert.AreEqual(StudentError.NotOldEnough, Student.MakeStudent(tom).UnwrapErrorUnsafe());
            Assert.AreEqual(StudentError.NotOldEnough, Student.MakeStudent(steve).UnwrapErrorUnsafe());
            Assert.AreEqual(topher.age, Student.MakeStudent(topher).UnwrapUnsafe().age);

            StudentIResult validatedTopherResult = Student.MakeStudent(topher);
            if (validatedTopherResult.IsOk())
            {
                Student validatedTopher = validatedTopherResult.Unwrap();
                // Student is definitely valid
            }
            else
            {
                // Topher isn't valid
            }
        }
    }
}
