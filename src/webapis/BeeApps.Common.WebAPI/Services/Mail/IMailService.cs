using BeeApps.Common.Models;

namespace BeeApps.Common.Services;

public interface IMailService
{
    public Task Send(Mail mail);
}