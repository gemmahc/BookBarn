using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Crawler
{
    /// <summary>
    /// The available outcomes to an action.
    /// </summary>
    public enum Result
    {
        None, // Default - not used.
        Pending, // Pending execution of the action.
        InProgress, // Execution of the action is in progress.
        Success, // Execution was successful.
        Failure, /// Execution failed.
    }
}
