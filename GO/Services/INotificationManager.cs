using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Services
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;
        void Initialize();
        void SendNotification(string title, string message, DateTime? notifyTime = null);
        void ReceivedNotification(string title, string message);
    }
}
