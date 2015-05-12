using System;
using System.Windows.Forms;
using MediaPortal.Common.Messaging;
using MediaPortal.UI.General;

namespace BrowseTheWeb
{
    public class MessageHandler : IDisposable
    {
        private AsynchronousMessageQueue _messageQueue;
        private Action onResize;
        private Action onActivate;

        public MessageHandler(Action onResize, Action onActivate)
        {
            this.onResize = onResize;
            this.onActivate = onActivate;
            _messageQueue = new AsynchronousMessageQueue(this, new string[]
                {
                    WindowsMessaging.CHANNEL,
                });
            _messageQueue.MessageReceived += OnMessageReceived;
            _messageQueue.Start();
        }

        public void Dispose()
        {
            _messageQueue.Shutdown();
        }

        private void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
        {
            //const int WM_EXITSIZEMOVE = 0x232;
            const int WM_MOVE = 0x0003;
            const int WM_SIZING = 0x0214;
            const int WM_ACTIVATEAPP = 0x001C;
            if (message.ChannelName == WindowsMessaging.CHANNEL)
            {
                WindowsMessaging.MessageType messageType = (WindowsMessaging.MessageType)message.MessageType;
                if (messageType == WindowsMessaging.MessageType.WindowsBroadcast)
                {
                    Message mess = (Message)message.MessageData[WindowsMessaging.MESSAGE];
                    if (mess.Msg == WM_MOVE || mess.Msg == WM_SIZING)
                    {
                        onResize();
                    }
                    if (mess.Msg == WM_ACTIVATEAPP && mess.WParam != null)
                    {
                        onActivate();
                    }
                }
            }
        }

    }
}
