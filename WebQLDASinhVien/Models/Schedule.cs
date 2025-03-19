namespace WebQLDASinhVien.Models
{
    public class Schedule
    {
        public int Id { get; set; } // Khóa chính
        public DateTime ScheduledDate { get; set; } // Ngày chấm bảo vệ
        public string TimeSlot { get; set; }         // Thời gian (ví dụ: "09:00 - 11:00")
        public string Venue { get; set; }            // Phòng, địa điểm chấm

        // Liên kết với đồ án
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

}
