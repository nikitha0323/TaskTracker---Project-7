using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskTracker.Data;

namespace TaskTracker.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly TaskRepository _repository = new();
        private List<TaskItem> _tasks = new();
        private bool _refreshing;
        private bool _storageReady;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _tasks = _repository.Load();
                _storageReady = true;

                RefreshTaskList();
                StatusText.Text = "Tasks loaded. Ready.";
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is JsonException)
            {
                StatusText.Text =
                    "Could not load tasks. Saving is disabled to protect " +
                    $"the existing file. Details: {ex.Message}";
            }
        }

        private void RefreshTaskList(Guid? selectedId = null)
        {
            _refreshing = true;

            TaskList.ItemsSource = null;
            TaskList.ItemsSource = _tasks;

            TaskList.SelectedItem = selectedId.HasValue
                ? _tasks.FirstOrDefault(t => t.Id == selectedId.Value)
                : null;

            _refreshing = false;

            UpdateEditor();

            int completed = _tasks.Count(t => t.IsCompleted);

            TaskCountText.Text =
                $"Tasks: {_tasks.Count} | Completed: {completed}";
        }

        private void UpdateEditor()
        {
            if (TaskList.SelectedItem is TaskItem task)
            {
                TitleInput.Text = task.Title;
                DescriptionInput.Text = task.Description;
            }
            else
            {
                TitleInput.Text = "";
                DescriptionInput.Text = "";
            }
        }

        private void TaskList_SelectionChanged(
            object? sender,
            SelectionChangedEventArgs e)
        {
            if (!_refreshing)
            {
                UpdateEditor();
            }
        }

        private bool ReadInput(
            out string title,
            out string description)
        {
            title = (TitleInput.Text ?? "").Trim();
            description = (DescriptionInput.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                StatusText.Text = "Please enter a task title.";
                return false;
            }

            return true;
        }

        private void SaveTasks(
            List<TaskItem> updatedTasks,
            string message,
            Guid? selectedId = null)
        {
            if (!_storageReady)
            {
                StatusText.Text =
                    "Saving is disabled because the task file could not load.";
                return;
            }

            try
            {
                // Save successfully before changing the displayed task list.
                _repository.Save(updatedTasks);

                _tasks = updatedTasks;
                RefreshTaskList(selectedId);

                StatusText.Text = message;
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException)
            {
                StatusText.Text = $"Could not save tasks: {ex.Message}";
            }
        }

        private void AddTask_Click(object? sender, RoutedEventArgs e)
        {
            if (!ReadInput(out string title, out string description))
            {
                return;
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description
            };

            var updatedTasks = new List<TaskItem>(_tasks);
            updatedTasks.Add(task);

            SaveTasks(updatedTasks, "Task added.", task.Id);
        }

        private void SaveChanges_Click(object? sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is not TaskItem selected)
            {
                StatusText.Text = "Select a task to edit.";
                return;
            }

            if (!ReadInput(out string title, out string description))
            {
                return;
            }

            var updatedTasks = _tasks.Select(task =>
                task.Id == selected.Id
                    ? new TaskItem
                    {
                        Id = task.Id,
                        Title = title,
                        Description = description,
                        IsCompleted = task.IsCompleted
                    }
                    : task).ToList();

            SaveTasks(updatedTasks, "Task updated.", selected.Id);
        }

        private void ClearForm_Click(object? sender, RoutedEventArgs e)
        {
            TaskList.SelectedItem = null;
            TitleInput.Text = "";
            DescriptionInput.Text = "";

            StatusText.Text = "Form cleared. Enter a new task.";
        }

        private void ToggleComplete_Click(
            object? sender,
            RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is not TaskItem selected)
            {
                StatusText.Text = "Select a task first.";
                return;
            }

            var updatedTasks = _tasks.Select(task =>
                task.Id == selected.Id
                    ? new TaskItem
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        IsCompleted = !task.IsCompleted
                    }
                    : task).ToList();

            SaveTasks(
                updatedTasks,
                "Completion status changed.",
                selected.Id);
        }

        private void DeleteTask_Click(object? sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is not TaskItem selected)
            {
                StatusText.Text = "Select a task to delete.";
                return;
            }

            var updatedTasks = _tasks
                .Where(task => task.Id != selected.Id)
                .ToList();

            SaveTasks(updatedTasks, "Task deleted.");
        }
    }
}