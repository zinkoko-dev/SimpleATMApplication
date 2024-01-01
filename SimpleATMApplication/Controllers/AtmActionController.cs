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
    private int? userId;
    public AtmActionController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public bool CheckAuthUserId()
    {
        userId = HttpContext.Session.GetInt32(SessionKeyUserId);
        return userId != null;
    }

    // GET
    [ActionName("Index")]
    public IActionResult AtmActionIndex()
    {
        if (CheckAuthUserId())
        {
            return Redirect("/atmaction/home");
        }
        return View("AtmActionIndex");
    }

    [ActionName("Home")]
    public IActionResult AtmActionHome()
    {
        if (CheckAuthUserId())
        {
            return View("AtmActionHome");
        }
        TempData["Message"] = "Please Login !!";
        return Redirect("/");
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

        if (!CheckAuthUserId())
        {
            return BadRequest("User not found.");
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
    
    [HttpPost]
    public async Task<IActionResult> Withdraw(CardHolderResponseModel reqModel)
    {
        if (reqModel.WithdrawAmt <= 0)
        {
            return BadRequest("Withdraw amount should be greater than zero.");
        }
        
        if (!CheckAuthUserId())
        {
            return BadRequest("User not found.");
        }

        var cardHolder = await _appDbContext.CardHolders.FirstOrDefaultAsync(x => x.Id == userId);
        if (cardHolder is null)
        {
            return BadRequest("Card holder not found.");
        }

        if (reqModel.WithdrawAmt > cardHolder.Balance)
        {
            return BadRequest("Not enough balance.");
        }

        cardHolder.Balance -= reqModel.WithdrawAmt;
        var saveResult = await _appDbContext.SaveChangesAsync();

        if (saveResult > 0)
        {
            return Ok("Withdraw successful.");
        }
        else
        {
            return BadRequest("Failed to save withdraw.");
        }
    }
}