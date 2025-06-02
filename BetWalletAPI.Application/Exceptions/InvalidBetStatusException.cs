namespace BetWalletAPI.Application.Exceptions;

public class InvalidBetStatusException : Exception
{
    public InvalidBetStatusException(string message) : base(message) { }
}
