using IDES.Domain.Entities;

namespace IDES.Application.Interfaces;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
}
