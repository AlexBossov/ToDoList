namespace ToDoList
{
    internal class DeadLine
    {
        public DeadLine(ushort day = 0, ushort month = 0, ulong year = 0)
        {
            if (day > 31 || month > 12 || year == 0 && year < 2021) return;
            Day = day;
            Month = month;
            Year = year;
        }

        public ushort Day { get; }
        public ushort Month { get; }
        public ulong Year { get; }
    }
}