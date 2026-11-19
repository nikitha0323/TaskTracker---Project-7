# Task Tracker - Project 7

A desktop task tracker built with Avalonia and C#.
I developed it on Windows and tested the same code on Ubuntu Linux.

## How to Run

Open a terminal in the folder containing TaskTracker.sln.

Build the solution:

```bash
dotnet build TaskTracker.sln
```

Run the application:

```bash
dotnet run --project TaskTracker.Desktop
```

## Testing

I tested adding, editing, completing, and deleting tasks
on Windows and Ubuntu 24.04.

I also confirmed that tasks stayed saved after closing
and reopening the app on Windows.

On the Linux lab computer, I installed the SDK in my user
folder and used a temporary workspace because of the
account's storage quota.

## Saved Data

Tasks are saved in tasks.json inside the user's local
application-data folder under TaskTracker.

Windows and Linux keep their own saved task files.
The Linux lab test used temporary storage.
