using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.Controllers
{
    public class Info_requestController : Controller
    {
        private SSPDatabaseEntities db = new SSPDatabaseEntities();

        // GET: Info_request
        public ActionResult Index()
        {
            return View(db.Info_request.ToList());
        }

        // GET: Info_request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info_request info_request = db.Info_request.Find(id);
            if (info_request == null)
            {
                return HttpNotFound();
            }
            return View(info_request);
        }

        // GET: Info_request/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Info_request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Info_requestId,Name,Description,Request_user,Request_date")] Info_request info_request)
        {
            if (ModelState.IsValid)
            {
                db.Info_request.Add(info_request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(info_request);
        }

        // GET: Info_request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info_request info_request = db.Info_request.Find(id);
            if (info_request == null)
            {
                return HttpNotFound();
            }
            return View(info_request);
        }

        // POST: Info_request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Info_requestId,Name,Description,Request_user,Request_date")] Info_request info_request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(info_request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(info_request);
        }

        // GET: Info_request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info_request info_request = db.Info_request.Find(id);
            if (info_request == null)
            {
                return HttpNotFound();
            }
            return View(info_request);
        }

        // POST: Info_request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Info_request info_request = db.Info_request.Find(id);
            db.Info_request.Remove(info_request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
