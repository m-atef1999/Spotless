namespace Spotless.Application.Exceptions
{
    public class InsufficientWalletBalanceException : Exception
    {
        public decimal RequiredAmount { get; }
        public decimal CurrentBalance { get; }
        public string Currency { get; }

        public InsufficientWalletBalanceException(decimal requiredAmount, decimal currentBalance, string currency)
            : base($"Insufficient wallet balance. Required: {requiredAmount} {currency}, Available: {currentBalance} {currency}")
        {
            RequiredAmount = requiredAmount;
            CurrentBalance = currentBalance;
            Currency = currency;
        }

        public InsufficientWalletBalanceException(string message) : base(message)
        {
            RequiredAmount = 0;
            CurrentBalance = 0;
            Currency = string.Empty;
        }

        public InsufficientWalletBalanceException(string message, Exception innerException) : base(message, innerException)
        {
            RequiredAmount = 0;
            CurrentBalance = 0;
            Currency = string.Empty;
        }
    }
}

