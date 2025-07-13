using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using System;

namespace XUnitTestBase; 

/// <summary>
/// A lightweight test base that provides auto-mocking support for the subject under test.
/// Automatically wires up constructor dependencies using Moq.AutoMocker.
/// 
/// <example>
/// public class OrderServiceTests : TestSubject&lt;OrderService&gt;
/// {
///     [Fact]
///     public void Valid_order_is_approved()
///     {
///         var order = new Order();
///         MockOf&lt;IOrderValidator&gt;().Setup(x =&gt; x.Validate(order)).Returns(true);
///
///         var result = Subject.Submit(order); // Call the system under test (SUT)
///
///         Assert.True(result.IsApproved);
///     }
/// }
/// </example>
/// </summary>
/// <typeparam name="T">The system under test (SUT).</typeparam>
public abstract partial class TestSubject<T> : IDisposable where T : class
{
    private readonly AutoMocker _mocker = new();
    private T? _subject;

    /// <summary>
    /// Lazily creates or returns the cached instance of the subject under test.
    /// </summary>
    /// <example>
    /// var result = Subject.DoSomething();
    /// </example>
    protected T Subject => _subject ??= _mocker.CreateInstance<T>();

    /// <summary>
    /// Retrieves the mock instance of a dependency.
    /// </summary>
    /// <typeparam name="TDependency">The dependency type to retrieve the mock for.</typeparam>
    /// <returns>The mock object of the dependency.</returns>
    /// <example>
    /// MockOf&lt;ILogger&gt;().Verify(x =&gt; x.Log(...), Times.Once);
    /// </example>
    protected Mock<TDependency> MockOf<TDependency>() where TDependency : class
        => _mocker.GetMock<TDependency>();

    /// <summary>
    /// Replaces a dependency with a concrete instance.
    /// </summary>
    /// <typeparam name="TDependency">The type of dependency to override.</typeparam>
    /// <param name="instance">The instance to inject.</param>
    /// <exception cref="ArgumentNullException">Thrown when the instance is null.</exception>
    /// <example>
    /// With&lt;IClock&gt;(new TestClock(DateTime.UtcNow));
    /// </example>
    protected void With<TDependency>(TDependency instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _mocker.Use(instance);
    }

    /// <summary>
    /// Clears the subject reference and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        _subject = null;
        GC.SuppressFinalize(this);
    }
}
