using Microsoft.Extensions.Configuration;

namespace TechChallenge.Infraestrutura.Settings;

public class AppSettings(IConfiguration configuration) : IAppSettings
{
    private readonly IConfiguration _configuration = configuration;

    public string GetValue(string variable)
    {
        string environmentVariable = variable.Replace(":", "_").ToUpper();
        var value = Environment.GetEnvironmentVariable(environmentVariable);

        if (string.IsNullOrWhiteSpace(value))
            value = _configuration[variable];

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(variable, "Variável de ambiente não configurada.");

        return value;
    }
}
