using System;
using System.Collections.Generic;

namespace ElgatoWaveSDK
{
    internal interface ITransactionTracker
    {
        int NextTransactionId();
    }

    internal class TransactionTracker : ITransactionTracker
    {
        private int baseNum { get; set; } = 0;
        private int TransactionId { get; set; } = 1;

        public int NextTransactionId()
        {
            if(baseNum == 0)
            {
                baseNum = new Random().Next(0, 121);
            }

            TransactionId++;

            if (TransactionId >= 5752191)
            {
                TransactionId = 0;
                baseNum++;

                if(baseNum >= 121)
                {
                    baseNum = 0;
                }
            }

            return Int32.Parse($"{baseNum:D3}{TransactionId:D7}");
        }
    }
}
