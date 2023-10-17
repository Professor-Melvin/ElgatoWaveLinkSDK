using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class InputStateChange : IHasChannel
    {
        public string? ChannelId
        {
            get;
            set;
        }

        public bool State
        {
            get;
            set;
        }
    }
}
