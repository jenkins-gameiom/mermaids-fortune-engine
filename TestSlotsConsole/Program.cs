using System;

namespace TestSlotsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter Denom (1,5,10, 20)");
            int denom = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter Bet (88, 176, 264, 528, 880)");
            int betAmount = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter Math (94, 96)");
            string math = Console.ReadLine();
            Console.WriteLine("Please enter Spins amount");
            int spinsAmount = int.Parse(Console.ReadLine());
            TestSlotsDll testSlotsDll = new TestSlotsDll();
            testSlotsDll.RunSpins(denom, betAmount, math, spinsAmount);
            Console.ReadLine();
        }
    }
}
