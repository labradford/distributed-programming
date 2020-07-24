using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Client
{
    public class Program
    {
        const string BASE_ADDR = "https://localhost:44336/api/students/";

        static void Main(string[] args)
        {
            var result = ClientHelper.Get<Student>(BASE_ADDR, SerializationModesEnum.Json, "{0}", 136655918);
            Console.WriteLine();

            Console.WriteLine("Get Single Student:");
            Console.WriteLine(result.Result);
            Console.WriteLine();

            var results = ClientHelper.Get<List<Student>>(BASE_ADDR, SerializationModesEnum.Json, "?page={0}&count={1}", 5, 5);
            Console.WriteLine();
            
            Console.WriteLine("Get List of 5 Students:");
            foreach (var s in results.Result)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine();

            Student add = new Student
            {
                ID = 123456789,
                LastName = "Gates",
                FirstName = "Bill",
                Grade = Student.GradeEnum.College,
                DOB = new DateTime(1955, 10, 28),
                GPA = 3.5f
            };

            try
            {
                ClientHelper.Post<Student>(BASE_ADDR, SerializationModesEnum.Json, add, string.Empty);
                Console.WriteLine("Student was successfully added.");
            } catch(Exception ex)
            {
                Console.WriteLine($"There was an error adding student: {ex}");
            }

            Console.WriteLine();

            // Update student
            add.LastName = "Jobs";
            add.FirstName = "Steve";
            try
            {
                ClientHelper.Put<Student>(BASE_ADDR, SerializationModesEnum.Json, add, string.Empty);
                Console.WriteLine("Student was successfully updated.");
            } catch(Exception ex)
            {
                Console.WriteLine($"There was an error updating student: {ex}");
            }

            Console.WriteLine();

            try
            {
                ClientHelper.Delete<Student>(BASE_ADDR, SerializationModesEnum.Json, add, string.Empty);
                Console.WriteLine("Student was successfully deleted.");
                Console.WriteLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an error deleting student: {ex}");
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to quit...");
            Console.ReadLine();
        }
    }
}
