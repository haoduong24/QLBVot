using QLBVot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLBVot.Controllers
{
    public class GioHangController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // Lấy hoặc tạo mới giỏ hàng từ Session
        private List<GioHang> LayGioHang()
        {
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult ThemVot(int maVot, string tenVot, decimal giaBan, int soLuong = 1)
        {
            var gioHang = LayGioHang();

            // Kiểm tra sản phẩm đã có trong giỏ hàng hay chưa
            var item = gioHang.FirstOrDefault(v => v.MaVot == maVot);
            if (item == null)
            {
                // Nếu chưa có thì thêm mới sản phẩm vào giỏ hàng
                gioHang.Add(new GioHang
                {
                    MaVot = maVot,
                    TenVot = tenVot,
                    GiaBan = giaBan,
                    SoLuong = soLuong
                });
            }
            else
            {
                // Nếu đã có, tăng số lượng
                item.SoLuong += soLuong;
            }

            ViewBag.CartItemCount = gioHang.Sum(g => g.SoLuong);
            return RedirectToAction("XemGioHang");
        }


        // Hiển thị giỏ hàng
        public ActionResult XemGioHang()
        {
            var gioHang = LayGioHang();
            ViewBag.TotalAmount = gioHang.Sum(item => item.ThanhTien); // Tổng tiền giỏ hàng
            return View(gioHang);
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public ActionResult CapNhat(int maVot, string action)
        {
            var gioHang = LayGioHang();

            if (gioHang != null)
            {
                var item = gioHang.FirstOrDefault(g => g.MaVot == maVot);

                if (item != null)
                {
                    // Tăng hoặc giảm số lượng dựa trên action
                    if (action == "increase")
                    {
                        item.SoLuong += 1; // Tăng số lượng
                    }
                    else if (action == "decrease" && item.SoLuong > 1)
                    {
                        item.SoLuong -= 1; // Giảm số lượng nhưng không nhỏ hơn 1
                    }
                }
            }

            // Lưu giỏ hàng vào session
            Session["GioHang"] = gioHang;

            // Trả về kết quả JSON
            return Json(new { success = true });
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public ActionResult XoaSanPham(int maVot)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(v => v.MaVot == maVot);
            if (item != null)
            {
                gioHang.Remove(item);
            }
            return RedirectToAction("XemGioHang");
        }

        // Đặt hàng
        [HttpPost]
        public ActionResult DatHang(string tenKhachHang)
        {
            var gioHang = LayGioHang();
            if (gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("XemGioHang");
            }

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                NgayDatHang = DateTime.Now,
                TenKhachHang = tenKhachHang,
                TrangThai = "Đang xử lý",
                TongTien = gioHang.Sum(item => item.ThanhTien)  // Tính tổng tiền từ thanh tiền của từng sản phẩm
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            db.DonHangs.Add(donHang);
            db.SaveChanges();

            // Tạo các chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaVot = item.MaVot.ToString(),
                    SoLuong = item.SoLuong,
                    DonGia = item.GiaBan,
                    ThanhTien = item.ThanhTien  // ThanhTien sẽ tính tự động
                };
                db.ChiTietDonHangs.Add(chiTiet);
            }
            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["GioHang"] = null;
            TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công!";

            return RedirectToAction("XemGioHang");
        }
    }
}

