using System;
using System.Runtime.Serialization;


namespace ElgatoWaveSDK.Models
{
    [Serializable]
    public class ElgatoException : Exception
    {
        public ElgatoException(string message): base(message)
        {

        }

        public ElgatoException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected ElgatoException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
    }
}
