using Aiursoft.Handler.Models;
using Aiursoft.Observer.Data;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Observer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ObserverDbContext _dbContext;

        public HomeController(ObserverDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Aiursoft Observer!");
        }
    }
}
