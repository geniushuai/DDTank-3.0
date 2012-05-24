using System;

namespace Game.Base
{
    public abstract class AbstractCommandHandler
    {
        public virtual void DisplayMessage(BaseClient client, string format, params object[] args)
        {
            DisplayMessage(client, string.Format(format, args));
        }
        public virtual void DisplayMessage(BaseClient client, string message)
        {
            if (client != null)
            {
                client.DisplayMessage(message);
            }
        }

        public virtual void DisplaySyntax(BaseClient client)
        {
            if (client != null)
            {
                CmdAttribute[] attrib = (CmdAttribute[])this.GetType().GetCustomAttributes(typeof(CmdAttribute), false);
                if (attrib.Length > 0)
                {
                    client.DisplayMessage(attrib[0].Description);
                    foreach (string str in attrib[0].Usage)
                        client.DisplayMessage(str);
                }
            }
        }
    }
}
