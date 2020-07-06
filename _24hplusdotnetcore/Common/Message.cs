using System;

namespace _24hplusdotnetcore.Common
{
    public static class Message
    {
        public static string SUCCESS = "";
        public static string LOGIN_SUCCESS = "";
        public static string INCORRECT_USERNAME_PASSWORD = "Sai tài khoản hoặc mật khẩu, Vui lòng nhập lại!";
        public static string LOGIN_BIDDEN = "Không thể login, Vui lòng liên hệ IT!";
        public static string IS_LOGGED_IN_ORTHER_DEVICE = "Bạn đã đăng nhập ở một nơi khác, Vui lòng đăng nhập lại!";
        public static string UNAUTHORIZED = "";
        public static string ERROR = "Lỗi hệ thống, Vui lòng liên hệ IT!";
        public static string NOT_FOUND_PRODUCT = "Không tìm thấy sản phẩm!";
        internal static string VERSION_IS_OLD = "Phiên bản của bạn đã cũ, Vui lòng cập nhập phiên bản mới!";
        internal static string NotificationAdd = "{0} vừa thêm mới khách hàng {1}";
        internal static string NotificationUpdate = "{0} vừa cập nhật lại thông tin khách hàng  {1}";
        public static string TeamLeadReject = "{0} vừa từ chối khách hàng {1}";
        public static string TeamLeadApprove = "{0} vừa duyệt thành công khách hàng {1}";
        public const string NOT_FOUND_KIOS = "Không tìm thấy kios";
        public static string CANT_UPDATE_CUSTOMER_ERROR = "Lỗi hệ thống, Vui lòng thử lại!";
    }
    public enum ResponseCode : int
    {
        SUCCESS = 1,
        ERROR,        
        UNAUTHORIZED,
        IS_LOGGED_IN_ORTHER_DEVICE
    }
}