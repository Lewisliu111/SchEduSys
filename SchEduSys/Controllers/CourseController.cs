﻿using System;
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
        //课程有：
        //课程Id，课程Name，开设时间StartTime，课程图片Logo，课程描述Description，课程代码Code，课程学分Credit，开课年级Level，选修必修Type，课程学院Id，课程学院Name，课程学时Period，课程常见问题FAQ，课程毕业政策GradingPolicy，课程基础要求Requirements
        //新建课程要求输入：
        //课程Name，开设时间StartTime，课程图片Logo，课程描述Description，课程代码Code，课程学分Credit，开课年级Level，选修必修Type，课程学院Name，课程学时Period，课程常见问题FAQ，课程毕业政策GradingPolicy，课程基础要求Requirements


        //新建一个课程。
        public bool AddCourse(String courseName, DateTime courseStartTime, String courseLogo, String courseDescription, String courseCode, float courseCredit, String courseLevel, String courseType, String departmentName, int coursePeriod, String courseFAQ, String courseGradingPolicy, String courseRequirements, String courseTopicNameStr)
        {
            ViewBag.AddCourseErrorLog = null;
            //判断该课程是否可以加入。
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
            //新建course对象
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
            schEduSysEntities.course.Add(newcourse);
            schEduSysEntities.SaveChanges();
            newcourse = schEduSysEntities.course.FirstOrDefault(co => co.courseCode == courseCode);
            //将课程与课程类型进行绑定。
            String[] courseTopicNames = courseTopicNameStr.Split('，');
            foreach (String courseTopicName in courseTopicNames)
            {
                coursetopic course_topic_use = schEduSysEntities.coursetopic.FirstOrDefault(cat => cat.topicName == courseTopicName);
                courseandtopic newcourseandtopic = new courseandtopic()
                {
                    courseId = newcourse.courseId,
                    topicId=course_topic_use.topicId
                };
                schEduSysEntities.courseandtopic.Add(newcourseandtopic);
            }
            schEduSysEntities.SaveChanges();
            return true;
        }

        //修改指定id的课程。
        public bool ModifyCourse(int courseId, String courseName, DateTime courseStartTime, String courseLogo, String courseDescription, String courseCode, float courseCredit, String courseLevel, String courseType, String departmentName, int coursePeriod, String courseFAQ, String courseGradingPolicy, String courseRequirements, String courseTopicNameStr)
        {
            ViewBag.ModifyCourseErrorLog = null;
            //判断要修改的课程是否存在。
            course TheCourse = schEduSysEntities.course.Find(courseId);
            if (TheCourse == null)
            {
                ViewBag.ModifyCourseErrorLog = "要修改的课程不存在！";
                return false;
            }
            //判断新输入的课程代码是否已经存在。
            course course_already_in = schEduSysEntities.course.FirstOrDefault(co => co.courseCode == courseCode);
            if (course_already_in != null)
            {
                ViewBag.AddCourseErrorLog = "新的课程代码已存在，请重新输入！";
                return false;
            }
            //判断新的学院是否不存在。
            department department_in = schEduSysEntities.department.FirstOrDefault(de => de.departName == departmentName);
            if (department_in == null)
            {
                ViewBag.AddCourseErrorLog = "新的学院不存在，请先创建学院！";
                return false;
            }
            TheCourse.courseName = courseName;
            TheCourse.courseStartTime = courseStartTime;
            TheCourse.courseLogo = courseLogo;
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
            //删除courseandtopic表中所有关于这个课程的数据。
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
            //添加新的courseandtopic数据。
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
            return true;
        }

        //删除指定id的课程。
        public bool DropCourse(int courseId)
        {
            course course_drop = schEduSysEntities.course.Find(courseId);
            if (course_drop == null)
            {
                ViewBag.DropCourseErrorLog = "要删除的课程不存在";
                return false;
            }
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
            schEduSysEntities.course.Remove(course_drop);
            schEduSysEntities.SaveChanges();
            return true;
        }
    }
}