using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleATMApplication.EFDbContext;
using SimpleATMApplication.Models;

namespace SimpleATMApplication.Controllers;

public class AtmActionController : Controller
{
    private readonly AppDbContext _appDbContext;
    private const string SessionKeyUserId = "_AuthenticatedUser";
    private const string SessionKeyUserFirstName = "_AuthenticatedUserName";
    
    public AtmActionController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    // GET
    [ActionName("Index")]
    public IActionResult AtmActionIndex()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeyUserId);
        if (userId is not null)
        {
            return Redirect("/atmaction/home");
        }
        return View("AtmActionIndex");
    }

    [ActionName("Home")]
    public IActionResult AtmActionHome()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeyUserId);
        if (userId is null)
        {
            TempData["Message"] = "Please Login !!";
            return Redirect("/");
        }
        return View("AtmActionHome");
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
        
        HttpContext.Session.SetInt32(SessionKeyUserId, cardHolderDataModel.Id);
        HttpContext.Session.SetString(SessionKeyUserFirstName, cardHolderDataModel.FirstName);
        return Redirect("/atmaction/home");
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(CardHolderResponseModel reqModel)
    {
        if (reqModel.DepositAmt <= 0)
        {
            return BadRequest("Deposit amount should be greater than zero.");
        }

        var userId = HttpContext.Session.GetInt32(SessionKeyUserId);
        if (userId is null)
        {
            return BadRequest("User session not found.");
        }

        var cardHolder = await _appDbContext.CardHolders.FirstOrDefaultAsync(x => x.Id == userId);
        if (cardHolder is null)
        {
            return BadRequest("Card holder not found.");
        }

        cardHolder.Balance += reqModel.DepositAmt;
        var saveResult = await _appDbContext.SaveChangesAsync();

        if (saveResult > 0)
        {
            return Ok("Deposit successful.");
        }
        else
        {
            return BadRequest("Failed to save deposit.");
        }
    }
}