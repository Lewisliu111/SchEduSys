using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchEduSys.Models;
using SchEduSys.Controllers;

namespace SchEduSys.Controllers
{
    public class DepartmentController : Controller
    {
        private SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
        //添加一个学院。
        [HttpPost]
        public bool AddDepartment(String departName,DateTime departCreatedTime,int departCode)
        {
            department department_in = schEduSysEntities.department.FirstOrDefault(dp => dp.departName == departName);
            if (department_in != null)
            {
                ViewBag.AddDepartErrorLog = "学院已经存在！";
                return false;
            }
            department newdepartment = new department()
            {
                departName = departName,
                departCreatedTime = departCreatedTime,
                departCode = departCode
            };
            try
            {
                schEduSysEntities.department.Add(newdepartment);
            }
            catch (Exception)
            {
                return false;
            }
            schEduSysEntities.SaveChanges();
            return true;
        }

        //修改学院的相关信息。
        [HttpPost]
        public bool ModifyDepartment(String oldDepartName,String newDepartName,DateTime departCreatedTime,int departCode)
        {
            department department_in = schEduSysEntities.department.FirstOrDefault(dp => dp.departName == newDepartName);
            if (department_in != null&&!oldDepartName.Equals(newDepartName))
            {
                ViewBag.ModifyDepartErrorLog = "要修改的学院名已经存在，请重新输入！";
                return false;
            }
            department Thedepartment = schEduSysEntities.department.FirstOrDefault(dp => dp.departName == oldDepartName);
            if (Thedepartment == null)
            {
                ViewBag.ModifyDepartErrorLog = "要修改的学院不存在，请重新输入！";
                return false;
            }
            //修改course表
            while (true)
            {
                course course_modify = schEduSysEntities.course.FirstOrDefault(co => co.courseDepartmentName == oldDepartName);
                if (course_modify == null)
                {
                    break;
                }
                course_modify.courseDepartmentName = newDepartName;
                schEduSysEntities.SaveChanges();
            }
            Thedepartment.departName = newDepartName;
            Thedepartment.departCreatedTime = departCreatedTime;
            Thedepartment.departCode = departCode;
            schEduSysEntities.SaveChanges();
            return true;
        }

        //删除一个学院。
        public bool DropDepartment(String departName)
        {
            department department_drop = schEduSysEntities.department.FirstOrDefault(dp => dp.departName == departName);
            if (department_drop == null)
            {
                ViewBag.DropDepartErrorLog = "要删除的学院不存在！";
                return false;
            }
            //删除course表的数据
            while (true)
            {
                course course_drop = schEduSysEntities.course.FirstOrDefault(co => co.courseDepartmentId == department_drop.departId);
                if (course_drop == null)
                {
                    break;
                }

                //删除课程图片。
                #region
                int index = course_drop.courseLogo.LastIndexOf('\\');
                String PicName = course_drop.courseLogo.Substring(index + 1);
                String PicPath = Server.MapPath("../Views/images/courseimages/" + PicName);
                System.IO.File.Delete(PicPath);
                #endregion

                //删除courseandTopic表的数据。
                while (true)
                {
                    courseandtopic courseandtopic_drop = schEduSysEntities.courseandtopic.FirstOrDefault(cat => cat.courseId == course_drop.courseId);
                    if (courseandtopic_drop == null)
                    {
                        break;
                    }
                    schEduSysEntities.courseandtopic.Remove(courseandtopic_drop);
                    schEduSysEntities.SaveChanges();
                }
                //删除course表的数据。
                schEduSysEntities.course.Remove(course_drop);
                schEduSysEntities.SaveChanges();
            }
            //删除department表的数据。
            schEduSysEntities.department.Remove(department_drop);
            schEduSysEntities.SaveChanges();
            return true;
        }

        //查询所有的学院。
        public void GetAllDepartment()
        {
            List<department> departments = schEduSysEntities.department.ToList();
            ViewBag.departments = departments;
        }

        //查询指定学院的相关信息。
        public void GetDepartmentByName(String departName)
        {
            department department_get = schEduSysEntities.department.FirstOrDefault(dp => dp.departName == departName);
            ViewBag.department_get = department_get;
        }

        public void GetDepartmentByCode(int departCode)
        {
            department department_get = schEduSysEntities.department.FirstOrDefault(dp => dp.departCode == departCode);
            ViewBag.department_get = department_get;
        }
    }
}