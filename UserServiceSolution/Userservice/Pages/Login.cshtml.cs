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
                return Page();
            }

            var credentials = new LoginCredentials(creds.Username, creds.Password);

            using HttpClient? client = _clientFactory?.CreateClient("HaavGateway");
            try
            {
                var res = await client.PostAsJsonAsync("user/login", credentials);
                if (res.IsSuccessStatusCode)
                    return RedirectToPage("/pages/user/list");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToPage("/pages/home");
            
        }
    }
}
