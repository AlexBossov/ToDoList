using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToDoList
{
    class DeadLine // Класс для удобного хранения deadline
    {
        private ushort Day;
        private ushort Month;
        private ulong Year;

        public DeadLine(ushort day = 0, ushort month = 0, ulong year = 0)
        {
            try
            {
                if (day > 31 || month > 12 || year != 0 && year < 2021)
                    throw new ArgumentException("Imposible date!\n");

                Day = day;
                Month = month;
                Year = year;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ushort GetDay() => Day;
        public ushort GetMonth() => Month;
        public ulong GetYear() => Year;
    }

    class Task // Класс для хранения задачи
    {
        private static ushort Identifier = 0; // идентификтор, котрорый постянно будет инкрементиться при добавлении задачи => постоянно будет уникальный

        public List<Task> SubTasks; // Лист подзадач
        private ushort Id; // Уникальный идентификатор для каждой задачи
        private string Text; // Суть задачи
        private DeadLine Deadline; // Срок сдачи
        public bool Completed; // true - задача выполнена, false - невыполнена
        public ushort CountOfCompletedSubTasks;

        public Task()
        {
            SubTasks = new List<Task>();
            Deadline = new DeadLine();
            Id = Identifier;
            Completed = false;
            CountOfCompletedSubTasks = 0;
        }

        public Task(string text) : this() => Text = text;
        public Task(string text, DeadLine deadline) : this(text) => Deadline = deadline;

        public string GetText() => Text;
        public DeadLine GetDeadline() => Deadline;
        public ushort GetId() => Id;

        public static void IncrementIdentifier() => Identifier++; // "генерация" уникального id

        public void AddSubTask(Task task)
        {
            SubTasks.Add(task);
            Task.IncrementIdentifier();
        }
    }

    class GroupOfTasks // Группа задач
    {
        private List<Task> Tasks;

        public GroupOfTasks() => Tasks = new List<Task>();

        private void AllOfTaskToConsole(Task task)
        {
            Console.Write($"{task.GetText()} ");

            if (!task.GetDeadline().GetDay().Equals(0) || !task.GetDeadline().GetMonth().Equals(0) ||
                !task.GetDeadline().GetYear().Equals(0))
                Console.Write(
                    $"{task.GetDeadline().GetDay()}.{task.GetDeadline().GetMonth()}.{task.GetDeadline().GetYear()} ");

            if (!task.SubTasks.Count.Equals(0))
                Console.Write($"{task.CountOfCompletedSubTasks}/{task.SubTasks.Count}");
            else
            {
                if (task.Completed)
                    Console.Write("Done! ");
            }
            
            Console.WriteLine($"(id = {task.GetId()})");
        }

        public void AllToConslole() // /all
        {
            foreach (Task task in Tasks)
            {
                Console.Write(". ");
                AllOfTaskToConsole(task);
                if (!task.SubTasks.Count.Equals(0))
                    foreach (Task subTask in task.SubTasks)
                    {
                        Console.Write("\t* ");
                        AllOfTaskToConsole(subTask);
                    }
            }
        }

        public void AllToFile(string filename = "output.txt") // /save filename.txt
        {
            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
            {
                foreach (Task task in Tasks)
                {
                    sw.Write(". ");
                    sw.Write($"{task.GetText()} ");

                    if (!task.GetDeadline().GetDay().Equals(0) || !task.GetDeadline().GetMonth().Equals(0) ||
                        !task.GetDeadline().GetYear().Equals(0))
                        sw.Write(
                            $"{task.GetDeadline().GetDay()}.{task.GetDeadline().GetMonth()}.{task.GetDeadline().GetYear()} ");

                    if (!task.SubTasks.Count.Equals(0))
                        sw.Write($"{task.CountOfCompletedSubTasks}/{task.SubTasks.Count}");
                    else
                    {
                        if (task.Completed)
                            sw.Write("Done! ");
                    }
                    Console.WriteLine($"(id = {task.GetId()})");
                    sw.WriteLine();
                    
                    foreach (Task subTask in task.SubTasks)
                    {
                        sw.Write("\t* ");
                        sw.Write($"{subTask.GetText()} ");

                        if (!subTask.GetDeadline().GetDay().Equals(0) || !subTask.GetDeadline().GetMonth().Equals(0) ||
                            !subTask.GetDeadline().GetYear().Equals(0))
                            sw.Write(
                                $"{subTask.GetDeadline().GetDay()}.{subTask.GetDeadline().GetMonth()}.{subTask.GetDeadline().GetYear()} ");
                        
                        if (subTask.Completed)
                            sw.Write("Done! ");
                        
                        Console.WriteLine($"(id = {subTask.GetId()})");
                        sw.WriteLine();
                    }
                }
            }
        }

        public void Add(Task task) // /add task-info
        {
            Tasks.Add(task);
            Task.IncrementIdentifier();
        }

        public void Delete(ushort id) // /del id
        {
            bool check = false; // Проверка на наоичие данного id, если такого нет, кидаем исключение

            foreach (Task task in Tasks)
            {
                if (task.GetId().Equals(id))
                {
                    Tasks.Remove(task);
                    check = true;
                    break;
                }

                if (task.SubTasks.Count > 0)
                    foreach (Task subTask in task.SubTasks)
                        if (subTask.GetId().Equals(id))
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
            bool check = false;
            
            foreach (Task task in Tasks)
            {
                if (task.GetId().Equals(id))
                {
                    task.Completed = true;
                    check = true;
                    break;
                }

                foreach (Task subTask in task.SubTasks)
                    if (subTask.GetId().Equals(id))
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
            // Тут точно по заданию непонятно надо ли выводить выполненные подзадачи, поэтому я не вывожу их, если сама задача не выполнена
            foreach (Task task in Tasks)
            {
                if (task.Completed)
                {
                    Console.Write(". ");
                    AllOfTaskToConsole(task);
                    foreach (Task subTask in task.SubTasks)
                    {
                        if (subTask.Completed)
                        {
                            Console.Write("\t* ");
                            AllOfTaskToConsole(subTask);
                        }
                    }
                }
            }
        }

        public void Today() // /today
        {
            foreach (Task task in Tasks)
            {
                if (task.GetDeadline().GetDay() == DateTime.Now.Day &&
                    task.GetDeadline().GetMonth() == DateTime.Now.Month &&
                    task.GetDeadline().GetYear() == (ulong) DateTime.Now.Year)
                {
                    Console.Write(". ");
                    AllOfTaskToConsole(task);
                }

                foreach (Task subTask in task.SubTasks)
                {
                    if (subTask.GetDeadline().GetDay() == DateTime.Now.Day &&
                        subTask.GetDeadline().GetMonth() == DateTime.Now.Month &&
                        subTask.GetDeadline().GetYear() == (ulong) DateTime.Now.Year)
                    {
                        Console.Write("\t* ");
                        AllOfTaskToConsole(subTask);
                    }
                }
            }
        }

        public void AddSubTask(Task subtask, ushort id) // /add-subtask subtask-info
        {
            foreach (Task task in Tasks)
                if (task.GetId().Equals(id))
                {
                    task.AddSubTask(subtask);
                    break;
                }
        }

        public void Load(string filename = "input.txt") // /load filename.txt
        {
            try
            {
                if (!File.Exists(filename))
                    throw new ArgumentException("This file doesn't exist");
                
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                     // string task;
                        line = line.Remove(0, 1);
                        if (!line[0].Equals('*'))
                        {
                            int found = line.IndexOf('.');
                            if (found != -1)
                            {
                                ushort d = Convert.ToUInt16(line.Substring(found - 2, 2));
                                ushort m = Convert.ToUInt16(line.Substring(found + 1, 2));
                                ulong y = Convert.ToUInt64(line.Substring(found + 4, 4));
                                line = line.Remove(found - 2, 10).Trim();
                                this.Add(new Task(line, new DeadLine(d, m, y)));
                            }
                            else
                            {
                                line = line.Trim();
                                this.Add(new Task(line));
                            }
                        }
                        else
                        {
                            line = line.Remove(0, 1);
                            int found = line.IndexOf('.');
                            if (found != -1)
                            {
                                ushort d = Convert.ToUInt16(line.Substring(found - 2, 2));
                                ushort m = Convert.ToUInt16(line.Substring(found + 1, 2));
                                ulong y = Convert.ToUInt64(line.Substring(found + 4, 4));
                                line = line.Remove(found - 2, 10).Trim();
                                Tasks.Last().AddSubTask(new Task(line, new DeadLine(d, m, y)));
                            }
                            else
                            {
                                line = line.Trim();
                                Tasks.Last().AddSubTask(new Task(line));
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

    class Program
    {
        static void Main(string[] args)
        {
            GroupOfTasks groupOfTasks = new GroupOfTasks();

            while (true)
            {
                string line = Console.ReadLine();
                if (line != null)
                {
                    if (line.Length > 12 && line.Substring(0, 12).Equals("/add-subtask"))
                    {
                        line = line.Remove(0, 12).Trim();
                        int found = line.IndexOf(' ');
                        if(found != -1)
                        {
                            int id = int.Parse(line.Substring(0, found));
                            line = line.Remove(0, found + 1);
                            
                            int index = line.IndexOf('.');
                            if (index.Equals(-1))
                                groupOfTasks.AddSubTask(new Task(line), (ushort)id);
                            else
                            {
                                ushort d = Convert.ToUInt16(line.Substring(index - 2, 2));
                                ushort m = Convert.ToUInt16(line.Substring(index + 1, 2));
                                ulong y = Convert.ToUInt64(line.Substring(index + 4, 4));
                                line = line.Remove(index - 2, 10).Trim();
                                groupOfTasks.AddSubTask(new Task(line, new DeadLine(d, m, y)), (ushort)id);
                            }
                        }
                        else
                        {
                            try
                            {
                                throw new Exception("There isn't this id\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                throw;
                            }
                        }

                    }
                    
                    if (line.Length.Equals(10) && line.Substring(0, 10).Equals("/completed"))
                    {
                        groupOfTasks.Completed();
                        continue;
                    }

                    if (line.Length > 8 && line.Substring(0, 9).Equals("/complete"))
                    {
                        line = line.Remove(0, 9).Trim();
                        int id = Convert.ToInt32(line);

                        try
                        {
                            if (id < 0) throw new Exception("id must be >= 0");
                            groupOfTasks.Complete((ushort) id);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                    }

                    if (line.Length.Equals(6) && line.Substring(0, 6).Equals("/today"))
                    {
                        groupOfTasks.Today();
                        continue;
                    }

                    if (line.Length > 5)
                    {
                        if (line.Substring(0, 5).Equals("/load"))
                        {
                            line = line.Remove(0, 5).Trim();
                            groupOfTasks.Load(line);
                            continue;
                        }

                        if (line.Substring(0, 5).Equals("/save"))
                        {
                            line = line.Remove(0, 5).Trim();
                            groupOfTasks.AllToFile(line);
                            continue;
                        }
                    }

                    if (line.Length >= 4)
                    {
                        if (line.Substring(0, 4).Equals("/all"))
                        {
                            Console.WriteLine(line.Length);
                            groupOfTasks.AllToConslole();
                            continue;
                        }

                        if (line.Substring(0, 4).Equals("/del"))
                        {
                            line = line.Remove(0, 4).Trim();
                            int id = Convert.ToInt32(line);

                            try
                            {
                                if (id < 0)
                                    throw new Exception("id must be > 0!");
                                groupOfTasks.Delete((ushort) id);
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                        }

                        if (line.Substring(0, 4).Equals("/add"))
                        {
                            line = line.Remove(0, 4).Trim();
                            int found = line.IndexOf('.');

                            if (found.Equals(-1))
                                groupOfTasks.Add(new Task(line));
                            else
                            {
                                ushort d = Convert.ToUInt16(line.Substring(found - 2, 2));
                                ushort m = Convert.ToUInt16(line.Substring(found + 1, 2));
                                ulong y = Convert.ToUInt64(line.Substring(found + 4, 4));
                                line = line.Remove(found - 2, 10).Trim();
                                groupOfTasks.Add(new Task(line, new DeadLine(d, m, y)));
                            }

                            continue;
                        }
                    }
                }
            }
        }
    }
}