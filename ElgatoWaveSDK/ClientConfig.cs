using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElgatoWaveSDK
{
    public class ClientConfig
    {
        public int PortStart { get; set; } = 1824;
        public int PortRange { get; set; } = 10;
        public int MaxAttempts { get; set; } = 2;

        public int BufferSize { get; set; } = 1024; //1mb
        public int MaxBufferSize { get; set; } = 51200; //50mb

#if  DEBUG
        public int ResponseTimeout { get; set; } = 600000;  
#else
        public int ResponseTimeout { get; set; } = 20000;
#endif
    }
}
