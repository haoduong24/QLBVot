using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using QLBVot.Models;

namespace QLBVot.Controllers
{
    public class DangnhapController : Controller
    {
        private DVHVOTEntities db = new DVHVOTEntities();

        // GET: Dangnhap
        public ActionResult Index()
        {
            return View();
        }

        // POST: Dangnhap/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.TaiKhoans.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập không đúng.");
                return View("Index");
            }

            // Kiểm tra mật khẩu đã mã hóa
            if (user.Password == HashPassword(password))
            {
                // Lưu thông tin người dùng vào session nếu đăng nhập thành công
                Session["UserID"] = user.UserID;
                Session["Username"] = user.Username;

                return RedirectToAction("Index", "Home"); // Redirect to homepage after successful login
            }
            else
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");
                return View("Index");
            }
        }

        // Hàm mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert password to byte array and compute hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Dangnhap"); // Redirect to login page after logout
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
