using System;

namespace MomoApi.NET.Exceptions
{
    public class NetworkException : Exception
    {
        public NetworkException(Exception innerException) : base("There was an issue with the network", innerException)
        {
        }
    }
}