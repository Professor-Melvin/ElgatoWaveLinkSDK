using System;
using System.Collections.Generic;
using System.Text;

namespace ElgatoWaveSDK.Models.Interfaces
{
    public interface IHasChannel
    {
        public string? ChannelId
        {
            get;
            set;
        }
    }
}
