namespace SimpleATMApplication.Models;

public class CardHolderResponseModel
{
    public string CardNumber { get; set; }
    public int Pin { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Decimal Balance { get; set; }
    public Decimal DepositAmt { get; set; }
}