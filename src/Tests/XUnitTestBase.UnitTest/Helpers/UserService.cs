using Castle.Core.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestBase.UnitTest.Helpers;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task SaveAsync(User user);
    Task GetByEmailAsync(object email);
    void Save(User user);
}

public interface IEmailSender
{
    Task SendWelcomeAsync(string email);
}

public interface IAuditLogger
{
    void LogEvent(string action, string email);
}

public interface ITest
{

}

public class UserService
{
    private readonly IUserRepository _repo;
    private readonly IEmailSender _emailSender;
    private readonly IAuditLogger _logger;
    private readonly EventTracker _eventTracker;

    public UserService(IUserRepository repo, IEmailSender emailSender, IAuditLogger logger, EventTracker eventTracker)
    {
        _repo = repo;
        _emailSender = emailSender;
        _logger = logger;
        _eventTracker = eventTracker;
    }

    public async Task<User> Register(User user)
    {
        if (await _repo.GetByEmailAsync(user.Email) is not null)
            throw new InvalidOperationException("User already exists");

        await _repo.SaveAsync(user);
        await _emailSender.SendWelcomeAsync(user.Email);

        _logger.LogEvent("UserRegistered", user.Email);
        _eventTracker.TrackUserEvent(user.Email, "Registered");

        return user;
    }
}
