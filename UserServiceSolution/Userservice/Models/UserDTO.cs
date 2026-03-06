using System.ComponentModel.DataAnnotations;

namespace Models;

public class UserDTO
{
    [Required]
    public string givenName { get; set; }
    [Required]
    public string familyName { get; set; }
    [Required]
    public string Address1 { get; set; }
    public string? Address2 { get; set; }
    public short PostalCode { get; set; }
    [Required]
    public string faxNumber { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string email { get; set; }
    [Required]
    public string telephone { get; set; }
}