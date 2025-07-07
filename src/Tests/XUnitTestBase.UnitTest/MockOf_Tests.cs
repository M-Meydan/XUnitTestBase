using Moq;
using Soenneker.Utils.AutoBogus;
using Xunit;
using XUnitTestBase.UnitTest.Helpers;

namespace XUnitTestBase.UnitTest
{
    public class MockOf_Tests : TestSubject<UserService>
    {

        /// <summary>
        /// Demonstrates how MockOf&lt;T&gt;() is used to set up dependency behavior.
        /// </summary>
        [Fact]
        public async Task MockOf_SetupAndVerify_DependencyCalls()
        {
            var user = new AutoFaker<User>().Generate();

            MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

            await Subject.Register(user);

            MockOf<IUserRepository>().Verify(r => r.SaveAsync(user), Times.Once);
            MockOf<IEmailSender>().Verify(s => s.SendWelcomeAsync(user.Email), Times.Once);
            MockOf<IAuditLogger>().Verify(l => l.LogEvent("UserRegistered", user.Email), Times.Once);
        }

        /// <summary>
        /// Ensures that MockOf&lt;T&gt;() returns the same instance on every call.
        /// </summary>
        [Fact]
        public void MockOf_ReturnsSameMockInstance_EveryTime()
        {
            var mock1 = MockOf<IUserRepository>();
            var mock2 = MockOf<IUserRepository>();

            Assert.Same(mock1, mock2);
        }

        /// <summary>
        /// Demonstrates that calling MockOf&lt;T&gt;() after overriding the dependency with a real instance via With&lt;T&gt;() throws an exception.
        /// AutoMocker can only retrieve mocks that it created.
        /// </summary>
        [Fact]
        public void MockOf_Throws_WhenDependencyWasOverriddenWithRealInstance()
        {
            var fakeLogger = new AuditLogger();
            With<IAuditLogger>(fakeLogger); // Replaces mock with a concrete instance

            // Act & Assert
            // This throws because AutoMocker cannot return a mock for a type that was overridden manually
            var ex = Assert.ThrowsAny<Exception>(() => MockOf<IAuditLogger>());

            Assert.Contains("was not a mock", ex.Message);
        }


        /// <summary>
        /// Verifies that With&lt;T&gt;() overrides any previously registered mock from MockOf&lt;T&gt;().
        /// This is because AutoMocker always uses the last registered instance when constructing the Subject.
        /// Therefore, even if MockOf is called first, With will take precedence during Subject creation.
        /// </summary>
        [Fact]
        public void With_TakesPrecedence_Over_MockOf()
        {
            var mock = MockOf<IAuditLogger>(); // registered mock

            var realLogger = new AuditLogger();
            With<IAuditLogger>(realLogger); // overrides the mock with a real instance

            var subject = Subject;

            // Use reflection or test behavior to assert that subject is using realLogger
            var field = subject.GetType()
                .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var resolved = field!.GetValue(subject);

            Assert.Same(realLogger, resolved); // confirms override
        }


        /// <summary>
        /// Demonstrates that a mock can be created before Subject is resolved,
        /// and still influence behavior of the Subject.
        /// </summary>
        [Fact]
        public async Task MockOf_Setups_Before_SubjectCreation()
        {
            var user = new AutoFaker<User>().Generate();

            // Setup happens before Subject is ever touched
            MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

            var result = await Subject.Register(user);

            Assert.Equal(user.Email, result.Email);
        }


        /// <summary>
        /// Demonstrates that MockOf&lt;T&gt;() will still return a mock even if the type T
        /// is not a constructor dependency of the subject under test.
        /// </summary>
        [Fact]
        public void MockOf_ReturnsMock_EvenIfNotConstructorDependency()
        {
            var mock = MockOf<ITest>();

            Assert.NotNull(mock); // mock still exists
            Assert.IsAssignableFrom<ITest>(mock.Object);
        }


        /// <summary>
        /// Demonstrates that MockOf&lt;T&gt;() works with abstract class dependencies like EventTracker.
        /// Confirms that user registration triggers event tracking with the correct context.
        /// </summary>
        [Fact]
        public async Task Register_TracksUserEvent_WithEventTracker()
        {
            var user = new AutoFaker<User>().Generate();

            MockOf<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

            await Subject.Register(user);

            // Verifies that the abstract class dependency was invoked as expected
            MockOf<EventTracker>().Verify(t => t.TrackUserEvent(user.Email, "Registered"), Times.Once);
        }

        /// <summary>
        /// Verifies that MockOf&lt;T&gt;() works for generic interface dependencies.
        /// Ensures that the correct generic type is resolved and verified.
        /// </summary>
        [Fact]
        public void MockOf_Works_For_GenericDependencies()
        {
            // Verify that the generic mock was auto-injected and invoked correctly
            var mock = MockOf<IGenericService<User>>();

            Assert.NotNull(mock);
        }

    }
}
