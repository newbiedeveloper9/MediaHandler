using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MediaHandler.Models;

namespace MediaHandler.ViewModels
{
    public partial class ThreadViewModel
    {
        private readonly List<SimpleMessage> _messageBuffer = new List<SimpleMessage>();

        private void SetMessageSuccess(string text)
        {
            var message = _messageBuffer.FirstOrDefault(x => x.Message.Equals(text));
            message.Buffer = SimpleMessage.BufferState.Success;

            //we're setting the state of buffer by REFERENCE and remove it from local list. See MessageColumnCollection in main class part for original source.
            _messageBuffer.Remove(message);
        }

        private async Task BackgroundBuffer()
        {
            var refreshTime = new TimeSpan(0, 0, 2);

            while (true)
            {
                //we're setting the state of buffer by REFERENCE and remove it from local list. See MessageColumnCollection in main class part for original source.
                _messageBuffer.RemoveAll(x => x.Buffer == SimpleMessage.BufferState.Error);

                await Task.Delay(refreshTime.Seconds*1000);
                _messageBuffer.ForEach(x=>
                    x.Buffer = x.Time.AddSeconds(15) <= DateTime.Now 
                        ? SimpleMessage.BufferState.Error : SimpleMessage.BufferState.Waiting);
                _messageBuffer.ForEach(x=>Console.WriteLine($"[{x.Time}]: {x.Message} {x.Buffer}"));

            }
        }
    }
}
