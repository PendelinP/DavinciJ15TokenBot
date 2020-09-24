using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.Common.Interfaces
{
    public class SignedMessageParsingError : Exception
    {
        public SignedMessageParsingError(): base()
        {
        }

        public SignedMessageParsingError(string error): base(error)
        {
        }
    }
}
