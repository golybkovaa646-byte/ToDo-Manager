using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ToDo_Manager.Services
{
    public class MessageService : IMessageService
    {
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
