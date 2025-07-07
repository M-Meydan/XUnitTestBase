using Moq;
using Soenneker.Utils.AutoBogus;
using Xunit;
using XUnitTestBase.UnitTest.Helpers;

namespace XUnitTestBase.UnitTest;

public partial class With_Tests : TestSubject<UserService>
{
    /// <summary>
    /// Demonstrates overriding an auto-mocked dependency with a custom implementation.
    /// </summary>
    [Fact]
    public async Task With_OverridesMockedDependency_WithCustomInstance()
    {
        var logger = new AuditLogger();
        With<IAuditLogger>(logger);

        var user = new AutoFaker<User>().Generate();
        MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

        await Subject.Register(user);

        Assert.Single(logger.LoggedEvents);
        Assert.Equal("UserRegistered", logger.LoggedEvents[0].EventName);
    }

    /// <summary>
    /// Ensures that once Subject is created, calling With<T>() afterward has no effect on its already-injected dependencies.
    /// </summary>
    [Fact]
    public void With_DoesNotAffect_WhenSubjectAlreadyCreated()
    {
        var original = Subject; // Subject dependencies already resolved

        var newLogger = new AuditLogger();
        With<IAuditLogger>(newLogger); //Dependency happens too late

        var after = Subject;

        Assert.Same(original, after); // Confirm it's the same instance
    }

    /// <summary>
    /// Ensures multiple With<T>() calls can override multiple dependencies.
    /// </summary>
    [Fact]
    public async Task With_Injects_MultipleCustomDependencies()
    {
        var user = new AutoFaker<User>().Generate();
       
        var logger = new AuditLogger();
        var emailSender = new Mock<IEmailSender>();

        With<IAuditLogger>(logger);
        With<IEmailSender>(emailSender.Object);
        
        MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

        await Subject.Register(user);

        Assert.Single(logger.LoggedEvents);
        emailSender.Verify(s => s.SendWelcomeAsync(user.Email), Times.Once);
    }

    /// <summary>
    /// Demonstrates that when overriding the same interface multiple times, the last override takes precedence.
    /// </summary>
    [Fact]
    public async Task With_LastOverride_TakesPrecedence()
    {
        var user = new AutoFaker<User>().Generate();

        var firstLogger = new AuditLogger();
        var secondLogger = new AuditLogger();

        With<IAuditLogger>(firstLogger);
        With<IAuditLogger>(secondLogger); // This should take precedence

        MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

        await Subject.Register(user);

        Assert.Empty(firstLogger.LoggedEvents);
        Assert.Single(secondLogger.LoggedEvents);
        Assert.Equal("UserRegistered", secondLogger.LoggedEvents[0].EventName);
    }

    /// <summary>
    /// Ensures that passing null to With<T>() throws an ArgumentNullException.
    /// </summary>
    [Fact]
    public void With_ThrowsArgumentNullException_WhenNullIsPassed()
    {
        Assert.Throws<ArgumentNullException>(() =>
            With<IAuditLogger>(null!)); // Using null-forgiving operator to bypass compile check
    }
}

