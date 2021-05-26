using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    public class CandleRangeChangeEventArgs : EventArgs
    {

        public IntRange CandleRange { get; private set; }

        public CandleRangeChangeEventArgs(IntRange range)
        {
            CandleRange = range;
        }

    }
}
