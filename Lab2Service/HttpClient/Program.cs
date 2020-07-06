using System;
using System.Net.NetworkInformation;
using SchoolServiceRef;
using MathServiceRef;
using SchoolServiceClient = SchoolServiceRef.SchoolServiceClient;
using MathServiceClient = MathServiceRef.MathServiceClient;
using Lab2Service;

namespace HttpClient
{
    class Program
    {
        static async void Test()
        {

            SchoolServiceClient proxy = new SchoolServiceClient();

            Console.WriteLine("Students:");
            var newStudent = await proxy.AddStudentAsync("A123456", "Smith", "Bill", DateTime.Parse("2/3/1977"),
            GenderEnum.Male, "Communication", 33f, 3.5f);
            Console.WriteLine($"Adding student: {newStudent.ID} {newStudent.LastName} {newStudent.FirstName}");
            Console.WriteLine();

            newStudent = await proxy.AddStudentAsync("B435345", "Williams", "Bill", DateTime.Parse("1/3/1988"),
            GenderEnum.Male, "Computer Science", 31f, 2.5f);
            Console.WriteLine($"Adding student: { newStudent.ID} {newStudent.LastName} {newStudent.FirstName}");
            Console.WriteLine();

            newStudent = await proxy.AddStudentAsync("D777666", "Francis", "Jill", DateTime.Parse("8/8/1982"),
            GenderEnum.Female, "Math", 22f, 3.9f);
            Console.WriteLine($"Adding student: { newStudent.ID} {newStudent.LastName} {newStudent.FirstName}");
            Console.WriteLine();

            Console.WriteLine("Get Student Method should print Student ID");
            var getStudent = await proxy.GetStudentAsync("D777666");
            Console.WriteLine($"Student ID: {getStudent.ID}");
            Console.WriteLine();

            Console.WriteLine("Get List of all Students");
            var students = await proxy.GetStudentsAsync();

            Console.WriteLine($"Count of Students: {students.Count}");
            foreach (Student s in students) {
                Console.WriteLine($"{s.ID} {s.LastName} {s.FirstName}");
            }
            Console.WriteLine();

            Console.WriteLine("Update student:");
            var updatedStudent = await proxy.UpdateStudentAsync("B435345", "Williams", "William", DateTime.Parse("1/3/1988"), GenderEnum.Male, "Computer Science", 31f, 2.5f);
            Console.WriteLine($"{updatedStudent.ID} {updatedStudent.LastName} {updatedStudent.FirstName}");
            Console.WriteLine();

            await proxy.DeleteStudentAsync("D777666");
            students = await proxy.GetStudentsAsync();

            Console.WriteLine("Get List of Students minus deleted student");
            Console.WriteLine($"Count of Students: {students.Count}");
            foreach (Student s in students)
            {
                Console.WriteLine($"{s.ID} {s.LastName} {s.FirstName}");
            }
            Console.WriteLine();

            Console.WriteLine("Teachers:");
            var newTeacher = await proxy.AddTeacherAsync(135791, "Jones", "James", DateTime.Parse("12/31/1967"),
            GenderEnum.Male, DateTime.Parse("8/21/2015"), 84000);
            Console.WriteLine($"Adding teacher: {newTeacher.ID} {newTeacher.LastName} {newTeacher.FirstName}");
            Console.WriteLine();

            newTeacher = await proxy.AddTeacherAsync(246802, "Johnson", "Julie", DateTime.Parse("4/4/1970"),
            GenderEnum.Female, DateTime.Parse("1/2/2012"), 92000);
            Console.WriteLine($"Adding teacher:{newTeacher.ID} {newTeacher.LastName} {newTeacher.FirstName}");
            Console.WriteLine();

            newTeacher = await proxy.AddTeacherAsync(123456, "Applebee", "Susan", DateTime.Parse("11/12/1979"),
            GenderEnum.Female, DateTime.Parse("10/03/2010"), 98000);
            Console.WriteLine($"Adding teacher: {newTeacher.ID} {newTeacher.LastName} {newTeacher.FirstName}");
            Console.WriteLine();

            Console.WriteLine("Get Teacher Method should print Teacher ID");
            var getTeacher = await proxy.GetTeacherAsync(246802);
            Console.WriteLine($"Teacher ID: {getTeacher.ID}");
            Console.WriteLine();

            Console.WriteLine("Get List of all Teachers");
            var teachers = await proxy.GetTeachersAsync();

            Console.WriteLine($"Count of Teachers: {teachers.Count}");
            foreach (Teacher t in teachers)
            {
                Console.WriteLine($"{t.ID} {t.LastName} {t.FirstName}");
            }
            Console.WriteLine();

            Console.WriteLine("Update teacher:");
            var updatedTeacher = await proxy.UpdateTeacherAsync(123456, "Blossom", "Susan", DateTime.Parse("11/12/1979"),
            GenderEnum.Female, DateTime.Parse("10/03/2010"), 98000);
            Console.WriteLine($"{updatedTeacher.ID} {updatedTeacher.LastName} {updatedTeacher.FirstName}");
            Console.WriteLine();

            await proxy.DeleteTeacherAsync(135791);
            teachers = await proxy.GetTeachersAsync();

            Console.WriteLine("Get List of Teachers minus deleted teacher");
            Console.WriteLine($"Count of Teachers: {teachers.Count}");
            foreach (Teacher t in teachers)
            {
                Console.WriteLine($"{t.ID} {t.LastName} {t.FirstName}");
            }

            MathServiceClient mathproxy = new MathServiceClient();
            Console.WriteLine();

            Console.WriteLine("Add method:");
            double result = await mathproxy.AddAsync(12.5, 2.3);
            Console.WriteLine($"Add 12.5 and 2.3: {result}");

            double subtractResult = await mathproxy.SubtractAsync(44.26, 22.13);
            Console.WriteLine($"Subtract 22.13 from 44.26: {subtractResult}");

            double multiplyResult = await mathproxy.MultiplyAsync(12.21, 21.12);
            Console.WriteLine($"Multiply 12.21 and 21.12: {multiplyResult}");

            double divideResult = await mathproxy.DivideAsync(144, 12);
            Console.WriteLine($"Divide 144 by 12: {divideResult}");

            double circleAreaResult = await mathproxy.CircleAreaAsync(2.34);
            Console.WriteLine($"The area of a circle: {circleAreaResult}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press <ENTER> to start...");
            Console.ReadLine();

            Test();
            
            Console.WriteLine("Press <ENTER> to quit...");
            Console.ReadLine();
        }
    }
}
