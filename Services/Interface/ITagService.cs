using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;

namespace ToDo_Manager.Services.Interface
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllAsync();
        Task AddAsync(Tag tag);
        Task DeleteAsync(Tag tag);
    }
}
