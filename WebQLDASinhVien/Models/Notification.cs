namespace WebQLDASinhVien.Models
{
    public class Notification
    {
        public int Id { get; set; } // Khóa chính
        public string Title { get; set; } // Tiêu đề thông báo
        public string Content { get; set; } // Nội dung thông báo
        public DateTime SentDate { get; set; } // Ngày gửi

        // Liên kết với đồ án
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

}
