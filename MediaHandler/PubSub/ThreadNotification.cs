using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHandler
{
    public class ThreadNotification<T> : IThreadNotification<T>
    {
        public T Obj { get; set; }

        public ThreadNotification(T obj)
        {
            Obj = obj;
        }
    }
    
    public interface IThreadNotification<T>
    {
        T Obj { get; set; }
    }
}
