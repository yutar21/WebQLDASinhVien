using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQLDASinhVien.Models
{
    public class Teacher
    {
        public int Id { get; set; } // Khóa chính

        [Display(Name = "Họ và Tên")]
        public string? FullName { get; set; } // Họ và tên

        [Display(Name = "Email")]
        public string? Email { get; set; } // Email

        [Display(Name = "Bộ Môn")]
        public string? Department { get; set; } // Bộ môn

        [Display(Name = "Số Điện Thoại")]
        public string? PhoneNumber { get; set; } // Số điện thoại

        [Display(Name = "Ngày Sinh")]
        public DateTime DateOfBirth { get; set; } // Ngày sinh

        [Display(Name = "Địa Chỉ")]
        public string? Address { get; set; } // Địa chỉ

        // Liên kết nhiều đồ án
        public ICollection<Project>? Projects { get; set; }
    }
}
