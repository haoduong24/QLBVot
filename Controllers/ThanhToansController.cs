using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBVot.Models;
using QRCoder; // Thêm thư viện QRCoder vào dự án

namespace QLBVot.Controllers
{
    public class ThanhToansController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // GET: ThanhToans
        public ActionResult Index()
        {
            var thanhToans = db.ThanhToans.Include(t => t.DonHang);
            return View(thanhToans.ToList());
        }

        // GET: ThanhToans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            return View(thanhToan);
        }

        // GET: ThanhToans/Create
        public ActionResult Create()
        {
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "TenKhachHang");
            return View();
        }

        // POST: ThanhToans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaThanhToan,MaDonHang,PhuongThucThanhToan,NgayThanhToan,SoTienThanhToan,TrangThaiThanhToan")] ThanhToan thanhToan)
        {
            if (ModelState.IsValid)
            {
                db.ThanhToans.Add(thanhToan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "TenKhachHang", thanhToan.MaDonHang);
            return View(thanhToan);
        }

        // GET: ThanhToans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "TenKhachHang", thanhToan.MaDonHang);
            return View(thanhToan);
        }

        // POST: ThanhToans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaThanhToan,MaDonHang,PhuongThucThanhToan,NgayThanhToan,SoTienThanhToan,TrangThaiThanhToan")] ThanhToan thanhToan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhToan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "TenKhachHang", thanhToan.MaDonHang);
            return View(thanhToan);
        }

        // GET: ThanhToans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            return View(thanhToan);
        }

        // POST: ThanhToans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            db.ThanhToans.Remove(thanhToan);
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
        // GET: ThanhToans/ThanhToan/5
        // GET: ThanhToans/ThanhToan/5
        [HttpGet]
        public ActionResult ThanhToan(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var donHang = db.DonHangs.FirstOrDefault(d => d.MaDonHang == id.Value);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Hiển thị phương thức thanh toán
            ViewBag.PhuongThucThanhToan = new SelectList(new[] { "Tiền mặt", "VNPay" });
            return View(donHang);
        }

        // POST: ThanhToans/ThanhToan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(int MaDonHang, string phuongThucThanhToan)
        {
            var donHang = db.DonHangs.FirstOrDefault(d => d.MaDonHang == MaDonHang);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Giả sử thanh toán thành công
            // Lưu thông tin thanh toán vào cơ sở dữ liệu
            ThanhToan thanhToan = new ThanhToan
            {
                MaDonHang = MaDonHang,
                PhuongThucThanhToan = phuongThucThanhToan,
                SoTienThanhToan = donHang.TongTien ?? 0,
                TrangThaiThanhToan = "Đã thanh toán",
                NgayThanhToan = DateTime.Now
            };

            db.ThanhToans.Add(thanhToan);
            donHang.TrangThai = "Đã thanh toán";
            db.SaveChanges();

            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Đặt hàng thành công! Bạn sẽ được chuyển đến bước thanh toán.";

            return RedirectToAction("ThanhToan", new { MaDonHang = MaDonHang });
        }

        // Action để hiển thị mã QR cho thanh toán VNPay
        public ActionResult QRCode(int MaDonHang)
        {
            var donHang = db.DonHangs.FirstOrDefault(d => d.MaDonHang == MaDonHang);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            ViewBag.TongTien = donHang.TongTien ?? 0;
            return View();
        }
        public ActionResult GenerateQRCode(decimal amount)
        {
            // Giả lập URL thanh toán VNPay (cần thay thế bằng URL thật khi tích hợp VNPay)
            string vnpayUrl = $"https://vnpay.vn/payment?amount={amount}";

            // Sử dụng thư viện QRCoder để tạo mã QR từ URL thanh toán
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(vnpayUrl, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            return File(ms.ToArray(), "image/png");
                        }
                    }
                }
            }

        }
    }
}
