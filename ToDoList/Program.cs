using System;

namespace ToDoList
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var groupOfTasks = new GroupOfTasks();

            try
            {
                while (true)
                {
                    var line = Console.ReadLine();
                    var find = line.IndexOf(' ');
                    string command;
                    command = find.Equals(-1) ? line : line[..find];

                    switch (command)
                    {
                        case "/all":
                            groupOfTasks.Print();
                            break;
                        case "/add":
                            line = line.Remove(0, 4).Trim();
                            var found1 = line.IndexOf('.');

                            if (found1.Equals(-1))
                            {
                                groupOfTasks.Add(new Task(line));
                            }
                            else
                            {
                                var d = Convert.ToUInt16(line.Substring(found1 - 2, 2));
                                var m = Convert.ToUInt16(line.Substring(found1 + 1, 2));
                                var y = Convert.ToUInt64(line.Substring(found1 + 4, 4));
                                line = line.Remove(found1 - 2, 10).Trim();
                                groupOfTasks.Add(new Task(line, new DeadLine(d, m, y)));
                            }

                            break;
                        case "/del":
                            line = line.Remove(0, 4).Trim();
                            var id1 = Convert.ToInt32(line);
                            if (id1 < 0)
                                throw new Exception("id must be > 0!");
                            groupOfTasks.Delete((ushort) id1);
                            break;
                        case "/load":
                            line = line.Remove(0, 5).Trim();
                            groupOfTasks.Load(line);
                            break;
                        case "/save":
                            line = line.Remove(0, 5).Trim();
                            groupOfTasks.PrintToFile(line);
                            break;
                        case "/today":
                            groupOfTasks.Today();
                            break;
                        case "/complete":
                            line = line.Remove(0, 9).Trim();
                            var id2 = Convert.ToInt32(line);

                            if (id2 < 0) throw new Exception("id must be >= 0");
                            groupOfTasks.Complete((ushort) id2);
                            break;
                        case "/completed":
                            groupOfTasks.Completed();
                            break;
                        case "/add-subtask":
                            line = line.Remove(0, 12).Trim();
                            var found2 = line.IndexOf(' ');
                            if (found2 != -1)
                            {
                                var id3 = int.Parse(line.Substring(0, found2));
                                line = line.Remove(0, found2 + 1);

                                var index = line.IndexOf('.');
                                if (index.Equals(-1))
                                {
                                    groupOfTasks.AddSubTask(new Task(line), (ushort) id3);
                                }
                                else
                                {
                                    var d = Convert.ToUInt16(line.Substring(index - 2, 2));
                                    var m = Convert.ToUInt16(line.Substring(index + 1, 2));
                                    var y = Convert.ToUInt64(line.Substring(index + 4, 4));
                                    line = line.Remove(index - 2, 10).Trim();
                                    groupOfTasks.AddSubTask(new Task(line, new DeadLine(d, m, y)), (ushort) id3);
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
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}