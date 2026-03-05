using Microsoft.AspNetCore.Mvc;
using Models;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private static List<User> _users = new List<User>()
    {
        new()
        {
            Id = new Guid("c9fcbc4b-d2d1-4664-9079-dae78a1de446"),
            Name = "JoMoney",
            Address1 = "123 Main Street",
            City = "London",
            PostalCode = 420,
            EmailAddress = "JoMoney@gmail.com",
            PhoneNumber = "88888888"
            
        }
    };

    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public User Get(Guid userId)
    {
        return _users.Where(u => u.Id == userId).First();
    }
    
}