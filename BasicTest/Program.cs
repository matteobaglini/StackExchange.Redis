﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace BasicTest
{
    class Program
    {
        static void Main()
        {
            MassiveBulkOpsAsync(true, true);
            MassiveBulkOpsAsync(true, false);
            MassiveBulkOpsAsync(false, true);
            MassiveBulkOpsAsync(false, false);
        }
        const int AsyncOpsQty = 100000;
        static void MassiveBulkOpsAsync(bool preserveOrder, bool withContinuation)
        {
            using (var muxer = ConnectionMultiplexer.Connect("127.0.0.1"))
            {
                muxer.PreserveAsyncOrder = preserveOrder;
                RedisKey key = "MBOA";
                var conn = muxer.GetDatabase();
                muxer.Wait(conn.PingAsync());

                Action<Task> nonTrivial = delegate
                {
                    Thread.SpinWait(5);
                };
                var watch = Stopwatch.StartNew();
                for (int i = 0; i <= AsyncOpsQty; i++)
                {
                    var t = conn.StringSetAsync(key, i);
                    if (withContinuation) t.ContinueWith(nonTrivial);
                }
                int val = (int)muxer.Wait(conn.StringGetAsync(key));
                watch.Stop();

                Console.WriteLine("After {0}: {1}", AsyncOpsQty, val);
                Console.WriteLine("({3}, {4})\r\n{2}: Time for {0} ops: {1}ms; ops/s: {5}", AsyncOpsQty, watch.ElapsedMilliseconds, Me(),
                    withContinuation ? "with continuation" : "no continuation", preserveOrder ? "preserve order" : "any order",
                    AsyncOpsQty / watch.Elapsed.TotalSeconds);
            }
        }
        protected static string Me([CallerMemberName] string caller = null)
        {
            return caller;
        }

    }
}
