using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchEduSys.Models;
using MySql.Data.EntityFramework;

namespace SchEduSys.Controllers
{
    public class TopicController : Controller
    {
        private SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
        //添加一个课程类型，例如：计算机类
        public bool AddATopic(String tName)
        {
            coursetopic newtopic = new coursetopic()
            {
                topicName = tName
            };
            try
            {
                schEduSysEntities.coursetopic.Add(newtopic);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //删除一个课程类型，例如：数学类
        public bool DropATopic(String dName)
        {
            coursetopic dTopic = schEduSysEntities.coursetopic.FirstOrDefault(tp => tp.topicName == dName);
            if (dTopic == null)
            {
                return false;
            }
            //删除courseandTopic表的数据
            while (true)
            {
                courseandtopic dcourseandtopic = schEduSysEntities.courseandtopic.FirstOrDefault(cat => cat.topicId == dTopic.topicId);
                if (dcourseandtopic == null)
                {
                    break;
                }
                schEduSysEntities.courseandtopic.Remove(dcourseandtopic);
                schEduSysEntities.SaveChanges();
            }
            //删除coursetopic表的数据。
            schEduSysEntities.coursetopic.Remove(dTopic);
            schEduSysEntities.SaveChanges();
            return true;
        }

        //修改课程类型的信息
        public bool ModifyTopic(String oldTopicName, String NewTopicName)
        {
            coursetopic TheTopic = schEduSysEntities.coursetopic.FirstOrDefault(ct => ct.topicName == oldTopicName);
            if (TheTopic == null)
                return false;
            TheTopic.topicName = NewTopicName;
            schEduSysEntities.SaveChanges();
            return true;
        }

        //获得所有的课程类型。
        public void GetAllTopics()
        {
            List<coursetopic> coursetopics = schEduSysEntities.coursetopic.ToList();
            ViewBag.coursetopics = coursetopics;
        }
    }
}