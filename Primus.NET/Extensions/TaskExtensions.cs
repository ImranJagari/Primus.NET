using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Primus.NET.Extensions
{
    public static class TaskExtensions
    {
        public static void Execute<T>(this Task<T> task, Func<T, Task> action)
        {
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(async () =>
            {
                var result = awaiter.GetResult();
                if (result != null)
                    await action(result);
                else
                    throw new Exception("Result of the task is null cannot continue !");
            });
        }
    }
}
