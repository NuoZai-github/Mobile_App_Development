namespace Mobile_App_Develop.Models
{
    public class Notification
    {
        public string Id { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; } = NotificationType.Info;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsRead { get; set; } = false;
        
        public int? BusId { get; set; }
        
        public int? RouteId { get; set; }
        
        public string IconName { get; set; } = "info_circle.png";
        
        public string ActionUrl { get; set; } = string.Empty;
        
        public DateTime? ScheduledFor { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
    
    public enum NotificationType
    {
        Info,
        Warning,
        Alert,
        BusArrival,
        RouteUpdate,
        ServiceDisruption,
        Reminder,
        BusAlert,
        General,
        Arrival
    }
}