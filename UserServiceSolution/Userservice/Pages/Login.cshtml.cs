using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace UserService.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory? _clientFactory = null;
        [BindProperty]
        public LoginCredentials creds { get; set; }
        public LoginModel(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is not valid");
                return Page();
            }

            var credentials = new LoginCredentials(creds.Username, creds.Password);

            using HttpClient? client = _clientFactory?.CreateClient("HaavGateway");
            try
            {
                var res = await client.PostAsJsonAsync("user/login", credentials);
                if (res.IsSuccessStatusCode)
                {
                    var jwtToken = await res.Content.ReadFromJsonAsync<string>();
                    Response.Cookies.Append("access_token", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Path = "/userservices",
                        MaxAge = TimeSpan.FromHours(1)
                    });
                    return RedirectToPage("/UserList");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToPage("/Home");
            
        }
    }
}
