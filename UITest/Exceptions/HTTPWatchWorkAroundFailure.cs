using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

// ref: http://blog.gurock.com/articles/creating-custom-exceptions-in-dotnet/

namespace UI.WebAccess.Exceptions
{
    [Serializable]
    public class HTTPWatchWorkAroundException : SystemException
    {
        public HTTPWatchWorkAroundException() { }
        public HTTPWatchWorkAroundException(string message) : base(message) { }
        protected HTTPWatchWorkAroundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
