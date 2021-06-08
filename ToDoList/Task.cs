using System.Collections.Generic;

namespace ToDoList
{
    internal class Task
    {
        private static ushort
            _identifier; // идентификтор, котрорый постянно будет инкрементиться при добавлении задачи => постоянно будет уникальный

        public bool Completed; // true - задача выполнена, false - невыполнена
        public ushort CountOfCompletedSubTasks;

        public List<Task> SubTasks; // Лист подзадач

        public Task(string text)
        {
            SubTasks = new List<Task>();
            Deadline = new DeadLine();
            Id = _identifier;
            Completed = false;
            CountOfCompletedSubTasks = 0;
            Text = text;
        }

        public Task(string text, DeadLine deadline) : this(text)
        {
            Deadline = deadline;
        }

        public ushort Id { get; } // Уникальный идентификатор для каждой задачи
        public string Text { get; } // Суть задачи
        public DeadLine Deadline { get; } // Срок сдачи

        public static void IncrementIdentifier()
        {
            _identifier++; // "генерация" уникального id
        }

        public void AddSubTask(Task task)
        {
            SubTasks.Add(task);
            IncrementIdentifier();
        }

        public string ToString()
        {
            var task = $"{Text} ";

            if (!Deadline.Day.Equals(0) || !Deadline.Month.Equals(0) ||
                !Deadline.Year.Equals(0))
                task += $"{Deadline.Day}.{Deadline.Month}.{Deadline.Year} ";

            if (!SubTasks.Count.Equals(0))
            {
                task += $"{CountOfCompletedSubTasks}/{SubTasks.Count}";
            }
            else
            {
                if (Completed)
                    task += "Done! ";
            }

            task += $"(id = {Id})";
            return task;
        }
    }
}