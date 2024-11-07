using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBVot.Models;

namespace QLBVot.Controllers
{
    public class VotsController : Controller
    {
        private DVHVOTEntities db = new DVHVOTEntities();

        // GET: Vots
        public ActionResult Index()
        {
            return View(db.Vots.ToList());
        }

        // GET: Vots/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vot vot = db.Vots.Find(id);
            if (vot == null)
            {
                return HttpNotFound();
            }
            return View(vot);
        }

        // GET: Vots/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaVot,TenVot,GiaBan,SoLuongTon")] Vot vot)
        {
            if (ModelState.IsValid)
            {
                db.Vots.Add(vot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vot);
        }

        // GET: Vots/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vot vot = db.Vots.Find(id);
            if (vot == null)
            {
                return HttpNotFound();
            }
            return View(vot);
        }

        // POST: Vots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaVot,TenVot,GiaBan,SoLuongTon")] Vot vot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vot);
        }

        // GET: Vots/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vot vot = db.Vots.Find(id);
            if (vot == null)
            {
                return HttpNotFound();
            }
            return View(vot);
        }

        // POST: Vots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Vot vot = db.Vots.Find(id);
            db.Vots.Remove(vot);
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
