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

        public ushort Day { get; set; }
        public ushort Month { get; set; }
        public ulong Year { get; set; }
    }
}