# XUnitTestBase

[![NuGet](https://img.shields.io/nuget/v/XUnitTestBase.svg)](https://www.nuget.org/packages/XUnitTestBase/)
[![License](https://img.shields.io/github/license/M-Meydan/XUnitTestBase)](https://github.com/M-Meydan/XUnitTestBase/blob/main/LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/M-Meydan/XUnitTestBase/build.yml?branch=main)](https://github.com/M-Meydan/XUnitTestBase/actions/workflows/build.yml)


**XUnitTestBase v1.0** — A clean, extensible test foundation for .NET with auto-mocking, integration testing, and developer-friendly conventions.

---

## 🎯 Purpose

**XUnitTestBase** provides a standardized and developer-friendly test foundation for .NET applications using:

- 🧪 xUnit
- 🤖 Moq with AutoMocker
- 🧱 WebApplicationFactory<T> for integration testing

It helps teams write cleaner, faster, and more maintainable unit and integration tests with minimal setup.

---

## ✨ Key Features

- ✅ TestSubject<T>: auto-mocked unit test base class using Moq.AutoMocker
- ✅ MockOf<T>(): get & verify dependency mocks
- ✅ With<T>(): override mocks or inject custom test doubles
- ✅ IntegrationTestBase<T>: lightweight base for API integration tests
- ✅ Support for WebApplicationFactory, InMemory EF Core, and Fake Auth

---

## 🔁 Integration Testing

Use IntegrationTestBase<T> to verify your app's full stack:

csharp
public class UserApiTests : IntegrationTestBase<Program>
{
    public UserApiTests() : base(services =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
    }) { }

    [Fact]
    public async Task GetUser_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/users/1");
        response.EnsureSuccessStatusCode();
    }
}



---

## 🧪 Unit Testing

Extend TestSubject<T> for clean unit tests:

csharp
public class UserServiceTests : TestSubject<UserService>
{
    [Fact]
    public async Task Register_CallsAllDependencies()
    {
        var user = FakeBuilder.Create<User>();

        MockOf<IUserRepository>()
            .Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync((User?)null);

        await Subject.Register(user);

        MockOf<IEmailSender>().Verify(s => s.SendWelcomeAsync(user.Email), Times.Once);
        MockOf<IAuditLogger>().Verify(l => l.LogEvent("UserRegistered", user.Email), Times.Once);
    }
}



---

## 💡 Example Highlights

- 🧪 Auto-mocking multiple dependencies
- 🔁 Overriding mocks with real/fake implementations
- 🚀 End-to-end HTTP testing with in-memory HttpClient

---
