using Xunit;
using XUnitTestBase.UnitTest.Helpers;

namespace XUnitTestBase.UnitTest;

public class BrokenService_Tests : TestSubject<BrokenService>
{
    /// <summary>
    /// AutoMocker cannot resolve value types like Guid,
    /// because they are not interfaces or classes that can be mocked.
    /// To work around this, manually provide the value using With&lt;T&gt;().
    /// </summary>
    [Fact]
    public void Subject_Throws_WhenConstructorHas_UnmockableValue()
    {
        // Arrange & Act & Assert
        var ex = Assert.ThrowsAny<Exception>(() =>
        {
            var _ = Subject; // Constructor requires a Guid, which AutoMocker cannot inject
        });
    }


    /// <summary>
    /// Demonstrates how to fix unmockable value-type injection using With&lt;T&gt;().
    /// AutoMocker cannot resolve value types like Guid by default, so we explicitly inject them.
    /// This ensures the constructor of the subject receives the correct value.
    /// </summary>
    [Fact]
    public void Subject_CreatesInstance_WhenUnmockableValueIsManuallyInjected()
    {
        var id = Guid.NewGuid();

        // Workaround: Inject the value type manually since AutoMocker can't mock Guid
        With<Guid>(id);

        var instance = Subject;

        Assert.NotNull(instance);
        Assert.Equal(id, instance.Id); // Validate that the injected value was used
    }
}

