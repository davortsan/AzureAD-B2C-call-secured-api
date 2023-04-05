using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using TasksWeb.Models;

namespace TasksWeb.Controllers
{
    [Authorize]
    [AuthorizeForScopes(Scopes = new string[] {"access_as_user"})]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenAcquisition _tokenAdquisition;

        public TaskController(ILogger<TaskController> logger, IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAdquisition)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenAdquisition = tokenAdquisition;
        }

        [AuthorizeForScopes(Scopes = new string[] { "access_as_user" })]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<TaskModel> model = new List<TaskModel>();

                //The next line uses the URL + scope that is created when you expose an API in your APP Registrations
                string[] scopes = new string[] { "https://yourtenantname.onmicrosoft.com/GUID_API/access_as_user" };
                string accessToken = await _tokenAdquisition.GetAccessTokenForUserAsync(scopes);


                var request = new HttpRequestMessage(HttpMethod.Get, "Task");

                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var client = _httpClientFactory.CreateClient("TasksAPI");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var _result = response.Content.ReadAsStringAsync().Result;

                    model = JsonConvert.DeserializeObject<TaskModel[]>(_result).ToList();
                }

                return View(model);
            }
            catch (Exception ex) { return View(); }
        }
    }
}
