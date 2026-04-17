using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
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
    private IHttpClientFactory _httpClientFactory;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _userRepository = userRepository;
        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"Controllor besked - XYZ Service responding from {_ipaddr}");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
    {
        var userResult = await _userRepository.TryLogin(credentials);
        if (userResult == null)
            return BadRequest();
        // Call auth service to get JWT
        var client = _httpClientFactory.CreateClient("authService");
        var authResponse = await client.PostAsJsonAsync("auth", userResult);
        if (!authResponse.IsSuccessStatusCode)
            return Unauthorized();
        var jwt = await authResponse.Content.ReadAsStringAsync();
        return Ok(jwt);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDTO user)
    {
        if (!ModelState.IsValid)
        {
              _logger.LogWarning("Failed with creating user - invalid model state");
            return BadRequest(ModelState);
        }
        else
        {
            var createdUser = await _userRepository.CreateUser(user);
            _logger.LogInformation($"User created with id {createdUser.Id}");
                    return Ok(createdUser);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userRepository.GetUserById(id);

        if (user == null)
        {
            _logger.LogWarning("Failed getting id som user");
            return NotFound("Bruger ikke fundet.");
        }

        return Ok(user);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = await _userRepository.GetAllUsers();
        
        if (user == null)
        {
            _logger.LogWarning("Failed with creating user");
            return NotFound("Bruger ikke fundet.");
        }
        return Ok(user);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO user)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed with updating user -  invalid model state");
            return BadRequest(ModelState);
        }

        var updatedUser = await _userRepository.UpdateUser(id, user);

        if (updatedUser == null)
        {
            _logger.LogWarning("Failed to update user");
            return NotFound("Bruger ikke fundet.");
        }

        return Ok(updatedUser);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userRepository.DeleteUser(id);

        if (!deleted)
        {
            _logger.LogWarning("Failed to delete user");
            return NotFound("Bruger ikke fundet.");
        }

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