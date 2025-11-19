namespace Spotless.Application.Interfaces
{
    public interface IPaymobSignatureService
    {

        bool VerifyProcessedCallbackSignature(PaymobProcessedCallbackData callbackData, string receivedHmac);

        bool VerifyResponseCallbackSignature(PaymobResponseCallbackData callbackData, string receivedHmac);
    }

    public class PaymobProcessedCallbackData
    {
        public int AmountCents { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public bool ErrorOccured { get; set; }
        public bool HasParentTransaction { get; set; }
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public bool Is3dSecure { get; set; }
        public bool IsAuth { get; set; }
        public bool IsCapture { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsStandalonePayment { get; set; }
        public bool IsVoided { get; set; }
        public int OrderId { get; set; }
        public int Owner { get; set; }
        public bool Pending { get; set; }
        public string SourceDataPan { get; set; } = string.Empty;
        public string SourceDataSubType { get; set; } = string.Empty;
        public string SourceDataType { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class PaymobResponseCallbackData
    {
        public int AmountCents { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public bool ErrorOccured { get; set; }
        public bool HasParentTransaction { get; set; }
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public bool Is3dSecure { get; set; }
        public bool IsAuth { get; set; }
        public bool IsCapture { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsStandalonePayment { get; set; }
        public bool IsVoided { get; set; }
        public int OrderId { get; set; }
        public int Owner { get; set; }
        public bool Pending { get; set; }
        public string SourceDataPan { get; set; } = string.Empty;
        public string SourceDataSubType { get; set; } = string.Empty;
        public string SourceDataType { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
