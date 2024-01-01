namespace SimpleATMApplication.Models;

public class CardHolderResponseModel
{
    public CardHolderResponseModel()
    {
    }

    public CardHolderResponseModel(string cardNumber, int pin, string firstName, string lastName, double balance, double depositAmt)
    {
        CardNumber = cardNumber;
        Pin = pin;
        FirstName = firstName;
        LastName = lastName;
        Balance = balance;
        DepositAmt = depositAmt;
    }

    public string CardNumber { get; set; }
    public int Pin { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double Balance { get; set; }
    public double DepositAmt { get; set; }
}