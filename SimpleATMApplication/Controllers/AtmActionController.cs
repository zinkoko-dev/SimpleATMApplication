using Microsoft.AspNetCore.Mvc;

namespace SimpleATMApplication.Controllers;

public class AtmActionController : Controller
{
    // GET
    [ActionName("Index")]
    public IActionResult AtmActionIndex()
    {
        return View("AtmActionIndex");
    }
}