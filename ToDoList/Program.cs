using System;
using System.IO;
using System.Text;

namespace ToDoList
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var taskGroup = new TaskGroup();

            // var taskGroups = new List<TaskGroup>();
            // var mainGroup = new TaskGroup();
            // taskGroups.Add(mainGroup);

            try
            {
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line is null)
                        continue;
                    var find = line.IndexOf(' ');
                    var command = find.Equals(-1) ? line : line[..find];

                    switch (command)
                    {
                        case "/all":
                            Console.Write(taskGroup
                                .ToString()); // Если делаю без ToString(), то выводится ToDoList.TaskGroup
                            break;
                        case "/add":
                            line = line.Remove(0, 4).Trim();
                            var found1 = line.IndexOf('.');

                            if (found1.Equals(-1))
                            {
                                taskGroup.Add(new Task(line));
                            }
                            else
                            {
                                var deadLine = new DeadLine(line.Substring(found1 - 2, 10));
                                line = line.Remove(found1 - 2, 10).Trim();
                                taskGroup.Add(new Task(line, deadLine));
                            }

                            break;
                        case "/del":
                            line = line.Remove(0, 4).Trim();
                            var id1 = Convert.ToInt32(line);
                            taskGroup.Delete((ushort) id1);
                            break;
                        case "/load":
                            line = line.Remove(0, 5).Trim();
                            taskGroup.Load(line);
                            break;
                        case "/save":
                            line = line.Remove(0, 5).Trim();
                            using (var sw = new StreamWriter(line, false, Encoding.Default))
                            {
                                sw.WriteLine(taskGroup);
                            }

                            break;
                        case "/today":
                            taskGroup.Today();
                            break;
                        case "/complete":
                            line = line.Remove(0, 9).Trim();
                            var id2 = Convert.ToInt32(line);
                            taskGroup.Complete((ushort) id2);
                            break;
                        case "/completed":
                            taskGroup.Completed();
                            break;
                        case "/add-subtask":
                            line = line.Remove(0, 12).Trim();
                            var found2 = line.IndexOf(' ');
                            if (found2 != -1)
                            {
                                var id3 = int.Parse(line[..found2]);
                                line = line.Remove(0, found2 + 1);

                                var index = line.IndexOf('.');
                                if (index.Equals(-1))
                                {
                                    taskGroup.AddSubTask(new Task(line), (ushort) id3);
                                }
                                else
                                {
                                    var deadLine = new DeadLine(line.Substring(index - 2, 10));
                                    line = line.Remove(index - 2, 10).Trim();
                                    taskGroup.AddSubTask(new Task(line, deadLine), (ushort) id3);
                                }
                            }
                            else
                            {
                                throw new Exception("There isn't this id\n");
                            }

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