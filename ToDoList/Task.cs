using System.Collections.Generic;
using System.Text;

namespace ToDoList
{
    internal class Task
    {
        private static ushort
            _identifier; // идентификтор, котрорый постянно будет инкрементиться при добавлении задачи => постоянно будет уникальный

        public readonly List<Task> SubTasks;

        public bool Completed; // true - задача выполнена, false - невыполнена
        public ushort CountOfCompletedSubTasks;

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
        public string Text { get; }
        public DeadLine Deadline { get; }

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
            var stringBuilder = new StringBuilder($"{Text} ");

            if (!Deadline.Day.Equals(0) || !Deadline.Month.Equals(0) ||
                !Deadline.Year.Equals(0))
                stringBuilder.Append($"{Deadline.Day}.{Deadline.Month}.{Deadline.Year} ");

            if (Completed && !SubTasks.Count.Equals(0))
            {
                stringBuilder.Append($"{SubTasks.Count}/{SubTasks.Count}");
                foreach (var subTask in SubTasks)
                    subTask.Completed = true;
            }
            else if (!SubTasks.Count.Equals(0))
            {
                stringBuilder.Append($"{CountOfCompletedSubTasks}/{SubTasks.Count}");
            }
            else if (Completed)
            {
                stringBuilder.Append("Done! ");
            }

            stringBuilder.Append($"(id = {Id})");
            return stringBuilder.ToString();
        }
    }
}