![NuGet](https://img.shields.io/nuget/v/XUnitTestBase.svg)
![License](https://img.shields.io/github/license/muhsinmeydan/XUnitTestBase.svg)
![Build](https://img.shields.io/github/actions/workflow/status/muhsinmeydan/XUnitTestBase/build.yml?branch=main)

# XUnitTestBase

**XUnitTestBase v1.0** — A clean, extensible test foundation for .NET with auto-mocking, integration testing, and developer-friendly conventions.

---

## 📁 Table of Contents

- [Version](#-version)
- [Purpose](#-purpose)
- [Key Features](#-key-features)
- [Integration Testing](#-integration-testing)
- [Unit Testing](#-unit-testing)
- [Example Highlights](#-example-highlights)
- [Author](#-author)
- [License](#-license)

---

## 🔚 Version

**Latest:** `v1.0`\
**Status:** Actively maintained ✅

---

## 🎯 Purpose

**XUnitTestBase** provides a standardized and developer-friendly test foundation for .NET applications using:

- 💢 `xUnit`
- 🤖 `Moq` with `AutoMocker`
- 🧱 `WebApplicationFactory<T>` for integration testing

It enables teams to write cleaner, faster, and more maintainable unit and integration tests with minimal setup.

---

## ✨ Key Features

- ✅ `TestSubject<T>`: auto-mocked unit test base class using `Moq.AutoMocker`
- ✅ `MockOf<T>()`: access and verify dependency mocks
- ✅ `With<T>()`: override mocks or inject custom test doubles
- ✅ `IntegrationTestBase<T>`: lightweight base class for API integration tests
- ✅ Supports `WebApplicationFactory`, `InMemory EF Core`, and `Fake Auth`
- ✅ Compatible with `AutoBogus` for test data generation

---

## 🔁 Integration Testing

Use `IntegrationTestBase<T>` to verify your app's full stack:

```csharp
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
```

---

## 🧪 Unit Testing

Extend `TestSubject<T>` for clean and isolated unit tests:

```csharp
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
```

---

## 💡 Example Highlights

- 💢 Auto-mocking for multiple dependencies
- 🔁 Override mocks with real or custom fakes
- 📆 Test data generation using `AutoBogus`
- 🚀 End-to-end HTTP testing with in-memory `HttpClient`

---

## 👤 Author

Built for practical, scalable test development. Contributions and suggestions are welcome.\
Created by **Muhsin Meydan**

---

## 🛠 License
MIT License — free for commercial and open-source u
se.
