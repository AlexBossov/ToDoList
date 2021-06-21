using System;

namespace ToDoList
{
    internal class DeadLine
    {
        public DeadLine(ushort day = 0, ushort month = 0, ulong year = 0)
        {
            if (day > 31 || month > 12 || year is 0 and < 2021) return;
            Day = day;
            Month = month;
            Year = year;
        }

        public DeadLine(string deadLine)
        {
            if (deadLine.Length != 10)
                throw new ArgumentException("DeadLine is Invalid");

            var day = Convert.ToUInt16(deadLine[..2]);
            var month = Convert.ToUInt16(deadLine.Substring(3, 2));
            var year = Convert.ToUInt64(deadLine.Substring(6, 4));

            if (day > 31 || month > 12 || year is 0 and < 2021)
                return;

            Day = day;
            Month = month;
            Year = year;
        }

        public ushort Day { get; set; }
        public ushort Month { get; set; }
        public ulong Year { get; set; }
    }
}