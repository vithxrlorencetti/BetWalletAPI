﻿namespace BetWalletAPI.Application.Exceptions;

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message) : base(message) { }
}
