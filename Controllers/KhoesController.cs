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
    public class KhoesController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // GET: Khoes
        public ActionResult Index()
        {
            var khoes = db.Khoes.Include(k => k.Vot);
            return View(khoes.ToList());
        }

        // GET: Khoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // GET: Khoes/Create
        public ActionResult Create()
        {
            ViewBag.MaVot = new SelectList(db.Vots, "MaVot", "TenVot");
            return View();
        }

        // POST: Khoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKho,MaVot,SoLuongTon")] Kho kho)
        {
            if (ModelState.IsValid)
            {
                db.Khoes.Add(kho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaVot = new SelectList(db.Vots, "MaVot", "TenVot", kho.MaVot);
            return View(kho);
        }

        // GET: Khoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaVot = new SelectList(db.Vots, "MaVot", "TenVot", kho.MaVot);
            return View(kho);
        }

        // POST: Khoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKho,MaVot,SoLuongTon")] Kho kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaVot = new SelectList(db.Vots, "MaVot", "TenVot", kho.MaVot);
            return View(kho);
        }

        // GET: Khoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // POST: Khoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kho kho = db.Khoes.Find(id);
            db.Khoes.Remove(kho);
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
