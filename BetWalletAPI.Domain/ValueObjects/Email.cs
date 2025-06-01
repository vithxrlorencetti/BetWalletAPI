﻿using System.Text.RegularExpressions;

namespace BetWalletAPI.Domain.ValueObjects;

public record Email(string Value)
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        return new Email(value.ToLowerInvariant());
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string email) => new(email);
}