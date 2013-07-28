using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Laugris.Sage
{
    /// <summary>
    /// The handler for the registered Windows message
    /// </summary>
    /// <param name="m">The Windows message.</param>
    public delegate void MessageHandler(ref Message m);

    /// <summary>
    /// Custom windows message which can be processed by the application or window
    /// </summary>
    public sealed class RegisteredMessage
    {
        public int MessageId { get; set; }
        public MessageHandler Handler { get; set; }
    }

    /// <summary>
    /// Custom messages processor    
    /// Must be connected to the window or Form in order to be executed
    /// </summary>
    [Serializable]
    public sealed class Messenger : Dictionary<int, RegisteredMessage>
    {
        public int RegisterMessage(string message, MessageHandler handler)
        {
            RegisteredMessage rm;

            if (handler == null)
                return 0;
            else
            {
                int id = NativeMethods.RegisterWindowMessage(message);
                if (!this.ContainsKey(id))
                {
                    rm = new RegisteredMessage();
                    rm.Handler = handler;
                    rm.MessageId = id;
                    NativeMethods.AddRemoveMessageFilter(id, ChangeWindowMessageFilterFlags.Add);
                    this.Add(id, rm);
                }
                else
                {
                    rm = this[id];
                    rm.Handler = handler;
                }
                return id;
            }
        }

        public int RegisterMessage(int messageId, MessageHandler handler)
        {
            RegisteredMessage rm;

            if (handler == null)
                return 0;
            else
            {
                if (!this.ContainsKey(messageId))
                {
                    rm = new RegisteredMessage();
                    rm.Handler = handler;
                    rm.MessageId = messageId;
                    NativeMethods.AddRemoveMessageFilter(messageId, ChangeWindowMessageFilterFlags.Add);
                    this.Add(messageId, rm);
                }
                else
                {
                    rm = this[messageId];
                    rm.Handler = handler;
                }
                return messageId;
            }
        }

        /// <summary>
        /// Returns the message id of the specified message. 
        /// If message is not regsitered yet it will be registered first
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static int MessageId(string message)
        {
            return NativeMethods.RegisterWindowMessage(message);
        }


        public RegisteredMessage this[Message m]
        {
            get 
            {
                if (this.ContainsKey(m.Msg))
                    return this[m.Msg];
                else
                    return null;
            }
        }

        public bool Dispatch(ref Message m)
        {
            try
            {
                RegisteredMessage message = this[m];
                if (message == null)
                    return false;
                else
                {
                    if (message.Handler == null)
                        return false;
                    message.Handler(ref m);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new InvocationException("Laugris Messenger error: ", ex);
            }
        }
    }
}
