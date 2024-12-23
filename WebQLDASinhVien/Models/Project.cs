namespace WebQLDASinhVien.Models
{
    public class Project
    {
        public int Id { get; set; } // Khóa chính
        public string? Title { get; set; } // Tên đồ án
        public string? Description { get; set; } // Mô tả đồ án
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc

        // Liên kết với sinh viên
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Liên kết với giáo viên
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Lưu trữ đường dẫn file đồ án (file Word hoặc ZIP)
        public string? FilePath { get; set; } // Đường dẫn file
    }
}
