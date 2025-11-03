namespace Mobile_App_Develop.Models
{
    public class Bus
    {
        public int Id { get; set; }
        
        public string BusNumber { get; set; } = string.Empty;
        
        public string DriverName { get; set; } = string.Empty;
        
        public int RouteId { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public BusStatus Status { get; set; } = BusStatus.Stopped;
        
        public int Capacity { get; set; } = 50;
        
        public int CurrentPassengers { get; set; } = 0;
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        
        public double Speed { get; set; } = 0.0;
        
        public string Direction { get; set; } = string.Empty;
        
        public DateTime? EstimatedArrival { get; set; }
        
        public string NextStop { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public double OccupancyPercentage => Capacity > 0 ? (double)CurrentPassengers / Capacity * 100 : 0;
    }
    
    public enum BusStatus
    {
        Stopped,
        Moving,
        AtStop,
        OutOfService,
        Maintenance,
        InService,
        Delayed
    }
}