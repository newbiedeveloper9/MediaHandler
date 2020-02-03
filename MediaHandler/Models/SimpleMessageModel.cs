using System;
using System.Threading;

namespace MediaHandler.Models
{
    public class SimpleMessage
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public BufferState Buffer { get; set; }

        public SimpleMessage(string message, DateTime time)
        {
            Message = message;
            Time = time;
        }

        public enum BufferState
        {
            Waiting,
            Success,
            Error,
        }
    }
}
