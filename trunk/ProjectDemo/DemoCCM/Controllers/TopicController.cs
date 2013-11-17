﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoCCM.Models;

namespace DemoCCM.Controllers
{
    public class TopicController : Controller
    {
        //
        // GET: /ChuDe/

        ConceptMapDBContext db = new ConceptMapDBContext();
      
        public PartialViewResult _dropdownListPartial(String LevelID, String topicId1)
        {
            List<ConceptsForTopic> ct =
                db.ConceptsForTopics.Where(p => p.TopicID.Equals(topicId1) && p.Levels.Contains(LevelID)).ToList();
            ViewBag.cd = new SelectList(ct, "ConceptID", "Question");//tham số thứ chứa Field load lên
           //=--------------
            ViewBag.levelId = LevelID;
            ViewBag.topicId = topicId1;
            //--------------------------
            return PartialView(ct);
        }

        //giúp giữ lại những câu hỏi thuộc Câp độ và CHủ đề đang đứng
        [HttpPost, ActionName("_dropdownListPartial")]
        public PartialViewResult _dropdownListPartialkkk(String LevelID, String topicId1, string Bien1)
        {
            List<ConceptsForTopic> ct =
                db.ConceptsForTopics.ToList();
            ViewBag.cd = new SelectList(ct, "ConceptID", "Question");//tham số thứ chứa Field load lên

            return PartialView();
        }

        
        //tui mun lấy gtri LevelID1, đưa wa Indexxxxx
        public ActionResult Index(String LevelID1, String topicId1, string ConceptID)
        {
            ViewBag.levelID2 = LevelID1;
            ViewBag.topicID2 = topicId1;
           
           
            if (ConceptID == null)
                ConceptID = db.ConceptsForTopics.FirstOrDefault(c => c.TopicID.Equals(topicId1)&&c.Levels.Contains(LevelID1)).ConceptID;
            Map m = getMap(LevelID1, topicId1, ConceptID);
            return View(m);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult Indexxx(String LevelID1, String topicId1, String ConceptID)
        {
            Map m = getMap(LevelID1, topicId1, ConceptID);
            return View(m);
        }

        
    
       
        public List<Link> listLink(String LevelID, String topicId1, string ConceptID)
        {
            List<Link> list=new List<Link>();
            var ids = from l in db.Links
                      join c in db.ConceptAlls on l.ConceptID2 equals c.ConceptID
                      join t in db.ConceptsForTopics on c.ConceptID equals t.ConceptID
                      where l.ConceptID1.Equals(ConceptID) && t.Levels.Contains(LevelID) && t.TopicID.Equals(topicId1)
                      select l;
            list.AddRange(ids.ToList());
            foreach (var l in ids)
            {
                list.AddRange(listLink(LevelID,topicId1,l.ConceptID2));
            }
            return list;
        }
        //Lấy map truyền dữ liệu vào
        public Map getMap(String LevelID, String topicId1, string ConceptID)
        {
            Map m = new Map();
            List<Link> links = listLink(LevelID,topicId1,ConceptID);
            var cons = from c in db.ConceptAlls
                       select c;
            m.Links = links;
            List<ConceptAll> concepts = cons.ToList();
            var result = from c in concepts
                         join l in links on c.ConceptID equals l.ConceptID2
                         select c;
            m.Concepts = result.ToList();
            m.Concepts.Insert(0, cons.Where(p => p.ConceptID.Equals(ConceptID)).First());
            return m;
        }
       
        //Làm tương tự như Khái Niệm 
        public PartialViewResult _LinkOfTopicPartial(List<Link> links)
        {
            return PartialView(links);
        }

    }
}
