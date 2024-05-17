namespace TechChallenge.Worker.Configurations;

public interface IAppSettings
{
    public string GetValue(string variable);
}
