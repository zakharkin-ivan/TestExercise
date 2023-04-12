namespace TestExercise.Controllers.Log
{
    /// <summary>
    /// <b>Не менять.</b>
    /// </summary>
    internal class ConsoleLogController : LogControllerBase
    {
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        public ConsoleLogController() : base(Console.Out)  {   }
    }
}
