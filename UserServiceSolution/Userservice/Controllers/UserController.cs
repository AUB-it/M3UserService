using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using UserService.Repositories.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDTO user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await _userRepository.CreateUser(user);

        return Ok(createdUser);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userRepository.GetUserById(id);

        if (user == null)
            return NotFound("Bruger ikke fundet.");

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllUsers();
        return Ok(users);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedUser = await _userRepository.UpdateUser(id, user);

        if (updatedUser == null)
            return NotFound("Bruger ikke fundet.");

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userRepository.DeleteUser(id);

        if (!deleted)
            return NotFound("Bruger ikke fundet.");

        return NoContent();
    }
    
    [HttpGet("version")]
    public async Task<Dictionary<string,string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;
        properties.Add("service", "HaaV User Service");
        var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
            .Assembly.Location).ProductVersion;
        properties.Add("version", ver!);
        try {
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString();
            properties.Add("hosted-at-address", ipa);
        } catch (Exception ex) {
            _logger.LogError(ex.Message);
            properties.Add("hosted-at-address", "Could not resolve IP-address");
        }
        return properties;
    }
}