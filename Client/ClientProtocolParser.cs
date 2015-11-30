using Org.Kevoree.Library.Protocol;
using System;

namespace Org.Kevoree.Library.Client
{
    class ClientProtocolParser
    {
        public Message Parse(string message)
        {
            if (String.IsNullOrWhiteSpace(message))
            {
                throw new ClientProtocolParsingError("Message empty");
            }

            var messageTrimed = message.Trim();

            var pushPrefix = "push/";
            if (!messageTrimed.StartsWith(pushPrefix))
            {
                throw new ClientProtocolParsingError("Unknown action");
            }

            var content = message.Substring(pushPrefix.Length);

            return new Push(content);
        }
    }
}
