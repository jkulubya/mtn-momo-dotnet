using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MomoApi.NET.Exceptions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorCode
    {
        [EnumMember(Value = "PAYEE_NOT_FOUND")]
        PayeeNotFound,

        [EnumMember(Value = "PAYER_NOT_FOUND")]
        PayerNotFound,
        
        [EnumMember(Value = "NOT_ALLOWED")]
        NotAllowed,

        [EnumMember(Value = "NOT_ALLOWED_TARGET_ENVIRONMENT")]
        NotAllowedTargetEnvironment,

        [EnumMember(Value = "INVALID_CALLBACK_URL_HOST")]
        InvalidCallbackUrlHost,

        [EnumMember(Value = "INVALID_CURRENCY")]
        InvalidCurrency,

        [EnumMember(Value = "SERVICE_UNAVAILABLE")]
        ServiceUnavailable,

        [EnumMember(Value = "INTERNAL_PROCESSING_ERROR")]
        InternalProcessingError,

        [EnumMember(Value = "NOT_ENOUGH_FUNDS")]
        NotEnoughFunds,

        [EnumMember(Value = "PAYER_LIMIT_REACHED")]
        PayerLimitReached,

        [EnumMember(Value = "PAYEE_NOT_ALLOWED_TO_RECEIVE")]
        PayeeNotAllowedToReceive,

        [EnumMember(Value = "PAYMENT_NOT_APPROVED")]
        PaymentNotApproved,

        [EnumMember(Value = "RESOURCE_NOT_FOUND")]
        ResourceNotFound,

        [EnumMember(Value = "APPROVAL_REJECTED")]
        ApprovalRejected,
        
        [EnumMember(Value = "EXPIRED")]
        Expired,

        [EnumMember(Value = "TRANSACTION_CANCELED")]
        TransactionCancelled,

        [EnumMember(Value = "RESOURCE_ALREADY_EXIST")]
        ResourceAlreadyExist
    }
}