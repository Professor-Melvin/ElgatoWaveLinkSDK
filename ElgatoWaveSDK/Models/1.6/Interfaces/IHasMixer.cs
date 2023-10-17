using System;
using System.Collections.Generic;
using System.Text;

namespace ElgatoWaveSDK.Models.Interfaces
{
    public interface IHasMixer
    {
        public string? MixerId
        {
            get;
            set;
        }
    }
}
