using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace ElgatoWaveSDK.Models
{
    internal class EventReceivedArgs
    {
        public string EventName
        {
            get;
            set;
        }

        public JsonNode? Obj
        {
            get;
            set;
        }

        public EventReceivedArgs(string eventName, JsonNode? obj)
        {
            EventName = eventName;
            Obj = obj;
        }

    }
}
