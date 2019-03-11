using System.Runtime.Serialization;

namespace MomoApi.NET
{
    public enum Status
    {
        [EnumMember(Value = "PENDING")]
        Pending,
        
        [EnumMember(Value = "SUCCESSFUL")]
        Successful,
        
        [EnumMember(Value = "FAILED")]
        Failed
    }
}