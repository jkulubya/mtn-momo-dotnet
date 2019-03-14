using System.Runtime.Serialization;

namespace MtnMomo.NET
{
    public enum TransactionStatus
    {
        [EnumMember(Value = "PENDING")]
        Pending,
        
        [EnumMember(Value = "SUCCESSFUL")]
        Successful,
        
        [EnumMember(Value = "FAILED")]
        Failed
    }
}