using Microsoft.AspNetCore.Mvc;
namespace RaceDay.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() => View();
}
