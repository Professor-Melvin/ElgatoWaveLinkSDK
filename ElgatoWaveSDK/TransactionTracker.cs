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
        private int _baseNum { get; set; } = 0;
        private int _transactionId { get; set; } = 1;

        internal TransactionTracker(int baseNum, int transactionId)
        {
            _baseNum = baseNum;
            _transactionId = transactionId;
        }

        public TransactionTracker()
        {

        }

        public int NextTransactionId()
        {
            if(_baseNum == 0)
            {
                _baseNum = new Random().Next(0, 121);
            }

            _transactionId++;

            if (_transactionId >= 5752191)
            {
                _transactionId = 0;
                _baseNum++;

                if(_baseNum >= 121)
                {
                    _baseNum = 0;
                }
            }

            return Int32.Parse($"{_baseNum:D3}{_transactionId:D7}");
        }
    }
}
