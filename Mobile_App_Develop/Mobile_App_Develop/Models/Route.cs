namespace Mobile_App_Develop.Models
{
    public class Route
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string Color { get; set; } = "#2196F3";
        
        public List<BusStop> Stops { get; set; } = new List<BusStop>();
        
        public TimeSpan StartTime { get; set; } = new TimeSpan(6, 0, 0);
        
        public TimeSpan EndTime { get; set; } = new TimeSpan(22, 0, 0);
        
        public int FrequencyMinutes { get; set; } = 15;
        
        public bool IsActive { get; set; } = true;
        
        public double TotalDistance { get; set; } = 0.0;
        
        public int EstimatedDurationMinutes { get; set; } = 30;
        
        public List<int> ActiveBusIds { get; set; } = new List<int>();
    }
    
    public class BusStop
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public int Order { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string Description { get; set; } = string.Empty;
        
        public List<string> Amenities { get; set; } = new List<string>();
    }
}