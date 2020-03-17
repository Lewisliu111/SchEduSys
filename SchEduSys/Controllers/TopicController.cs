using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchEduSys.Models;

namespace SchEduSys.Controllers
{
    public class TopicController : Controller
    {
        public SchEduSysEntities schEduSysEntities = new SchEduSysEntities();
        // GET: Topic
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
    }
}