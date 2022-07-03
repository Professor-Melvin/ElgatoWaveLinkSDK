using System;


namespace ElgatoWaveSDK.Models
{
    public class ElgatoException : Exception
    {
        public ElgatoException(string message): base(message)
        {

        }

        public ElgatoException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
