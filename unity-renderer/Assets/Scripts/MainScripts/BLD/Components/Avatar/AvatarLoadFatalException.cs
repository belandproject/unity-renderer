using System;

namespace BLD
{
    public class AvatarLoadFatalException : Exception
    {
        public AvatarLoadFatalException(string message) : base(message)
        {
        }
    }
}