using System.ComponentModel.DataAnnotations;

namespace Models;

public class UserDTO
{
    [Required(ErrorMessage = "This field is REALLY REQUIRED FR")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
    public string GivenName { get; set; } = "";
    [Required(ErrorMessage = "This field is REALLY adfs FR")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
    public string FamilyName { get; set; } = "";
    [Required(ErrorMessage = "This FR 124124")]
    public string Address1 { get; set; } = "";
    public string? Address2 { get; set; } = "";
    public short PostalCode { get; set; }
    [Required(ErrorMessage = "This FR")]
    public string FaxNumber { get; set; } = "";
    [Required(ErrorMessage = " field is  REQUIRED FR")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
    public string City { get; set; } = "";
    [Required(ErrorMessage = "adfs sdfg2 is 346 52 s2")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "adf field is jjj 23  b")]
    public string Telephone { get; set; } = "";
}