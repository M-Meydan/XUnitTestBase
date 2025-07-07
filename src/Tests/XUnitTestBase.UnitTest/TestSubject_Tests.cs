using Moq;
using Soenneker.Utils.AutoBogus;
using Xunit;
using XUnitTestBase.UnitTest.Helpers;

namespace XUnitTestBase.UnitTest;

public partial class TestSubject_Tests : TestSubject<UserService>
{
    [Fact]
    public void Subject_IsSingleton_WithinTest()
    {
        var first = Subject;
        var second = Subject;

        Assert.Same(first, second); // Reference equality
    }


    [Fact]
    public void Subject_CreatesInstance_WithoutExplicitMockAccess()
    {
        // Act
        var subject = Subject;

        // Assert
        Assert.NotNull(subject);
        Assert.IsType<UserService>(subject);
    }

    [Fact]
    public async Task Subject_InjectsAndResolves_MultipleDependencies()
    {
        var user = new AutoFaker<User>().Generate();

        MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

        var result = await Subject.Register(user);

        Assert.Equal(user.Email, result.Email);
    }

}

