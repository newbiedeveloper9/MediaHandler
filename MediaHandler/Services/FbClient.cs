using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using fbchat_sharp.API;
using MediaHandler.PubSub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaHandler.Services
{
    public class FbClient : MessengerClient
    {
        private readonly IEventAggregator _eventAggregator;

        public FbClient(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override Task onMessage(string mid = null, string author_id = null, string message = null, FB_Message message_object = null,
            string thread_id = null, ThreadType? thread_type = null, long ts = 0, JToken metadata = null, JToken msg = null)
        {
            var newMessage = new NewMessageStruct(thread_id, message_object);
            _eventAggregator.BeginPublishOnUIThread(
                new ThreadNotification<NewMessageStruct>(newMessage));
            return base.onMessage(mid, author_id, message, message_object, thread_id, thread_type, ts, metadata, msg);
        }

        protected override async Task DeleteCookiesAsync()
        {
            await Task.Yield();
            File.Delete(Environment.CurrentDirectory + @"\cookies");
        }

        protected override async Task<Dictionary<string, List<Cookie>>> ReadCookiesFromDiskAsync()
        {
            try
            {
                var file = Path.Combine(Environment.CurrentDirectory, "cookies");

                using (var fileStream = File.OpenRead(file))
                {
                    await Task.Yield();
                    using (var jsonTextReader = new JsonTextReader(new StreamReader(fileStream)))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<Dictionary<string, List<Cookie>>>(jsonTextReader);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Problem reading cookies from disk: {ex}");
                return null;
            }
        }

        protected override async Task WriteCookiesToDiskAsync(Dictionary<string, List<Cookie>> cookieJar)
        {
            var file = Path.Combine(Environment.CurrentDirectory, "cookies");

            using (var fileStream = File.Create(file))
            {
                try
                {
                    using (var jsonWriter = new JsonTextWriter(new StreamWriter(fileStream)))
                    {
                        var serializer = new JsonSerializer();

                        serializer.Serialize(jsonWriter, cookieJar);
                        await jsonWriter.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    Log($"Problem writing cookies to disk: {ex}");
                }
            }
        }
    }
}

