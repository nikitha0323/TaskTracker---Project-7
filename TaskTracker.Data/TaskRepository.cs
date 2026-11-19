using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TaskTracker.Data
{
    public class TaskRepository
    {
        public string FilePath { get; }

        public TaskRepository()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "TaskTracker");

            FilePath = Path.Combine(folder, "tasks.json");
        }

        public List<TaskItem> Load()
        {
            if (!File.Exists(FilePath))
            {
                return new List<TaskItem>();
            }

            string json = File.ReadAllText(FilePath);

            return JsonSerializer.Deserialize<List<TaskItem>>(json)
                ?? throw new InvalidDataException(
                    "The saved task file does not contain a task list.");
        }

        public void Save(List<TaskItem> tasks)
        {
            string folder = Path.GetDirectoryName(FilePath)!;
            Directory.CreateDirectory(folder);

            string json = JsonSerializer.Serialize(
                tasks,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            // Write a temporary file before replacing the saved file.
            string temporaryPath = FilePath + ".tmp";

            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, FilePath, overwrite: true);
        }
    }
}