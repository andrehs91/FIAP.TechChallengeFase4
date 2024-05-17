namespace TechChallenge.Infraestrutura.Configurations;

public interface IAppSettings
{
    public string GetValue(string variable);
}
