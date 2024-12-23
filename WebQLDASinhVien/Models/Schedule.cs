namespace WebQLDASinhVien.Models
{
    public class Schedule
    {
        public int Id { get; set; } // Khóa chính
        public DateTime Date { get; set; } // Ngày chấm đồ án
        public TimeSpan StartTime { get; set; } // Giờ bắt đầu
        public TimeSpan EndTime { get; set; } // Giờ kết thúc

        // Liên kết với đồ án
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

}
