using System;

namespace WebQLDASinhVien.Models
{
    public class Project
    {
        public int Id { get; set; } // Khóa chính
        public string? Title { get; set; } // Tên đồ án
        public string? Description { get; set; } // Mô tả đồ án
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc

        // Tiến độ làm đồ án (0 - 100%)
        public int Progress { get; set; }

        // Phản hồi của sinh viên (các khó khăn, thắc mắc)
        public string? Feedback { get; set; }

        // Đường dẫn file: file Word được upload hoặc link Google Doc
        public string? FilePath { get; set; }

        // Liên kết với sinh viên
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Liên kết với giáo viên hướng dẫn
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
