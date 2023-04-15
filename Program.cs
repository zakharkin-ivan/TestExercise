using TestExercise.Controllers.Bank;
using TestExercise.Controllers.Log;

namespace TestExercise
{
    internal class Program
    {
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BankManager bankManager = new(new(), new(), new ConsoleLogController());
            while (true) 
            {
                Console.WriteLine(new string('-', 100));
                bankManager.Update();
                Task.Delay(10000).Wait();
            }
        }
    }
}