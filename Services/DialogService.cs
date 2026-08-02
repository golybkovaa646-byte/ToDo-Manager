using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;
using ToDo_Manager.View;

namespace ToDo_Manager.Services
{
    public class DialogService : IDialogService
    {
        private readonly ITaskService _taskService;
        private readonly IMessageService _messageService;

        public DialogService(ITaskService taskService, IMessageService messageService)
        {
            _taskService = taskService;
            _messageService = messageService;
        }

        public void EditTask(EditTaskViewModel vm)
        {
            var window = new EditTaskWindow
            {
                DataContext = vm
            };

            window.ShowDialog();
        }
    }

}
