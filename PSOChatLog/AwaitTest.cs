using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PSOChatLog
{
    class AwaitTest
    {
        public HelloWorldAwaiter GetAwaiter()
        {
            return new HelloWorldAwaiter();
        }
    }
    struct HelloWorldAwaiter : INotifyCompletion
    {
        public bool IsCompleted 
        {
            get 
            {
                return false; 
            }
        }
        public void OnCompleted(Action continuation)
        {
            // awaitの続きを頼む
            continuation();
        }

        public string GetResult() 
        {
            return "Hello world"; 
        }
    }
}
