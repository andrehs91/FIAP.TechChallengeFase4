﻿namespace TechChallenge.Worker.Configurations;

public class AppSettings(IConfiguration configuration) : IAppSettings
{
    private readonly IConfiguration _configuration = configuration;

    public string GetValue(string variable)
    {
        string environmentVariable = variable.Replace(":", "_").ToUpper();
        var value = Environment.GetEnvironmentVariable(environmentVariable);
        value ??= _configuration[variable];

        if (value is null)
            throw new ArgumentNullException(variable, "Variável de ambiente não configurada.");

        return value;
    }
}
