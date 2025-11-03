using Mobile_App_Develop.Models;
using System.Timers;

namespace Mobile_App_Develop.Services
{
    public class BusService : IBusService
    {
        private readonly List<Bus> _buses;
        private readonly List<Route> _routes;
        private readonly System.Timers.Timer _locationUpdateTimer;
        private readonly Random _random;

        public event EventHandler<BusLocationEventArgs>? BusLocationUpdated;
        public event EventHandler<BusStatusEventArgs>? BusStatusChanged;

        public BusService()
        {
            _random = new Random();
            _buses = InitializeBuses();
            _routes = InitializeRoutes();
            
            // 设置定时器来模拟实时位置更新
            _locationUpdateTimer = new System.Timers.Timer(5000); // 每5秒更新一次
            _locationUpdateTimer.Elapsed += OnLocationUpdateTimer;
            _locationUpdateTimer.Start();
        }

        private List<Route> InitializeRoutes()
        {
            return new List<Route>
            {
                new Route
                {
                    Id = 1,
                    Name = "Campus Loop",
                    Description = "Main campus circular route",
                    Color = "#2196F3",
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    FrequencyMinutes = 15,
                    EstimatedDurationMinutes = 25,
                    TotalDistance = 8.5,
                    ActiveBusIds = new List<int> { 1, 2 },
                    Stops = new List<BusStop>
                    {
                        new BusStop { Id = 1, Name = "Central Station", Latitude = -33.8688, Longitude = 151.2093, Order = 1 },
                        new BusStop { Id = 2, Name = "Library", Latitude = -33.8678, Longitude = 151.2103, Order = 2 },
                        new BusStop { Id = 3, Name = "Engineering Building", Latitude = -33.8668, Longitude = 151.2113, Order = 3 },
                        new BusStop { Id = 4, Name = "Student Center", Latitude = -33.8658, Longitude = 151.2123, Order = 4 },
                        new BusStop { Id = 5, Name = "Sports Complex", Latitude = -33.8648, Longitude = 151.2133, Order = 5 }
                    }
                },
                new Route
                {
                    Id = 2,
                    Name = "City Express",
                    Description = "Direct route to city center",
                    Color = "#FF5722",
                    StartTime = new TimeSpan(7, 0, 0),
                    EndTime = new TimeSpan(19, 0, 0),
                    FrequencyMinutes = 20,
                    EstimatedDurationMinutes = 35,
                    TotalDistance = 12.3,
                    ActiveBusIds = new List<int> { 3 },
                    Stops = new List<BusStop>
                    {
                        new BusStop { Id = 6, Name = "UTS Main Gate", Latitude = -33.8838, Longitude = 151.2003, Order = 1 },
                        new BusStop { Id = 7, Name = "Town Hall", Latitude = -33.8728, Longitude = 151.2063, Order = 2 },
                        new BusStop { Id = 8, Name = "Circular Quay", Latitude = -33.8618, Longitude = 151.2113, Order = 3 }
                    }
                }
            };
        }

        private List<Bus> InitializeBuses()
        {
            return new List<Bus>
            {
                new Bus
                {
                    Id = 1,
                    BusNumber = "UTS001",
                    DriverName = "Michael Chen",
                    RouteId = 1,
                    Latitude = -33.8688,
                    Longitude = 151.2093,
                    Status = BusStatus.Moving,
                    Capacity = 50,
                    CurrentPassengers = 23,
                    Speed = 25.5,
                    Direction = "Clockwise",
                    NextStop = "Library",
                    EstimatedArrival = DateTime.Now.AddMinutes(3)
                },
                new Bus
                {
                    Id = 2,
                    BusNumber = "UTS002",
                    DriverName = "Sarah Johnson",
                    RouteId = 1,
                    Latitude = -33.8658,
                    Longitude = 151.2123,
                    Status = BusStatus.AtStop,
                    Capacity = 50,
                    CurrentPassengers = 31,
                    Speed = 0,
                    Direction = "Clockwise",
                    NextStop = "Sports Complex",
                    EstimatedArrival = DateTime.Now.AddMinutes(1)
                },
                new Bus
                {
                    Id = 3,
                    BusNumber = "UTS003",
                    DriverName = "David Wilson",
                    RouteId = 2,
                    Latitude = -33.8838,
                    Longitude = 151.2003,
                    Status = BusStatus.Moving,
                    Capacity = 60,
                    CurrentPassengers = 18,
                    Speed = 35.0,
                    Direction = "To City",
                    NextStop = "Town Hall",
                    EstimatedArrival = DateTime.Now.AddMinutes(8)
                }
            };
        }

