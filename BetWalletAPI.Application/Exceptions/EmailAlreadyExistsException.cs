﻿namespace BetWalletAPI.Application.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException(string message) : base(message) { }
}
