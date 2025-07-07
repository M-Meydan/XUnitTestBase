using XUnitTestBase.UnitTest.Helpers;

namespace XUnitTestBase.UnitTest;

// Test double used in these tests
public class AuditLogger : IAuditLogger
{
    public List<(string EventName, string Data)> LoggedEvents { get; } = new();

    public void LogEvent(string eventName, string data)
    {
        LoggedEvents.Add((eventName, data));
    }
}

