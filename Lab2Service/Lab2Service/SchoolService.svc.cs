using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace Lab2Service
{
	public class SchoolService : ISchoolService
	{
		public Student AddStudent(string id, string lastName, string firstName, DateTime dob, GenderEnum gender, string major, float units, float gpa)
		{
			DataStore.ClearData();
			var data = DataStore.LoadData();
			Student result = null;
			var student = GetStudent(id);
			if (student == null)
			{
				// Student doesn't exist
				result = new Student() { ID = id, LastName = lastName, FirstName = firstName,
					DOB = dob, Gender = gender, Major = major, Units = units, GPA = gpa	};
				data.Add(result);
				DataStore.SaveData();
			}
			return result;
		}

		public void DeleteStudent(string id)
		{
			var data = DataStore.LoadData();
			data.RemoveAll(person => person is Student && (person as Student).ID == id);
			DataStore.SaveData();
		}

		public Student GetStudent(string id)
		{
			var data = DataStore.LoadData();
			var query = from person in data
						let student = person as Student
						where student != null && student.ID == id
						select student;
			return query.FirstOrDefault();
		}

		public List<Student> GetStudents()
		{
			var data = DataStore.LoadData();
			var query = from person in data
						let student = person as Student
						where student != null
						select student;
			return query.ToList();
		}

		public Student UpdateStudent(string id, string lastName, string firstName, DateTime dob, GenderEnum gender, string major, float units, float gpa)
		{
			// Simplest technique is to remove then add
			DeleteStudent(id);
			return AddStudent(id, lastName, firstName, dob, gender, major, units, gpa);
		}

		/// <summary>
		/// Add teacher
		/// </summary>
		/// <param name="id">teacher id</param>
		/// <param name="lastName">teacher last name</param>
		/// <param name="firstName">teacher first name</param>
		/// <param name="dob">date of birth</param>
		/// <param name="gender">gender</param>
		/// <param name="dateOfHire">hire date</param>
		/// <param name="salary">salary</param>
		/// <returns>Teacher</returns>
        public Teacher AddTeacher(int id, string lastName, string firstName, DateTime dob, GenderEnum gender, DateTime dateOfHire, int salary)
        {
			var data = DataStore.LoadData();
			Teacher result = null;
			var teacher = GetTeacher(id);
			if (teacher == null)
			{
				// Teacher doesn't exist
				result = new Teacher()
				{
					ID = id,
					LastName = lastName,
					FirstName = firstName,
					DOB = dob,
					Gender = gender,
					DateOfHire = dateOfHire,
					Salary = salary
				};
				data.Add(result);
				DataStore.SaveData();
			}
			return result;
		}

		/// <summary>
		/// Deletes a teacher
		/// </summary>
		/// <param name="id">id of teacher</param>
        public void DeleteTeacher(int id)
        {
			var data = DataStore.LoadData();
			data.RemoveAll(person => person is Teacher && (person as Teacher).ID == id);
			DataStore.SaveData();
		}

		/// <summary>
		/// Gets teacher from data store
		/// </summary>
		/// <param name="id">id of the teacher</param>
		/// <returns>Teacher</returns>
        public Teacher GetTeacher(int id)
        {
			var data = DataStore.LoadData();
			var query = from person in data
						let teacher = person as Teacher
						where teacher != null && teacher.ID == id
						select teacher;
			return query.FirstOrDefault();
		}

		/// <summary>
		/// Gets list of teachers
		/// </summary>
		/// <returns>List of type Teacher</returns>
        public List<Teacher> GetTeachers()
        {
			var data = DataStore.LoadData();
			var query = from person in data
						let teacher = person as Teacher
						where teacher != null
						select teacher;
			return query.ToList();
		}

		/// <summary>
		/// Updates a teacher, first deletes the teacher, then adds
		/// </summary>
		/// <param name="ID">teacher id</param>
		/// <param name="lastName">teacher last name</param>
		/// <param name="firstName">teacher first name</param>
		/// <param name="dob">teacher date of birth</param>
		/// <param name="gender">teacher gender</param>
		/// <param name="dateOfHire">teacher date of hire</param>
		/// <param name="salary">teacher salary</param>
		/// <returns>Teacher</returns>
        public Teacher UpdateTeacher(int ID, string lastName, string firstName, DateTime dob, GenderEnum gender, DateTime dateOfHire, int salary)
        {
			// Remove then add
			DeleteTeacher(ID);
			return AddTeacher(ID, lastName, firstName, dob, gender, dateOfHire, salary);
		}

		public PersonList GetPeople(string lastName, string firstName)
		{
			var data = DataStore.LoadData();
			var query = data.AsQueryable();
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName == lastName);
			}
			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName == firstName);
			}
			return new PersonList(query);
		}
	}
}
