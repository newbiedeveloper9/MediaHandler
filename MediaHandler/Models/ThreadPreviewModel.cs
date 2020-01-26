using System;
using System.Windows.Media;
using fbchat_sharp.API;

namespace MediaHandler.Models
{
    public class ThreadPreviewModel
    {
        private static readonly Random Rnd = new Random();

        public static ThreadPreviewModel Create(ThreadPreviewStruct lastMessage,
            FB_Thread thread, Color? color = null)
        {
            color = color ?? new Color()
            {
                R = (byte)Rnd.Next(0, 255),
                G = (byte)Rnd.Next(0, 255),
                B = (byte)Rnd.Next(0, 255),
                A = byte.MaxValue
            };

            var longTimestamp = Convert.ToInt64(thread.last_message_timestamp);
            var dateOffset = DateTimeOffset.FromUnixTimeSeconds(longTimestamp/1000);
            var date = dateOffset.UtcDateTime.ToLocalTime();
           
            return new ThreadPreviewModel()
            {
                LastMessage = lastMessage,
                Color = new SolidColorBrush(color.Value),
                Thread = thread,
                Date = date,
            };
        }

        public SolidColorBrush Color { get; set; } 
        public FB_Thread Thread { get; set; }
        public ThreadPreviewStruct LastMessage { get; set; }
        public DateTime Date { get; set; }
    }

    public struct ThreadPreviewStruct
    {
        public bool IsRead { get; }
        public string Text { get; }
        public string Author { get; }

        public ThreadPreviewStruct(bool isRead, string text, string author)
        {
            IsRead = isRead;
            Text = text;
            Author = author;
        }
    }
}
