using Mobile_App_Develop.Models;

namespace Mobile_App_Develop.Services
{
    public interface IBusService
    {
        Task<List<Bus>> GetAllBusesAsync();
        Task<Bus?> GetBusByIdAsync(int busId);
        Task<List<Bus>> GetBusesByRouteAsync(int routeId);
        Task<List<Route>> GetAllRoutesAsync();
        Task<Route?> GetRouteByIdAsync(int routeId);
        Task<List<BusStop>> GetStopsByRouteAsync(int routeId);
        Task<DateTime?> GetEstimatedArrivalAsync(int busId, int stopId);
        Task<bool> UpdateBusLocationAsync(int busId, double latitude, double longitude);
        event EventHandler<BusLocationEventArgs> BusLocationUpdated;
        event EventHandler<BusStatusEventArgs> BusStatusChanged;
    }
}