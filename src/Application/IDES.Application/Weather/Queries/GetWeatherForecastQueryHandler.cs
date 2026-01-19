using IDES.Application.Interfaces;
using IDES.Domain.Entities;

namespace IDES.Application.Weather.Queries;

public class GetWeatherForecastQueryHandler
{
    private readonly IWeatherForecastService _weatherForecastService;

    public GetWeatherForecastQueryHandler(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    public async Task<WeatherForecast[]> Handle(DateTime startDate)
    {
        return await _weatherForecastService.GetForecastAsync(startDate);
    }
}
