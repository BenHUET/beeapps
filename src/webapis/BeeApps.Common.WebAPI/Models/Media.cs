using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.Models;

public class Media
{
    [Required] public int Id { get; set; }

    [Required] public string MD5 { get; set; }
}