using System;
using System.Diagnostics;

namespace Lykke.Bil2.BaseTests
{
    public static class Waiting
    {
        public static TimeSpan Timeout { get; }

        static Waiting()
        {
            Timeout = Debugger.IsAttached
                ? System.Threading.Timeout.InfiniteTimeSpan
                : TimeSpan.FromSeconds(10);
        }
    }
}
