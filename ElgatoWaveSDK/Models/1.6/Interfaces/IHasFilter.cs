using System;
using System.Collections.Generic;
using System.Text;

namespace ElgatoWaveSDK.Models.Interfaces
{
    public interface IHasFilter
    {
        public string? FilterId
        {
            get;
            set;
        }
    }
}
