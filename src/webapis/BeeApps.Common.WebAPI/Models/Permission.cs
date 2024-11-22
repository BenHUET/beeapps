using BeeApps.Common.Enumerations;

namespace BeeApps.Common.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; }

    public static string ToShortInline(IEnumerable<Permission> permissions)
    {
        if (permissions == null)
            return "";

        return string.Join("", permissions.Select(p => p.Id.ToString("00000000")));
    }

    public static IEnumerable<PermissionName> FromShortInline(string inline)
    {
        if (inline.Length % 8 != 0)
            throw new InvalidDataException();

        var permissions = new PermissionName[inline.Length / 8];
        try
        {
            for (var i = 0; i < inline.Length; i += 8)
            {
                var substring = inline.Substring(i, 8);
                permissions[i / 8] = (PermissionName)int.Parse(substring);
            }
        }
        catch
        {
            throw new InvalidDataException();
        }

        return permissions;
    }
}