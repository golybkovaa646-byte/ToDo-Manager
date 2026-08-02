using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;

namespace ToDo_Manager.ViewModels
{
    public partial class TagsViewModel : ObservableObject
    {
        private readonly ITagService _tagService;
        private readonly IMessageService _messageService;

        public ObservableCollection<Tag> Tags { get; } = new();

        [ObservableProperty]
        private System.Windows.Media.Color _newTagColor = System.Windows.Media.Colors.Gray;


        [ObservableProperty]
        private string newTagName = string.Empty;

        [ObservableProperty]
        private string newTagColorHex = "#808080";

        public TagsViewModel(ITagService tagService, IMessageService messageService)
        {
            _tagService = tagService;
            _messageService = messageService;

            LoadTags();
        }

        private async void LoadTags()
        {
            var items = await _tagService.GetAllAsync();
            Tags.Clear();
            foreach (var tag in items)
                Tags.Add(tag);
        }

        partial void OnNewTagColorChanged(System.Windows.Media.Color value)
        {
            NewTagColorHex = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
        }


        [RelayCommand]
        private async void AddTag()
        {
            if (string.IsNullOrWhiteSpace(NewTagName))
            {
                _messageService.ShowInfo("Tag name cannot be empty.");
                return;
            }

            var tag = new Tag
            {
                Name = NewTagName,
                ColorHex = NewTagColorHex
            };

            await _tagService.AddAsync(tag);
            Tags.Add(tag);

            NewTagName = string.Empty;
            NewTagColorHex = "#808080";
        }

        [RelayCommand]
        private async void DeleteTag(Tag tag)
        {
            await _tagService.DeleteAsync(tag);
            Tags.Remove(tag);
        }
    }
}
