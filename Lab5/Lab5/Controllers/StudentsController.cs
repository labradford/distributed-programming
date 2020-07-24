using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Lab5.Models;

namespace Lab5.Controllers
{
    public class StudentsController : ApiController
    {
        private string FilePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/App_Data/student.xml");
            }
        }

        public List<Student> Get(int page = 0, int count = 50)
        {
            var data = StudentList.Load(FilePath).AsQueryable();
            return data.Skip(page * count).Take(count).ToList();
        }

        public Student Get(int id)
        {
            var data = StudentList.Load(FilePath);
            return data.Where(s => s.ID == id).FirstOrDefault();
        }

        public void Post([FromBody] Student value)
        {
            var data = StudentList.Load(FilePath);
            Student existing = data.Where(s => s.ID == value.ID).FirstOrDefault();
            if (existing == null)
            {
                data.Add(value);
                data.Save(FilePath);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
        }

        public void Put(int id, [FromBody] Student value)
        {
            var data = StudentList.Load(FilePath);
            Student existing = data.Where(s => s.ID == id).FirstOrDefault();

            if (existing == null)
            {
                data.Add(value);
            }
            else
            {
                data.RemoveAll(s => s.ID == id);
                data.Add(value);
            }
            data.Save(FilePath);
        }

        public void Delete(int id)
        {
            var data = StudentList.Load(FilePath);
            Student existing = data.Where(s => s.ID == id).FirstOrDefault();
            if (existing != null)
            {
                data.RemoveAll(s => s.ID == id);
            }
            data.Save(FilePath);
        }
    }
}