        public async Task<List<Bus>> GetAllBusesAsync()
        {
            await Task.Delay(500); // 模拟网络延迟
            return _buses.Where(b => b.IsActive).ToList();
        }

        public async Task<Bus?> GetBusByIdAsync(int busId)
        {
            await Task.Delay(300);
            return _buses.FirstOrDefault(b => b.Id == busId && b.IsActive);
        }

        public async Task<List<Bus>> GetBusesByRouteAsync(int routeId)
        {
            await Task.Delay(400);
            return _buses.Where(b => b.RouteId == routeId && b.IsActive).ToList();
        }

        public async Task<List<Route>> GetAllRoutesAsync()
        {
            await Task.Delay(600);
            return _routes.Where(r => r.IsActive).ToList();
        }

        public async Task<Route?> GetRouteByIdAsync(int routeId)
        {
            await Task.Delay(300);
            return _routes.FirstOrDefault(r => r.Id == routeId && r.IsActive);
        }

        public async Task<List<BusStop>> GetStopsByRouteAsync(int routeId)
        {
            await Task.Delay(400);
            var route = _routes.FirstOrDefault(r => r.Id == routeId);
            return route?.Stops.Where(s => s.IsActive).OrderBy(s => s.Order).ToList() ?? new List<BusStop>();
        }

        public async Task<DateTime?> GetEstimatedArrivalAsync(int busId, int stopId)
        {
            await Task.Delay(300);
            var bus = _buses.FirstOrDefault(b => b.Id == busId);
            if (bus != null)
            {
                // 简单的估算逻辑
                var randomMinutes = _random.Next(1, 15);
                return DateTime.Now.AddMinutes(randomMinutes);
            }
            return null;
        }

        public async Task<bool> UpdateBusLocationAsync(int busId, double latitude, double longitude)
        {
            await Task.Delay(200);
            var bus = _buses.FirstOrDefault(b => b.Id == busId);
            if (bus != null)
            {
                bus.Latitude = latitude;
                bus.Longitude = longitude;
                bus.LastUpdated = DateTime.Now;
                
                BusLocationUpdated?.Invoke(this, new BusLocationEventArgs(bus));
                return true;
            }
            return false;
        }

        private void OnLocationUpdateTimer(object? sender, ElapsedEventArgs e)
        {
            // 模拟巴士位置更新
            foreach (var bus in _buses.Where(b => b.Status == BusStatus.Moving))
            {
                // 随机小幅度移动位置
                var latChange = (_random.NextDouble() - 0.5) * 0.001; // 约100米范围内
                var lngChange = (_random.NextDouble() - 0.5) * 0.001;
                
                bus.Latitude += latChange;
                bus.Longitude += lngChange;
                bus.LastUpdated = DateTime.Now;
                
                // 随机更新速度
                bus.Speed = _random.Next(15, 45);
                
                // 随机更新乘客数量
                if (_random.Next(1, 10) == 1) // 10%概率更新乘客数量
                {
                    var change = _random.Next(-3, 5);
                    bus.CurrentPassengers = Math.Max(0, Math.Min(bus.Capacity, bus.CurrentPassengers + change));
                }
                
                BusLocationUpdated?.Invoke(this, new BusLocationEventArgs(bus));
            }
        }

        public void Dispose()
        {
            _locationUpdateTimer?.Stop();
            _locationUpdateTimer?.Dispose();
        }
    }

    // 事件参数类
    public class BusLocationEventArgs : EventArgs
    {
        public Bus Bus { get; set; }
        public int BusId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public BusLocationEventArgs(Bus bus)
        {
            Bus = bus;
            BusId = bus.Id;
            Latitude = bus.Latitude;
            Longitude = bus.Longitude;
        }
    }

    public class BusStatusEventArgs : EventArgs
    {
        public Bus Bus { get; set; }
        public int BusId { get; set; }
        public BusStatus Status { get; set; }
        
        public BusStatusEventArgs(Bus bus)
        {
            Bus = bus;
            BusId = bus.Id;
            Status = bus.Status;
        }
    }
}