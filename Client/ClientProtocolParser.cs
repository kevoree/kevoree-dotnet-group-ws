using Org.Kevoree.Library.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Client
{
    class ClientProtocolParser
    {
        public Message parse(string message)
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
