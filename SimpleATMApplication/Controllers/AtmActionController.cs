using Microsoft.AspNetCore.Mvc;
using SimpleATMApplication.EFDbContext;
using SimpleATMApplication.Models;

namespace SimpleATMApplication.Controllers;

public class AtmActionController : Controller
{
    private readonly AppDbContext _appDbContext;

    public AtmActionController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    // GET
    [ActionName("Index")]
    public IActionResult AtmActionIndex()
    {
        return View("AtmActionIndex");
    }

    [HttpPost]
    public IActionResult Login(CardHolderResponseModel reqModel)
    {
        CardHolderDataModel cardHolderDataModel = _appDbContext.GetCardHolderByCardNumAndPin(reqModel.CardNumber, reqModel.Pin);
        if (cardHolderDataModel is null)
        {
            TempData["Message"] = "Login Fail !! Please Enter Again";
            return Redirect("/");
        }
        return View("AtmActionHome", cardHolderDataModel);
    }

    [HttpPost]
    public IActionResult Deposit([FromBody] CardHolderResponseModel model)
    {
        if (model != null)
        {
            double amt = model.DepositAmt;
            return Ok("Deposit successful");
        }
        else
        {
            return BadRequest("Invalid data received");
        }
    }
}