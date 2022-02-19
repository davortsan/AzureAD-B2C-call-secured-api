using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace TasksAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private static readonly string[] TaskNames = new[]
        {
        "Task 1", "Task 2", "Task 3", "Task 4", "Task 5", "Task 6"
        };

        private readonly ILogger<TaskController> _logger;

        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public TaskController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Task> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            return Enumerable.Range(1, 5).Select(index => new Task
            {
                LimitDate = DateTime.Now.AddDays(index),
                Priority = Random.Shared.Next(1, 10),
                TaskName = TaskNames[Random.Shared.Next(TaskNames.Length)]
            })
            .ToArray();
        }
    }
}