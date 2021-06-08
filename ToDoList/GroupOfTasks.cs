using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDoList
{
    internal class GroupOfTasks // Группа задач
    {
        private readonly List<Task> _tasks;

        public GroupOfTasks()
        {
            _tasks = new List<Task>();
        }

        public void Print() // /all
        {
            foreach (var task in _tasks)
            {
                Console.WriteLine($". {task.ToString()}");
                if (!task.SubTasks.Count.Equals(0))
                    foreach (var subTask in task.SubTasks)
                        Console.WriteLine($"\t* {subTask.ToString()}");
            }
        }

        public void PrintToFile(string filename = "output.txt") // /save filename.txt
        {
            using (var sw = new StreamWriter(filename, false, Encoding.Default))
            {
                foreach (var task in _tasks)
                {
                    sw.Write($". {task.ToString()}");
                    if (!task.SubTasks.Count.Equals(0))
                        foreach (var subTask in task.SubTasks)
                            sw.Write($"\t* {subTask.ToString()}");
                }
            }
        }

        public void Add(Task task) // /add task-info
        {
            _tasks.Add(task);
            Task.IncrementIdentifier();
        }

        public void Delete(ushort id) // /del id
        {
            var check = false; // Проверка на наличие данного id, если такого нет, кидаем исключение

            foreach (var task in _tasks)
            {
                if (task.Id.Equals(id))
                {
                    _tasks.Remove(task);
                    check = true;
                    break;
                }

                if (task.SubTasks.Count <= 0) continue;
                foreach (var subTask in task.SubTasks)
                    if (subTask.Id.Equals(id))
                    {
                        if (subTask.Completed)
                            task.CountOfCompletedSubTasks--;
                        task.SubTasks.Remove(subTask);
                        check = true;
                        break;
                    }
            }

            try
            {
                if (!check)
                    throw new Exception("List haven't this id!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Complete(ushort id) // /complete id
        {
            var check = false;

            foreach (var task in _tasks)
            {
                if (task.Id.Equals(id))
                {
                    task.Completed = true;
                    check = true;
                    break;
                }

                foreach (var subTask in task.SubTasks)
                    if (subTask.Id.Equals(id))
                    {
                        subTask.Completed = true;
                        task.CountOfCompletedSubTasks++;
                        check = true;
                        if (task.CountOfCompletedSubTasks.Equals((ushort) task.SubTasks.Count) &&
                            !task.SubTasks.Count.Equals(0))
                            task.Completed = true;
                        break;
                    }
            }

            try
            {
                if (!check)
                    throw new Exception("List haven't this id!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Completed() // /completed
        {
            var completedTasks = from task in _tasks where task.Completed select task;

            foreach (var task in completedTasks)
            {
                Console.Write($". {task.ToString()}");

                var completedSubTasks = from subtask in task.SubTasks where subtask.Completed select subtask;
                foreach (var subTask in completedSubTasks)
                    Console.Write($"\t* {subTask.ToString()}");
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
                Console.Write($". {task.ToString()}");

                var todaySubTasks = from subTask in task.SubTasks
                    where subTask.Deadline.Day == DateTime.Now.Day &&
                          subTask.Deadline.Month == DateTime.Now.Month &&
                          subTask.Deadline.Year == (ulong) DateTime.Now.Year
                    select subTask;
                foreach (var subTask in todaySubTasks)
                    Console.Write($"\t* {subTask.ToString()}");
            }
        }

        public void AddSubTask(Task subtask, ushort id) // /add-subtask subtask-info
        {
            var check = false;
            foreach (var task in _tasks)
                if (task.Id.Equals(id))
                {
                    task.AddSubTask(subtask);
                    check = true;
                    break;
                }

            try
            {
                if (!check)
                    throw new Exception("List haven't this id!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Load(string filename = "input.txt") // /load filename.txt
        {
            try
            {
                if (!File.Exists(filename))
                    throw new ArgumentException("This file doesn't exist");

                using (var sr = new StreamReader(filename, Encoding.Default))
                {
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
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}