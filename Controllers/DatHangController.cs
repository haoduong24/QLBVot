using QLBVot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;

namespace QLBVot.Controllers
{
    public class DatHangController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // Hành động để hiển thị thông tin giao hàng (khi nhấn Đặt hàng từ giỏ)
        [HttpPost]
        public ActionResult DatHang()
        {
            // Kiểm tra nếu giỏ hàng rỗng
            var gioHang = (List<GioHang>)Session["GioHang"];
            if (gioHang == null || gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("XemGioHang", "GioHang");
            }

            // Hiển thị form nhập thông tin giao hàng
            return View();
        }

        // Hành động để xác nhận đơn hàng
        [HttpPost]
        public ActionResult XacNhanDatHangString(string tenKhachHang, string diaChi, string soDienThoai, string email)
        {
            // Lấy giỏ hàng từ session
            var gioHang = (List<GioHang>)Session["GioHang"];
            if (gioHang == null || gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("XemGioHang", "GioHang");
            }

            // Tạo đối tượng đơn hàng
            var donHang = new DonHang
            {
                NgayDatHang = DateTime.Now,
                TenKhachHang = tenKhachHang,
                TrangThai = "Đang xử lý", // Trạng thái đơn hàng
                TongTien = gioHang.Sum(item => item.ThanhTien)
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            db.DonHangs.Add(donHang);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaVot = item.MaVot.ToString(),
                    SoLuong = item.SoLuong,
                    DonGia = item.GiaBan
                };

                db.ChiTietDonHangs.Add(chiTietDonHang);
            }
            db.SaveChanges();

            // Nếu cần xử lý thanh toán (nếu có)

            // Có thể thêm logic xử lý thanh toán vào đây

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["GioHang"] = null;

            // Thông báo thành công
            TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công!";

            // Chuyển đến trang xác nhận đơn hàng
            return RedirectToAction("XacNhanDatHang", new { id = donHang.MaDonHang });
        }

        // Hành động để hiển thị trang xác nhận đơn hàng
        // GET: XacNhanDatHang
        public ActionResult XacNhanDatHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var donHang = db.DonHangs.Include(d => d.ChiTietDonHangs).FirstOrDefault(d => d.MaDonHang == id);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Giả sử trạng thái mặc định là "Đã đặt hàng" khi tạo đơn
            donHang.TrangThai = donHang.TrangThai ?? "Đã đặt hàng";

            // Tính tổng tiền từ chi tiết đơn hàng
            donHang.TongTien = donHang.ChiTietDonHangs.Sum(c => c.SoLuong * c.DonGia);

            return View(donHang);
        }


        // POST: XacNhanDatHang
        [HttpPost]
            public ActionResult XacNhanDatHang(DonHang donHang)
            {
                if (ModelState.IsValid)
                {
                    // Xử lý dữ liệu của DonHang
                    db.SaveChanges();
                    return RedirectToAction("ThongBaoXacNhan");
                }

                return View(donHang);
            }
        }
    }

