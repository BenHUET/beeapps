namespace BeeApps.Common.Exceptions;

public class NotUniqueInDatabaseException : Exception
{
    public NotUniqueInDatabaseException(string field) : base($"{field} already exists in database")
    {
        Field = field;
    }

    public string Field { get; }
}