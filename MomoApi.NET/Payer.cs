using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MomoApi.NET
{
    public class Payer
    {
        
        public Payer(string partyId, PartyIdType partyIdType)
        {
            Validate(partyId, partyIdType);
            PartyId = partyId;
            PartyIdType = partyIdType;
        }
        
        public PartyIdType PartyIdType { get; }
        public string PartyId { get; }
        
//        private const string MSISDN = "MSISDN";
//        private const string EMAIL = "EMAIL";
//        private const string PARTY_CODE = "PARTY";
//
//        private string MapPartyIdTypeToEnum(PartyIdType partyIdType)
//        {
//            switch (partyIdType)
//            {
//                case NET.PartyIdType.Email:
//                    return EMAIL;
//                case NET.PartyIdType.Msisdn:
//                    return MSISDN;
//                case NET.PartyIdType.PartyCode:
//                    return PARTY_CODE;
//                default:
//                    throw new Exception();
//            }
//        }

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
        PartyCode
    }
}