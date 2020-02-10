using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHandler
{
    public class SubNotification<T> : ISubNotification<T>
    {
        public T Obj { get; set; }

        public SubNotification(T obj)
        {
            Obj = obj;
        }
    }
    
    public interface ISubNotification<T>
    {
        T Obj { get; set; }
    }
}
