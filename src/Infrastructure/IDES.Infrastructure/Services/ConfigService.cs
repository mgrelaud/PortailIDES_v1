using IDES.Application.Interfaces;
using IDES.Domain;
using System;
using System.IO;
using System.Text.Json;

namespace IDES.Infrastructure.Services;

public class ConfigService : IConfigService
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "QuantitatifBeton",
        "config.json"
    );

    public AppConfig Config { get; private set; }

    public ConfigService()
    {
        Config = ChargerConfig();
    }

    private AppConfig ChargerConfig()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                string json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
        }
        catch
        {
            // En cas d'erreur, retourner config par défaut
        }

        return new AppConfig();
    }

    public void SauvegarderConfig(AppConfig config)
    {
        try
        {
            string? directory = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(ConfigPath, json);
            Config = config;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors de la sauvegarde de la configuration : {ex.Message}");
        }
    }
}
