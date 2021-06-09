using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDoList
{
    internal class TaskGroup
    {
        private readonly List<Task> _tasks;

        public TaskGroup()
        {
            _tasks = new List<Task>();
        }

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
            // Не знаю как лучше сделать, если оставить так, то дедлайн не поменяешь, но кратко и красиво, если по-др то не оч красиво, стоит ли 
            // сделать возможной смену дедлайна?

            if (_tasks.Any(task1 => task.Text.Equals(task1.Text))) return;
            _tasks.Add(task);
            Task.IncrementIdentifier();
        }

        public void Delete(ushort id) // /del id
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id || t.SubTasks.Any(s => s.Id == id));

            if (task is null)
                throw new ArgumentOutOfRangeException("There isn't this id");

            if (task.Id == id)
                _tasks.Remove(task);
            else
                task.SubTasks.Remove(task.SubTasks.First(s => s.Id == id));
        }

        public void Complete(ushort id) // /complete id
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id || t.SubTasks.Any(s => s.Id == id));

            if (task is null)
                throw new ArgumentOutOfRangeException("There isn't this id");

            if (task.Id == id)
                task.Completed = true;
            else
                foreach (var subTask in task.SubTasks.Where(subTask => subTask.Id.Equals(id)))
                {
                    subTask.Completed = true;
                    task.CountOfCompletedSubTasks++;
                    if (task.CountOfCompletedSubTasks.Equals((ushort) task.SubTasks.Count) &&
                        !task.SubTasks.Count.Equals(0))
                        task.Completed = true;
                }
        }

        public void Completed() // /completed
        {
            var completedTasks = _tasks.Where(task => task.Completed);

            foreach (var task in completedTasks)
            {
                Console.WriteLine($". {task.ToString()}");

                var completedSubTasks = task.SubTasks.Where(subtask => subtask.Completed);
                foreach (var subTask in completedSubTasks)
                    Console.WriteLine($"\t* {subTask.ToString()}");
            }
        }

        public void Today() // /today
        {
            var todayTasks = _tasks.Where(task => task.Deadline.Day == DateTime.Now.Day &&
                                                  task.Deadline.Month == DateTime.Now.Month &&
                                                  task.Deadline.Year == (ulong) DateTime.Now.Year);
            foreach (var task in todayTasks)
            {
                Console.WriteLine($". {task.ToString()}");
                var todaySubTasks = task.SubTasks.Where(subTask => subTask.Deadline.Day == DateTime.Now.Day &&
                                                                   subTask.Deadline.Month == DateTime.Now.Month &&
                                                                   subTask.Deadline.Year == (ulong) DateTime.Now.Year);
                foreach (var subTask in todaySubTasks)
                    Console.WriteLine($"\t* {subTask.ToString()}");
            }
        }

        public void AddSubTask(Task subtask, ushort id) // /add-subtask subtask-info
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task is null)
                throw new ArgumentOutOfRangeException("There isn't this id");

            task.AddSubTask(subtask);
        }

        public void Load(string filename = "input.txt") // /load filename.txt
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