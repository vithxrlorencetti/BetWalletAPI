namespace BetWalletAPI.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "BRL")
{
    private Money() : this(0m, "BRL") { }

    private static readonly HashSet<string> ValidCurrencies = new HashSet<string> { "BRL", "USD", "EUR" };

    public static Money Create(decimal amount, string currency = "BRL")
    {
        currency = currency.ToUpper();

        if (string.IsNullOrWhiteSpace(currency) || !ValidCurrencies.Contains(currency))
        {
            throw new ArgumentException($"Invalid currency: '{currency}'. Allowed currencies are BRL, USD, EUR.");
        }

        return new Money(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public override string ToString()
    {
        return Currency.ToUpper() switch
        {
            "BRL" => $"R$ {Amount:F2}",
            "USD" => $"$ {Amount:F2}",
            "EUR" => $"€ {Amount:F2}",
            _ => $"{Currency} {Amount:F2}"
        };
    }

    public bool IsPositive => Amount > 0;
    public bool IsZero => Amount == 0;
    public bool IsNegative => Amount < 0;

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);

    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal money) => new(money);
}