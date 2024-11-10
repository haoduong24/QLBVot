using System.Web.Mvc;

namespace QLBVot.Areas.User // Đổi tên khu vực Admin thành User
{
    public class UserAreaRegistration : AreaRegistration // Cập nhật tên class để trùng với tên khu vực User
    {
        public override string AreaName
        {
            get
            {
                return "User";  // Đổi tên khu vực thành User
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "User_default", // Đổi tên route thành User_default
                "Admin/{controller}/{action}/{id}",  // Đổi tên URL thành /Admin để quản lý người dùng
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
