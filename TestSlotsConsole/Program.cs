using System;

namespace TestSlotsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSlotsDll testSlotsDll = new TestSlotsDll();
            Console.WriteLine("For Regular bet 50 bet, 1 denom, 96 math, press 1\n");
            Console.WriteLine("To enter manually press 2");
            int choice = int.Parse(Console.ReadLine());
            if (choice == 1)
            {
                Console.WriteLine("\nfor 1 million spins press 1");
                Console.WriteLine("for 3 million spins press 2");
                Console.WriteLine("for 5 million spins press 3");
                Console.WriteLine("for 10 million spins press 4");
                Console.WriteLine("for 20 million spins press 5");
                Console.WriteLine("for 50 million spins press 6");
                Console.WriteLine("for 100 million spins press 7");
                Console.WriteLine("for 500 million spins press 8");
                Console.WriteLine("for 500 thousands spins press 9");
                choice = int.Parse(Console.ReadLine());
                int spinsAmount = 0;
                switch (choice)
                {
                    case 1:
                        spinsAmount = 1000000;
                        break;
                    case 2:
                        spinsAmount = 3000000;
                        break;
                    case 3:
                        spinsAmount = 5000000;
                        break;
                    case 4:
                        spinsAmount = 10000000;
                        break;
                    case 5:
                        spinsAmount = 20000000;
                        break;
                    case 6:
                        spinsAmount = 50000000;
                        break;
                    case 7:
                        spinsAmount = 100000000;
                        break;
                    case 8:
                        spinsAmount = 500000000;
                        break;
                    case 9:
                        spinsAmount = 500000;
                        break;
                    default:
                        break;

                }
                testSlotsDll.RunSpins(1, 50, "96", spinsAmount);
                Console.ReadLine();
            }
            if (choice == 2)
            {
                Console.WriteLine("Please enter Denom (1,5,10, 20)");
                int denom = int.Parse(Console.ReadLine());
                Console.WriteLine("Please enter Bet (50, 100, 150, 250, 500)");
                int betAmount = int.Parse(Console.ReadLine());
                Console.WriteLine("Please enter Math (94, 96)");
                string math = Console.ReadLine();
                Console.WriteLine("Please enter Spins amount");
                int spinsAmount = int.Parse(Console.ReadLine());
                
                testSlotsDll.RunSpins(denom, betAmount, math, spinsAmount);
                Console.ReadLine();
            }
        }
    }
}
