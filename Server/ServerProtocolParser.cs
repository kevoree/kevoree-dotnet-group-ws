using Org.Kevoree.Library.Protocol;
using System;
using System.Text.RegularExpressions;

namespace Org.Kevoree.Library.Server
{
    internal class ServerProtocolParser
    {
        public Message Parse(string message)
        {
            Message ret;
            if (String.IsNullOrWhiteSpace(message))
            {
                throw new ServerProtocolParsingError("Message empty");
            }

            var messageTrimed = message.Trim();

            var pushPrefix = "push/";
            if (messageTrimed.StartsWith(pushPrefix))
            {
                var content = message.Substring(pushPrefix.Length);
                ret = new Push(content);

            }
            else if (messageTrimed.Equals("pull"))
            {
                ret = new Pull();
            }
            else
            {
                var matc = Regex.Match(messageTrimed, "^register/([a-z-0-9]+)/(.+)$");
                if (matc.Success)
                {
                    var group = matc.Groups[1].Value;
                    var model = matc.Groups[2].Value;
                    ret = new Register(group, model);
                }
                else
                {
                    throw new ServerProtocolParsingError("Unknown action");
                }
            }

            return ret;
        }
    }
}
