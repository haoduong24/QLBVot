using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using QLBVot.Models;

namespace QLBVot.Controllers
{
    public class DangkyController : Controller
    {
        private DVHVOTEntities db = new DVHVOTEntities();

        // GET: Dangky
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dangky/Create (Đăng ký)
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dangky/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Username,Password,Email")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu với SHA256
                taiKhoan.Password = HashPassword(taiKhoan.Password);

                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();
                return RedirectToAction("Index", "Dangnhap"); // Redirect to login page after successful registration
            }

            return View(taiKhoan);
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
