using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WebQLDASinhVien.Data;
using Microsoft.EntityFrameworkCore;

namespace WebQLDASinhVien.Services
{
    public class NotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IServiceProvider serviceProvider, ILogger<NotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNotifications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notifications.");
                }
                // Kiểm tra mỗi 5 phút (có thể điều chỉnh)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("Notification Service is stopping.");
        }

        private async Task ProcessNotifications(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WebQLDASinhVienContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // Ví dụ: gửi email nhắc nhở cho sinh viên nếu hạn nộp đồ án sắp đến (2 ngày nữa)
                var upcomingProjects = await context.Projects
                    .Include(p => p.Student)
                    .Where(p => p.EndDate.Date == DateTime.Now.AddDays(2).Date)
                    .ToListAsync(stoppingToken);

                foreach (var project in upcomingProjects)
                {
                    string subject = "Nhắc nhở: Hạn nộp đồ án sắp đến";
                    string message = $"Chào {project.Student.FullName},<br/>" +
                                     $"Đề tài '{project.Title}' của bạn có hạn nộp vào ngày {project.EndDate:dd/MM/yyyy}.<br/>" +
                                     "Vui lòng hoàn thành và nộp báo cáo đúng hạn.<br/><br/>Trân trọng.";

                    await emailService.SendEmailAsync(project.Student.Email, subject, message);
                }

                // Các thông báo khác (ví dụ: lịch chấm sắp diễn ra) có thể được thêm ở đây.
            }
        }
    }
}
