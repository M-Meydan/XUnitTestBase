namespace XUnitTestBase.UnitTest.Helpers;

public class BrokenService
{
    public Guid Id { get; set;  }
    public BrokenService(Guid guid) { Id = guid; } // not mockable
}
