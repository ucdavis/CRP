using System;
using CRP.Core.Helpers;

namespace CRP.Core.Abstractions
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow.ToPacificTime();

        public static void Reset()
        {
            Now = () => DateTime.UtcNow.ToPacificTime();
        }
    }
}