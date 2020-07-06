using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Lab2Service
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISchoolService" in both code and config file together.
	[ServiceContract]
	public interface ISchoolService
	{
		[OperationContract]
		Student AddStudent(string id, string lastName, string firstName, DateTime dob, GenderEnum gender, string major, float units, float gpa);
		
		[OperationContract]
		void DeleteStudent(string id);
		
		[OperationContract]
		Student GetStudent(string id);
		
		[OperationContract]
		List<Student> GetStudents();
		
		[OperationContract]
		Student UpdateStudent(string id, string lastName, string firstName, DateTime dob, GenderEnum gender, string major, float units, float gpa);

		[OperationContract]
		Teacher AddTeacher(int ID, string lastName, string firstName, DateTime dob, GenderEnum gender, DateTime dateOfHire, int salary);

		[OperationContract]
		void DeleteTeacher(int id);

		[OperationContract]
		Teacher GetTeacher(int id);

		[OperationContract]
		List<Teacher> GetTeachers();

		[OperationContract]
		Teacher UpdateTeacher(int ID, string lastName, string firstName, DateTime dob, GenderEnum gender, DateTime dateOfHire, int salary);

		[OperationContract]
		PersonList GetPeople(string lastName, string firstName);
	}
}
