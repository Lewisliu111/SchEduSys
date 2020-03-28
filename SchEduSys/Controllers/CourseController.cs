using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchEduSys.Models;

namespace SchEduSys.Controllers
{
    public class CourseController : Controller
    {
        private SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
        //课程有：
        //课程Id，课程Name，开设时间StartTime，课程图片Logo，课程描述Description，课程代码Code，课程学分Credit，开课年级Level，选修必修Type，课程学院Id，课程学院Name，课程学时Period，课程常见问题FAQ，课程毕业政策GradingPolicy，课程基础要求Requirements
        //新建课程要求输入：
        //课程Name，开设时间StartTime，课程描述Description，课程代码Code，课程学分Credit，开课年级Level，选修必修Type，课程学院Name，课程学时Period，课程常见问题FAQ，课程毕业政策GradingPolicy，课程基础要求Requirements


        //新建一个课程。
        [HttpPost]
        public bool AddCourse(String courseName, DateTime courseStartTime, String courseLogo, String courseDescription, String courseCode , String courseFAQ, String courseGradingPolicy, String courseRequirements, String courseTopicNameStr)
        {
            String departmentName=Request.Form["departmentName"].ToString();
            String courseType = Request.Form["courseType"].ToString();
            String courseLevel = Request.Form["courseLevel"].ToString();
            int coursePeriod = Convert.ToInt32(Request.Form["coursePeriod"].ToString());
            float courseCredit = (float)Convert.ToSingle(Request.Form["courseCredit"].ToString());
            ViewBag.AddCourseErrorLog = null;
            //判断该课程是否可以加入。
            #region
            course course_already_in = schEduSysEntities.course.FirstOrDefault(co => co.courseCode == courseCode);
            if (course_already_in != null)
            {
                ViewBag.AddCourseErrorLog = "课程代码已存在，请重新输入！";
                return false;
            }
            //判断学院是否存在。

            department department_in = schEduSysEntities.department.FirstOrDefault(de => de.departName == departmentName);
            if (department_in == null)
            {
                ViewBag.AddCourseErrorLog = "学院不存在，请先创建学院！";
                return false;
            }
            #endregion

            //若可以加入，则新建course对象
            #region
            course newcourse = new course()
            {
                courseName = courseName,
                courseStartTime = courseStartTime,
                courseLogo = courseLogo,
                courseDescription = courseDescription,
                courseCode = courseCode,
                courseCredit = courseCredit,
                courseLevel = courseLevel,
                courseType = courseType,
                courseDepartmentId = department_in.departId,
                courseDepartmentName = department_in.departName,
                coursePeriod = coursePeriod,
                courseFAQ = courseFAQ,
                courseGradingPolicy = courseGradingPolicy,
                courseRequirements = courseRequirements
            };
            #endregion

            //把图片存到服务器，并且获取地址，存入courseLogo。
            #region
            if (Request.Files.Count == 0)
            {
                ViewBag.AddCourseErrorLog = "没上传课程图片，请上传！";
                return false;
            }
            String uploadPath = Server.MapPath("../Views/images/courseimages/");
            // 保存文件到"/Views/Picture/CoursePics/"文件夹
            for (int i = 0; i < Request.Files.Count; ++i)
            {
                HttpPostedFileBase picture = Request.Files[i];
                //若文件名为空，证明没有选择上传文件。
                if (picture.FileName == "")
                {
                    ViewBag.AddCourseErrorLog = "没有选择上传文件";
                    return false;
                }
                String PicPath = uploadPath + Path.GetFileName(picture.FileName);
                String PicName = picture.FileName;
                // 检查上传文件的类型是否为合法图片。
                String PicExtension = Path.GetExtension(PicPath).ToLower();
                String PicFilter = ConfigurationManager.AppSettings["FileFilter"];
                if (PicFilter.IndexOf(PicExtension) <= -1)
                {
                    ViewBag.AddCourseErrorLog = "对不起！请上传图片！！";
                    return false;
                }
                // 如果服务器上已经存在同名图片，则需要修改文件名与其储存路径。
                while (System.IO.File.Exists(PicPath))
                {
                    Random rand = new Random();
                    PicName = rand.Next().ToString() + "-" + picture.FileName;
                    PicPath = uploadPath + Path.GetFileName(PicName);
                }
                // 把图片的存储路径保存到courseLogo中
                newcourse.courseLogo = PicPath;
                // 保存文件到服务器
                picture.SaveAs(PicPath);
            }
            #endregion

            //添加课程。
            #region
            schEduSysEntities.course.Add(newcourse);
            schEduSysEntities.SaveChanges();
            newcourse = schEduSysEntities.course.FirstOrDefault(co => co.courseCode == courseCode);
            #endregion

            //将课程与课程类型进行绑定。
            #region
            String[] courseTopicNames = courseTopicNameStr.Split('，');
            foreach (String courseTopicName in courseTopicNames)
            {
                coursetopic course_topic_use = schEduSysEntities.coursetopic.FirstOrDefault(cat => cat.topicName == courseTopicName);
                if (course_topic_use == null)
                {
                    ViewBag.AddCourseErrorLog = "课程类型错误或课程类型不存在，请重新输入！";
                    return false;
                }
                courseandtopic newcourseandtopic = new courseandtopic()
                {
                    courseId = newcourse.courseId,
                    topicId = course_topic_use.topicId
                };
                schEduSysEntities.courseandtopic.Add(newcourseandtopic);
            }
            schEduSysEntities.SaveChanges();
            #endregion
            return true;
        }

