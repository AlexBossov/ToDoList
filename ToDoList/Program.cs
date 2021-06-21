using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDoList
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var taskGroups = new List<TaskGroup>();
            var mainGroup = new TaskGroup();
            taskGroups.Add(mainGroup); // mainGroup - задачи, которые не находятся в какой-то группе

            try
            {
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line is null)
                        continue;
                    var find = line.IndexOf(' ');
                    var command = find.Equals(-1) ? line : line[..find];

                    ushort id; // часто использующиеся переменные
                    int index;
                    bool check;
                    switch (command)
                    {
                        case "/all":
                            Console.Write(taskGroups.First().ToString()); // First - задачи без группы
                            for (index = 1; index < taskGroups.Count; index++)
                            {
                                var taskGroup = taskGroups[index];

                                Console.Write($"{taskGroup.Name}\n   ");
                                var tmp = new StringBuilder(taskGroup.ToString());
                                tmp.Replace("\n", "\n   ");
                                Console.WriteLine(tmp);
                            }

                            break;
                        case "/add":
                            line = line.Remove(0, 4).Trim();
                            index = line.IndexOf('.');

                            if (index.Equals(-1))
                            {
                                taskGroups.First().Add(new Task(line));
                            }
                            else
                            {
                                var deadLine = new DeadLine(line.Substring(index - 2, 10));
                                line = line.Remove(index - 2, 10).Trim();
                                taskGroups.First().Add(new Task(line, deadLine));
                            }

                            break;
                        case "/del":
                            line = line.Remove(0, 4).Trim();
                            id = Convert.ToUInt16(line);
                            check = false;
                            foreach (var taskGroup in taskGroups)
                            {
                                check = taskGroup.Delete(id);
                                if (check) break;
                            }

                            if (!check)
                                throw new ArgumentOutOfRangeException(
                                    "List haven't this id"); // Это подходящий exception?
                            break;
                        case "/load": // Надо переделать
                            line = line.Remove(0, 5).Trim();
                            taskGroups.First().Load(line);
                            break;
                        case "/save": // Надо переделать
                            line = line.Remove(0, 5).Trim();
                            using (var sw = new StreamWriter(line, false, Encoding.Default))
                            {
                                sw.WriteLine(taskGroups.First());
                            }

                            break;
                        case "/today":
                            foreach (var taskGroup in taskGroups)
                                Console.WriteLine(taskGroup.Today());
                            break;
                        case "/complete":
                            line = line.Remove(0, 9).Trim();
                            var id2 = Convert.ToInt32(line);

                            check = false;
                            foreach (var taskGroup in taskGroups)
                            {
                                check = taskGroup.Complete((ushort) id2);
                                if (check) break;
                            }

                            if (!check)
                                throw new ArgumentOutOfRangeException(
                                    "List haven't this id"); // Это подходящий exception?
                            break;
                        case "/completed":
                            foreach (var taskGroup in taskGroups)
                                Console.WriteLine(taskGroup.Completed());
                            break;
                        case "/add-subtask":
                            line = line.Remove(0, 12).Trim();
                            var found2 = line.IndexOf(' ');
                            if (found2 != -1)
                            {
                                var id3 = int.Parse(line[..found2]);
                                line = line.Remove(0, found2 + 1);

                                index = line.IndexOf('.');
                                if (index.Equals(-1))
                                {
                                    taskGroups.First().AddSubTask(new Task(line), (ushort) id3);
                                }
                                else
                                {
                                    var deadLine = new DeadLine(line.Substring(index - 2, 10));
                                    line = line.Remove(index - 2, 10).Trim();
                                    taskGroups.First().AddSubTask(new Task(line, deadLine), (ushort) id3);
                                }
                            }
                            else
                            {
                                throw new Exception("There isn't this id\n");
                            }

                            break;
                        case "/create-group":
                            taskGroups.Add(new TaskGroup(line.Substring(13, line.Trim().Length - 13).Trim()));
                            break;
                        case "/delete-group":
                            var group = taskGroups.FirstOrDefault(taskGroup =>
                                taskGroup.Name.Equals(line.Substring(13, line.Trim().Length - 13).Trim()));
                            // Console.WriteLine(line.Substring(13, line.Trim().Length - 13));
                            if (group is null)
                                break;
                            foreach (var task in group._tasks)
                                taskGroups.First().Add(task);
                            taskGroups.Remove(group);
                            break;
                        case "/add-to-group":
                            line = line.Remove(0, 13).Trim();
                            index = line.IndexOf(' ');
                            id = Convert.ToUInt16(line[..index]);
                            line = line.Remove(0, index + 1).Trim();
                            var g = taskGroups.FirstOrDefault(taskGroup => taskGroup.Name.Equals(line));
                            Task t = null;
                            // Как заменить конструкцию цикла?)
                            foreach (var task1 in from taskGroup in taskGroups
                                from task1 in taskGroup._tasks
                                where task1.Id.Equals(id)
                                select task1)
                                t = task1;
                            if (t is null)
                                break;
                            taskGroups.First()._tasks.Remove(t);
                            g.Add(t);
                            break;
                        case "/delete-from-group":
                            line = line.Remove(0, 18).Trim();
                            if (line is null)
                                break;
                            index = line.IndexOf(' ');
                            id = Convert.ToUInt16(line[..index]);
                            line = line.Remove(0, index + 1).Trim();
                            group = taskGroups.FirstOrDefault(g => g.Name.Equals(line));
                            var tAsk = group?._tasks.FirstOrDefault(task => task.Id.Equals(id));
                            if (tAsk is null)
                                break;
                            group._tasks.Remove(tAsk);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }
        }
    }
}