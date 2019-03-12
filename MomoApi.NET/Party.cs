using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MomoApi.NET
{
    public class Party
    {
        
        public Party(string partyId, PartyIdType partyIdType)
        {
            Validate(partyId, partyIdType);
            PartyId = partyId;
            PartyIdType = partyIdType;
        }
        
        public PartyIdType PartyIdType { get; }
        public string PartyId { get; }
        
        private void Validate(string partyId, PartyIdType partyIdType)
        {
            
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum  PartyIdType
    {
        [EnumMember(Value = "MSISDN")]
        Msisdn,
        
        [EnumMember(Value = "EMAIL")]
        Email,
        
        [EnumMember(Value = "PARTY_CODE")]
        Party_Code
    }
}