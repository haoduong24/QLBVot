// DangnhapController.cs
using QLBVot.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLBVot.Controllers
{
    public class DangnhapController : Controller
    {
        private DVHVOTEntities7 db = new DVHVOTEntities7();

        // Action GET: Hiển thị form đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Action POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập và mật khẩu trong cơ sở dữ liệu
                var user = db.TAIKHOANs.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Lưu thông tin người dùng vào session sau khi đăng nhập thành công
                    Session["User"] = user;

                    // Đăng nhập thành công, chuyển hướng tới Vots/Index
                    return RedirectToAction("Index", "Vots");
                }
                else
                {
                    // Đăng nhập thất bại
                    ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                }
            }
            return View(model);
        }


        // Action GET: Hiển thị form đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Action POST: Xử lý đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                var existingUser = db.TAIKHOANs.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Thêm người dùng mới vào cơ sở dữ liệu
                db.TAIKHOANs.Add(model);
                db.SaveChanges();

                // Thông báo đăng ký thành công
                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            return View(model);
        }
        public ActionResult Logout()
        {
            // Clear the session or any authentication cookies if set
            Session.Clear(); // Clears all session data
            Session.Abandon(); // Abandon the session

            // Redirect to the login page or home page after logging out
            return RedirectToAction("Login", "Dangnhap");
        }
    }
}
