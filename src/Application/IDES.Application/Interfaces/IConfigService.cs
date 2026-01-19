using IDES.Domain;

namespace IDES.Application.Interfaces;

public interface IConfigService
{
    AppConfig Config { get; }
    void SauvegarderConfig(AppConfig config);
}
