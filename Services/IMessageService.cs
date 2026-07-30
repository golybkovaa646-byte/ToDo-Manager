using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo_Manager.Services
{
    public interface IMessageService
    {
        void ShowError(string message);
        void ShowInfo(string message);
        bool ShowConfrime(string message);
    }

}
