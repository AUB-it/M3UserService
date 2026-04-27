using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace UserService.Pages
{
    public class UserListModel : PageModel
    {
        private readonly IHttpClientFactory? _clientFactory = null;
        public List<User>? Users {get; set;}
        public UserListModel(IHttpClientFactory clientFactory)
            => _clientFactory = clientFactory;
        public void OnGet()
        {
            using HttpClient? client = _clientFactory?.CreateClient("HaavGateway");
            try
            {
                Users = client?.GetFromJsonAsync<List<User>>(
                    "user").Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
