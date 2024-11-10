using QLBVot.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QLBVot.Controllers
{
    public class VotsController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // GET: Vots
        public ActionResult Index()
        {
            var vots = db.Vots.ToList(); // Lấy tất cả vợt từ cơ sở dữ liệu
            return View(vots); // Truyền danh sách vợt vào view
        }

        // GET: Vots/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Lỗi 400 nếu `id` không hợp lệ
            }

            var vot = db.Vots.FirstOrDefault(v => v.MaVot == id);
            if (vot == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy vợt
            }

            return View(vot); // Trả về view chi tiết vợt
        }

        // GET: Vots/Create
        public ActionResult Create()
        {
            return View(); // Trả về view tạo vợt mới
        }

        // POST: Vots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vot model, HttpPostedFileBase Anh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Vots.Add(model);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Vợt đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm vợt: " + ex.Message;
                    return View(model);
                }
            }

            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            return View(model);
        }

        // GET: Vots/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var vot = db.Vots.FirstOrDefault(v => v.MaVot == id);
            if (vot == null)
            {
                return HttpNotFound();
            }

            return View(vot);
        }

        // POST: Vots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vot model, HttpPostedFileBase Anh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vot = db.Vots.FirstOrDefault(v => v.MaVot == model.MaVot);
                    if (vot == null)
                    {
                        return HttpNotFound();
                    }

                    vot.TenVot = model.TenVot;
                    vot.GiaBan = model.GiaBan;
                    vot.SoLuongTon = model.SoLuongTon;

                    if (Anh != null && Anh.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(Anh.FileName);
                        var directoryPath = Server.MapPath("~/Content/Images");

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var filePath = Path.Combine(directoryPath, fileName);
                        Anh.SaveAs(filePath);
                        vot.Anh = "~/Content/Images/" + fileName;
                    }

                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Vợt đã được cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật vợt: " + ex.Message;
                    return View(model);
                }
            }

            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            return View(model);
        }

        // GET: Vots/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var vot = db.Vots.FirstOrDefault(v => v.MaVot == id);
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
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var vot = db.Vots.FirstOrDefault(v => v.MaVot == id);
            if (vot != null)
            {
                db.Vots.Remove(vot);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Vợt đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy vợt để xóa.";
            }
            return RedirectToAction("Index");
        }
    }
}
