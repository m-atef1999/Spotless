using System.Text.Json.Serialization;

namespace Spotless.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    Fawry,
    PayPal,
    Wallet,
    CashOnDelivery,
    Paymob,
    Instapay
}
