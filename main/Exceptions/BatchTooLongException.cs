using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotosAPI.Exceptions
{
    public class BatchTooLongException : Exception
    {
        public BatchTooLongException()
        {
        }

        public BatchTooLongException(string message) : base(message)
        {
            Console.WriteLine("needs to be less sthan 50 items");
        }

        public BatchTooLongException(string message, Exception innerException) : base(message, innerException)
        {
            Console.WriteLine("needs to be less than 50 items, please remove some");
        }
    }
}