        //修改指定id的课程。
        [HttpPost]
        public bool ModifyCourse(int courseId, String courseName, DateTime courseStartTime, String courseDescription, String courseCode,  String courseFAQ, String courseGradingPolicy, String courseRequirements, String courseTopicNameStr)
        {
            String departmentName = Request.Form["departmentName"].ToString();
            String courseType = Request.Form["courseType"].ToString();
            String courseLevel = Request.Form["courseLevel"].ToString();
            int coursePeriod = Convert.ToInt32(Request.Form["coursePeriod"].ToString());
            float courseCredit = (float)Convert.ToSingle(Request.Form["courseCredit"].ToString());
            ViewBag.ModifyCourseErrorLog = null;
            //判断要修改的课程是否存在。
            #region
            course TheCourse = schEduSysEntities.course.Find(courseId);
            if (TheCourse == null)
            {
                ViewBag.ModifyCourseErrorLog = "要修改的课程不存在！";
                return false;
            }
            #endregion

            //判断新输入的课程代码是否已经存在。
            #region
            if (!courseCode.Equals(TheCourse.courseCode))
            {
                course course_already_in = schEduSysEntities.course.FirstOrDefault(co => co.courseCode == courseCode);
                if (course_already_in != null)
                {
                    ViewBag.AddCourseErrorLog = "新的课程代码已存在，请重新输入！";
                    return false;
                }
            }
            #endregion

            //判断新的学院是否不存在。
            #region
            department department_in = schEduSysEntities.department.FirstOrDefault(de => de.departName == departmentName);
            if (department_in == null)
            {
                ViewBag.AddCourseErrorLog = "新的学院不存在，请先创建学院！";
                return false;
            }
            #endregion

            //删除旧的课程图片。
            #region
            String uploadPath = Server.MapPath("../Views/images/courseimages/");
            int index = TheCourse.courseLogo.LastIndexOf('\\');
            String oldPicName = TheCourse.courseLogo.Substring(index + 1);
            String oldPicPath = uploadPath + oldPicName;
            System.IO.File.Delete(oldPicPath);
            #endregion

            //将新的图片插入数据库。
            #region
            if (Request.Files.Count == 0)
            {
                ViewBag.AddCourseErrorLog = "没上传新的课程图片，请上传！";
                return false;
            }
            // 保存文件到"/Views/Picture/CoursePics/"文件夹
            for (int i = 0; i < Request.Files.Count; ++i)
            {
                HttpPostedFileBase picture = Request.Files[i];
                //若文件名为空，证明没有选择上传文件。
                if (picture.FileName == "")
                {
                    ViewBag.AddCourseErrorLog = "没有选择上传文件";
                    return false;
                }
                String newPicPath = uploadPath + Path.GetFileName(picture.FileName);
                String newPicName = picture.FileName;
                // 检查上传文件的类型是否为合法图片。
                String PicExtension = Path.GetExtension(newPicPath).ToLower();
                String PicFilter = ConfigurationManager.AppSettings["FileFilter"];
                if (PicFilter.IndexOf(PicExtension) <= -1)
                {
                    ViewBag.AddCourseErrorLog = "对不起！请上传图片！！";
                    return false;
                }
                // 如果服务器上已经存在同名图片，则需要修改文件名与其储存路径。
                while (System.IO.File.Exists(newPicPath))
                {
                    Random rand = new Random();
                    newPicName = rand.Next().ToString() + "-" + picture.FileName;
                    newPicPath = uploadPath + Path.GetFileName(newPicName);
                }
                // 把图片的存储路径保存到courseLogo中
                TheCourse.courseLogo = newPicPath;
                // 保存文件到服务器
                picture.SaveAs(newPicPath);
            }


            #endregion

            //更新课程数据。
            #region
            TheCourse.courseName = courseName;
            TheCourse.courseStartTime = courseStartTime;
            TheCourse.courseDescription = courseDescription;
            TheCourse.courseCode = courseCode;
            TheCourse.courseCredit = courseCredit;
            TheCourse.courseLevel = courseLevel;
            TheCourse.courseType = courseType;
            TheCourse.courseDepartmentId = department_in.departId;
            TheCourse.courseDepartmentName = department_in.departName;
            TheCourse.coursePeriod = coursePeriod;
            TheCourse.courseFAQ = courseFAQ;
            TheCourse.courseGradingPolicy = courseGradingPolicy;
            TheCourse.courseRequirements = courseRequirements;
            schEduSysEntities.SaveChanges();
            #endregion

            //删除courseandtopic表中所有关于这个课程的数据。
            #region
            while (true)
            {
                courseandtopic dcourseandtopic = schEduSysEntities.courseandtopic.FirstOrDefault(cat => cat.courseId == TheCourse.courseId);
                if (dcourseandtopic == null)
                {
                    break;
                }
                schEduSysEntities.courseandtopic.Remove(dcourseandtopic);
                schEduSysEntities.SaveChanges();
            }
            #endregion

            //添加新的courseandtopic数据。
            #region
            String[] courseTopicNames = courseTopicNameStr.Split('，');
            foreach (String courseTopicName in courseTopicNames)
            {
                coursetopic course_topic_use = schEduSysEntities.coursetopic.FirstOrDefault(cat => cat.topicName == courseTopicName);
                courseandtopic newcourseandtopic = new courseandtopic()
                {
                    courseId = TheCourse.courseId,
                    topicId = course_topic_use.topicId
                };
                schEduSysEntities.courseandtopic.Add(newcourseandtopic);
            }
            schEduSysEntities.SaveChanges();
            #endregion

            return true;
        }


