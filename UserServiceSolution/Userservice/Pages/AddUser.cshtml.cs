using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace UserService.Pages
{
    public class AddUserModel : PageModel
    {
        private readonly IHttpClientFactory? _clientFactory = null;
        [BindProperty]
        public UserDTO User { get; set; } = new();
        public AddUserModel(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine("On post was hit");
            foreach (var item in Request.Form)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            Console.WriteLine($"Username after binding: {User.Username}");
            Console.WriteLine($"Email after binding: {User.Email}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine(ModelState.ErrorCount);
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = User.Username,
                Password = User.Password,
                GivenName = User.GivenName,
                FamilyName = User.FamilyName,
                Address1 = User.Address1,
                Address2 = User.Address2,
                PostalCode = User.PostalCode,
                FaxNumber = User.FaxNumber,
                City = User.City,
                Email = User.Email,
                Telephone = User.Telephone
            };

            using HttpClient? client = _clientFactory?.CreateClient("HaavGateway");
            try
            {
                var res = await client.PostAsJsonAsync("user", newUser);
                if (res.IsSuccessStatusCode)
                    return RedirectToPage("/UserList");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToPage("/Home");
            
        }
    }
}
