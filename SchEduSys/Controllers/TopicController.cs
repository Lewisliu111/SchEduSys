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
        public bool AddTopic(String topicName)
        {
            coursetopic topic_in = schEduSysEntities.coursetopic.FirstOrDefault(ct => ct.topicName == topicName);
            if(topic_in!=null)
            {
                ViewBag.AddTopicErrorLog = "课程类型已经存在！";
                return false;
            }
            coursetopic newtopic = new coursetopic()
            {
                topicName = topicName
            };
            try
            {
                schEduSysEntities.coursetopic.Add(newtopic);
            }
            catch (Exception)
            {
                return false;
            }
            schEduSysEntities.SaveChanges();
            return true;
        }

        
        //删除一个课程类型，例如：数学类
        public bool DropTopic(String topicName)
        {
            coursetopic topic_drop = schEduSysEntities.coursetopic.FirstOrDefault(tp => tp.topicName == topicName);
            if (topic_drop == null)
            {
                ViewBag.DropTopicErrorLog = "要删除的课程类型本来就不存在！";
                return false;
            }
            //删除courseandTopic表的数据
            while (true)
            {
                courseandtopic dcourseandtopic = schEduSysEntities.courseandtopic.FirstOrDefault(cat => cat.topicId == topic_drop.topicId);
                if (dcourseandtopic == null)
                {
                    break;
                }
                schEduSysEntities.courseandtopic.Remove(dcourseandtopic);
                schEduSysEntities.SaveChanges();
            }
            //删除coursetopic表的数据。
            schEduSysEntities.coursetopic.Remove(topic_drop);
            schEduSysEntities.SaveChanges();
            return true;
        }

        //修改课程类型的信息
        [HttpPost]
        public bool ModifyTopic(String oldTopicName, String NewTopicName)
        {
            coursetopic topic_in = schEduSysEntities.coursetopic.FirstOrDefault(ct => ct.topicName == NewTopicName);
            if (topic_in != null)
            {
                ViewBag.ModifyTopicErrorLog = "新的课程类型已经存在！";
                return false;
            }
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