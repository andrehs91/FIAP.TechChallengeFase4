namespace TechChallenge.Infraestrutura.Settings;

public interface IAppSettings
{
    public string GetValue(string variable);
}
