using PartialEdit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PartialEdit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Your application description page.";
            var data = DataModel.GetData();
            return View(data);
        }

        public ActionResult About()
        {
           
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ListItem(int id, string assignedTo,bool isbutton)
        {
            var model = new AssignHelpRequest
            {
                Id = id,
                IsButton=isbutton,
                AssignedTo = !string.IsNullOrEmpty(assignedTo) ? assignedTo : "DK"
            };

            return PartialView(model);
        }

        public ActionResult Edit(int? id, string assignedTo)
        {
            var model = new AssignHelpRequest
            {
                Id = id ?? 0,
                AssignedTo = assignedTo
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AssignHelpRequest model)
        {            

            if (ModelState.IsValid)
            {
                try
                {
                    var ids = Request.Form["selectedids"];
                    if (!string.IsNullOrEmpty(ids))
                    {
                        string[] users = ids.Split(',');
                    }
                    // _studentRepository.AssignHelpRequest(model.Id, model.AssignedTo);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("AssignedTo", ex.Message);
                }
            }

           // model.UserList = GetAssignedToList();
            return PartialView(model);
        }

        [HttpPost]
        public void Save(RequestData data)
        {
          
        }

    }

    public class RequestData {
        public List<int> Ids { get; set; }

        public string AssinedTo { get; set; }
    }

}