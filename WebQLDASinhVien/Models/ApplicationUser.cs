using Microsoft.AspNetCore.Identity;

namespace WebQLDASinhVien.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } // Họ và tên người dùng
    }

}
