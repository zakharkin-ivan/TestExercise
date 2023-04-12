using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExercise.Controllers.Log
{
    /// <summary>
    /// <b>Не менять.</b>
    /// </summary>
    internal class FileLogController : LogControllerBase
    {
        private const string LOG_FILE = "log.txt";
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        public FileLogController() : base(new StreamWriter(File.Open(LOG_FILE, FileMode.Truncate))) {   }
    }
}
