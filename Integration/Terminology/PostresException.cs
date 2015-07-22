using System;

namespace Lis.Test.Integration.Terminology
{
    [Serializable]
    public class PostresException : Exception
    {
        public PostresException(string message)
            : base(message)
        {
        }

        public PostresException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
