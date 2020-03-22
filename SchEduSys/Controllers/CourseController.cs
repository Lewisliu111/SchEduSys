using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchEduSys.Models;

namespace SchEduSys.Controllers
{
    public class CourseController : Controller
    {
        private SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
        // GET: Course
        public bool AddACourse(String courseName,DateTime course)
        {
            course course_already_in=schEduSysEntities.course.Where(co=>co.)

            course newcourse = new course
            {

            };
            return true;
        }
    }
}