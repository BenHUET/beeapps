namespace BeeApps.Common.Exceptions;

public class EmailNotValidatedException : Exception
{
    public EmailNotValidatedException() : base("Email address not confirmed")
    {
    }
}