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
                if (task.SubTasks.Count.Equals(0)) continue;
                foreach (var subTask in task.SubTasks)
                    stringBuilder.Append($"\t* {subTask.ToString()}\n");
            }

            var tmp = stringBuilder.ToString();
            return tmp;
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
                throw new ArgumentOutOfRangeException(nameof(id));

            if (task.Id == id)
                _tasks.Remove(task);
            else
                task.SubTasks.Remove(task.SubTasks.First(s => s.Id == id));
        }

        public void Complete(ushort id) // /complete id
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id || t.SubTasks.Any(s => s.Id == id));

            if (task is null)
                throw new ArgumentOutOfRangeException(nameof(id));

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
            var completedTasks = from task in _tasks where task.Completed select task;

            foreach (var task in completedTasks)
            {
                Console.WriteLine($". {task.ToString()}");

                var completedSubTasks = from subtask in task.SubTasks where subtask.Completed select subtask;
                foreach (var subTask in completedSubTasks)
                    Console.WriteLine($"\t* {subTask.ToString()}");
            }
        }

        public void Today() // /today
        {
            var todayTasks = from task in _tasks
                where task.Deadline.Day == DateTime.Now.Day &&
                      task.Deadline.Month == DateTime.Now.Month &&
                      task.Deadline.Year == (ulong) DateTime.Now.Year
                select task;

            foreach (var task in todayTasks)
            {
                Console.WriteLine($". {task.ToString()}");

                var todaySubTasks = from subTask in task.SubTasks
                    where subTask.Deadline.Day == DateTime.Now.Day &&
                          subTask.Deadline.Month == DateTime.Now.Month &&
                          subTask.Deadline.Year == (ulong) DateTime.Now.Year
                    select subTask;
                foreach (var subTask in todaySubTasks)
                    Console.WriteLine($"\t* {subTask.ToString()}");
            }
        }

        public void AddSubTask(Task subtask, ushort id) // /add-subtask subtask-info
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task is null)
                throw new ArgumentOutOfRangeException(nameof(id));

            task.AddSubTask(subtask);
        }

        public void Load(string filename = "input.txt") // /load filename.txt
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("This file doesn't exist");

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
                        var d = Convert.ToUInt16(line.Substring(found - 2, 2));
                        var m = Convert.ToUInt16(line.Substring(found + 1, 2));
                        var y = Convert.ToUInt64(line.Substring(found + 4, 4));
                        line = line.Remove(found - 2, 10).Trim();
                        Add(new Task(line, new DeadLine(d, m, y)));
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
                        var d = Convert.ToUInt16(line.Substring(found - 2, 2));
                        var m = Convert.ToUInt16(line.Substring(found + 1, 2));
                        var y = Convert.ToUInt64(line.Substring(found + 4, 4));
                        line = line.Remove(found - 2, 10).Trim();
                        _tasks.Last().AddSubTask(new Task(line, new DeadLine(d, m, y)));
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