        //删除指定id的课程。
        public bool DropCourse(int courseId)
        {
            //判断要删除的课程是否存在。
            #region
            course course_drop = schEduSysEntities.course.Find(courseId);
            if (course_drop == null)
            {
                ViewBag.DropCourseErrorLog = "要删除的课程不存在";
                return false;
            }
            #endregion

            //删除课程图片。
            #region
            int index = course_drop.courseLogo.LastIndexOf('\\');
            String PicName = course_drop.courseLogo.Substring(index + 1);
            String PicPath = Server.MapPath("../Views/images/courseimages/" + PicName);
            System.IO.File.Delete(PicPath);
            #endregion

            //删除courseandTopic表的数据。
            #region
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
            #endregion
            schEduSysEntities.course.Remove(course_drop);
            schEduSysEntities.SaveChanges();
            return true;
        }

        //查询课程。
        public ActionResult Index()
        {
            var courseList = schEduSysEntities.course.SqlQuery("Select * from course limit 0, 8").ToList<course>();
            foreach (var course in courseList)
            {
                var topicIds = schEduSysEntities.Database.SqlQuery<int>("Select topicId from courseandtopic").ToList<int>();
                var topicnames = new List<string>();
                foreach (var topicid in topicIds)
                {
                    var temp = schEduSysEntities.Database.SqlQuery<string>("Select topicName from coursetopic where topicId = " + topicid).ToList<string>();
                    topicnames.Add(temp[0]);
                }
                course.topics = topicnames;
            }
            ViewBag.courseList = courseList;
            return View("Index");
        }

        public ActionResult Detail(int id)
        {
            var topicIds = schEduSysEntities.Database.SqlQuery<int>("Select topicId from courseandtopic").ToList<int>();
            var topicnames = new List<string>();
            foreach (var topicid in topicIds)
            {
                var temp = schEduSysEntities.Database.SqlQuery<string>("Select topicName from coursetopic where topicId = " + topicid).ToList<string>();
                topicnames.Add(temp[0]);
            }
            var c = schEduSysEntities.course.SqlQuery("select * from course where courseId = " + id).FirstOrDefault<course>();

            c.topics = topicnames;


            return View("Detail", c);
        }


        public ActionResult Search(String queryStr, String queryType)
        {
            var courseList = new List<course>();

            if (queryType == null)
            {
                courseList = schEduSysEntities.course.SqlQuery("Select * from course"
                     ).ToList<course>();

                foreach (var course in courseList)
                {
                    var topicIds = schEduSysEntities.Database.SqlQuery<int>("Select topicId from courseandtopic").ToList<int>();
                    var topicnames = new List<string>();
                    foreach (var topicid in topicIds)
                    {
                        var temp = schEduSysEntities.Database.SqlQuery<string>("Select topicName from coursetopic where topicId = " + topicid).ToList<string>();
                        topicnames.Add(temp[0]);
                    }

                    course.topics = topicnames;
                }
            }
            else
            {
                courseList = schEduSysEntities.course.SqlQuery("Select * from course where " + queryType + "=" + queryStr
                     ).ToList<course>();
                foreach (var course in courseList)
                {
                    var topicIds = schEduSysEntities.Database.SqlQuery<int>("Select topicId from courseandtopic").ToList<int>();
                    var topicnames = new List<string>();
                    foreach (var topicid in topicIds)
                    {
                        var temp = schEduSysEntities.Database.SqlQuery<string>("Select topicName from coursetopic where topicId = " + topicid).ToList<string>();
                        topicnames.Add(temp[0]);
                    }
                    course.topics = topicnames;
                }
            }
            ViewBag.courseList = courseList;
            return View("List");
        }
    }
}