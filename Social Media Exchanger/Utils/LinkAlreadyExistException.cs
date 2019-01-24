using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Social_Media_Exchanger.Utils
{
    public class LinkAlreadyExistException : Exception
    {

        public LinkAlreadyExistException()
            : base()
        {
        }


        public LinkAlreadyExistException(String message)
            : base(message)
        {
        }


        public LinkAlreadyExistException(String message, Exception innerException)
            : base(message, innerException)
        {
        }


        protected LinkAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}