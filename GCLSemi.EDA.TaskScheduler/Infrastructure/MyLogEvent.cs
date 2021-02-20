using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calamus.TaskScheduler.Infrastructure
{
    class MyLogEvent : IEnumerable<KeyValuePair<string, object>>
    {
        List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

        public string Message { get; }

        public MyLogEvent(string message)
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerators()
        {
            return _properties.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return GetEnumerators(); }

        public MyLogEvent WithProperty(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_properties).GetEnumerator();
        }

        public static Func<MyLogEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;
    }
}
