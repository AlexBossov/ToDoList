using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDoList
{
    internal class TaskGroup
    {
        public readonly List<Task> _tasks;

        public TaskGroup(string name = "main")
        {
            _tasks = new List<Task>();
            Name = name;
        }

        public string Name { get; set; }

        public string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var task in _tasks)
            {
                stringBuilder.Append($". {task.ToString()}\n");
                foreach (var subTask in task.SubTasks)
                    stringBuilder.Append($"\t* {subTask.ToString()}\n");
            }

            return stringBuilder.ToString();
        }

        public void Add(Task task) // /add task-info
        {
            if (_tasks.Any(task1 => task.Text.Equals(task1.Text))) return;
            _tasks.Add(task);
            Task.IncrementIdentifier();
        }

        public bool Delete(ushort id) // /del id, false - такого id нет
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id || t.SubTasks.Any(s => s.Id == id));

            if (task is null)
                return false;

            if (task.Id == id)
            {
                _tasks.Remove(task);
            }
            else
            {
                var subTask = task.SubTasks.FirstOrDefault(s => s.Id == id);
                if (subTask is null)
                    return false;
                task.SubTasks.Remove(subTask);
            }

            return true;
        }

        public bool Complete(ushort id) // /complete id, false - такого id нет
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id || t.SubTasks.Any(s => s.Id == id));

            if (task is null)
                return false;

            if (task.Id == id)
            {
                task.Completed = true;
            }
            else
            {
                var subTask = task.SubTasks.FirstOrDefault(subTask1 => subTask1.Id.Equals(id));

                if (subTask is null)
                    return false;

                subTask.Completed = true;
                task.CountOfCompletedSubTasks++;
                if (task.CountOfCompletedSubTasks.Equals((ushort) task.SubTasks.Count) &&
                    !task.SubTasks.Count.Equals(0)) task.Completed = true;
            }

            return true;
        }

        public string Completed() // /completed
        {
            var completedTasks = _tasks.Where(task => task.Completed);

            var stringBuilder = new StringBuilder();

            foreach (var task in completedTasks)
            {
                stringBuilder.Append($". {task.ToString()}");

                var completedSubTasks = task.SubTasks.Where(subtask => subtask.Completed);
                foreach (var subTask in completedSubTasks)
                    stringBuilder.Append($"\t* {subTask.ToString()}");
            }

            return stringBuilder.ToString();
        }

        public string Today() // /today
        {
            var todayTasks = _tasks.Where(task => task.Deadline.Day == DateTime.Now.Day &&
                                                  task.Deadline.Month == DateTime.Now.Month &&
                                                  task.Deadline.Year == (ulong) DateTime.Now.Year);

            var stringBuilder = new StringBuilder();

            foreach (var task in todayTasks)
            {
                stringBuilder.Append($". {task.ToString()}");
                var todaySubTasks = task.SubTasks.Where(subTask => subTask.Deadline.Day == DateTime.Now.Day &&
                                                                   subTask.Deadline.Month == DateTime.Now.Month &&
                                                                   subTask.Deadline.Year == (ulong) DateTime.Now.Year);
                foreach (var subTask in todaySubTasks)
                    stringBuilder.Append($"\t* {subTask.ToString()}");
            }

            return stringBuilder.ToString();
        }

        public void AddSubTask(Task subtask, ushort id) // /add-subtask subtask-info
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task is null)
                throw new ArgumentOutOfRangeException("There isn't this id");

            task.AddSubTask(subtask);
        }

        public void Load(string filename = "input.txt") // /load filename.txt Надо переделать
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("This file doesn't exist");

            _tasks.Clear(); // Подумал, что при считывании файла, надо не просто добавлять задачи, а обновлять все)

            using var sr = new StreamReader(filename, Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // string task;
                line = line.Remove(0, 1);
                if (!line[0].Equals('*'))
                {
                    var found = line.IndexOf('.');
                    if (found != -1)
                    {
                        var deadLine = new DeadLine(line.Substring(found - 2, 10));
                        line = line.Remove(found - 2, 10).Trim();
                        Add(new Task(line, deadLine));
                    }
                    else
                    {
                        line = line.Trim();
                        Add(new Task(line));
                    }
                }
                else
                {
                    line = line.Remove(0, 1);
                    var found = line.IndexOf('.');
                    if (found != -1)
                    {
                        var deadLine = new DeadLine(line.Substring(found - 2, 10));
                        line = line.Remove(found - 2, 10).Trim();
                        _tasks.Last().AddSubTask(new Task(line, deadLine));
                    }
                    else
                    {
                        line = line.Trim();
                        _tasks.Last().AddSubTask(new Task(line));
                    }
                }
            }
        }
    }
}