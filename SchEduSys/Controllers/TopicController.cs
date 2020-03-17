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
        public SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
       

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
            coursetopic dTopic = schEduSysEntities.coursetopic.FirstOrDefault(m => m.topicName == dName);
            if (dTopic == null)
            {
                return false;
            }
            schEduSysEntities.coursetopic.Remove(dTopic);
            schEduSysEntities.SaveChanges();
            return true;
        }
    }
}