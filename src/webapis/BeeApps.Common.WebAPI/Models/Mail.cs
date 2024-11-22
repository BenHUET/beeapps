namespace BeeApps.Common.Models;

public record Mail(
    string ToAddress,
    string ToName,
    string Subject,
    string Body
);