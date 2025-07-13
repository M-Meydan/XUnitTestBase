namespace XUnitTestBase.TestApi.Services;
public interface ITimeService
{
    string GetCurrentTime();
}

public class RealTimeService : ITimeService
{
    public string GetCurrentTime() => DateTime.UtcNow.ToString("O");
}

