The [Rust programming language has a hugely powerful `Result` type](https://doc.rust-lang.org/std/result/) that is used when a function can "fail" - that is, produce a result that would otherwise be considered invalid.

At the time of writing this code, C# does not have a similar pattern, so here I've put together a basic implementation that proves it out.
The following test in `ResultTypeTests/Examples/ValidateStudent.cs` shows at a high level how this could be used.

```csharp
[Test]
public void IntegrationValidateStudents()
{
    // MakeStudent considers a student valid if their age (the first parameter) is over 18, and their mark (the second parameter) is over 50.

    PotentialStudent alex = new PotentialStudent(20, 30);
    PotentialStudent tom = new PotentialStudent(12, 70);
    PotentialStudent steve = new PotentialStudent(12, 30);
    PotentialStudent topher = new PotentialStudent(20, 70);

    // Note how Alex's mark is too low, Tom is too young, Steve is also too young, but Topher is both old enough and has a high enough mark.
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
```
