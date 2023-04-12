namespace TestExercise.Controllers.Log
{
    /// <summary>
    /// <b>Не менять.</b>
    /// </summary>
    internal abstract class LogControllerBase
    {
        protected static LogControllerBase? instance = null;
        protected readonly TextWriter stream;
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        public LogControllerBase(TextWriter writer)
        {
            stream = writer;
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        public async Task LogAsync(string msg)
            => await stream.WriteLineAsync($"{DateTime.Now:dd.MM.yyyy}\t-\t{msg}");
    }
}