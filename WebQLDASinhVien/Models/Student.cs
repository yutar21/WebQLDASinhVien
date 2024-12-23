using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQLDASinhVien.Models
{
    public class Student
    {
        public int Id { get; set; } // Khóa chính

        [Display(Name = "Mã sinh viên")]
        public string? StudentCode { get; set; } // Mã sinh viên

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; } // Họ và tên

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } // Ngày sinh

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; } // Địa chỉ

        [Display(Name = "Email")]
        public string? Email { get; set; } // Email

        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; } // Số điện thoại

        // Liên kết nhiều đồ án
        public ICollection<Project>? Projects { get; set; }
    }
}
