namespace Spotless.Domain.Exceptions
{
    public class PaymentFailedException : Exception
    {
        public PaymentFailedException() : base("Payment processing failed.")
        {
        }

        public PaymentFailedException(string message) : base(message)
        {
        }

        public PaymentFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

