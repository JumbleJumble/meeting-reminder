using System;
using System.IO;
using Newtonsoft.Json;

namespace MeetingReminder.Services;

public class Config
{
    public string SecretsLocation { get; set; } = null!;
    public int[] AlertMinutesBefore { get; set; } = Array.Empty<int>();
}

public interface IConfigProvider
{
    Config Config { get; }
}

public class ConfigProvider
    : IConfigProvider
{
    private readonly Config _cachedConfig;

    public ConfigProvider()
    {
        var json = File.ReadAllText("config.json");
        _cachedConfig = JsonConvert.DeserializeObject<Config>(json)!;
    }

    public Config Config => _cachedConfig;
}
