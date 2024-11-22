namespace BeeApps.Common.WebAPI.Options;

public class AuthOptions
{
    public string ValidateTokenURL { get; set; }
    public string ValidateEmailURL { get; set; }
    public bool RequiresValidation { get; set; }
}