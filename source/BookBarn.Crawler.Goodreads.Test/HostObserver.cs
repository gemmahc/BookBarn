using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class HostObserver : IObserver<HostInfo>
    {
        public void OnCompleted()
        {
            Console.WriteLine("Host shutting down.");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Something bad happened: {0}", error.Message);
        }

        public void OnNext(HostInfo value)
        {
            value.ScheduledLastCycle.ForEach(c => Console.WriteLine("Scheduled: [{0}]", c));
            value.SucceededLastCycle.ForEach(c => Console.WriteLine("Completed: [{0}]", c));
            value.FailedLastCycle.ForEach(c => Console.WriteLine("Failed: [{0}]", c));
            value.RetriesQueuedLastCycle.ForEach(c => Console.WriteLine("Retrying: [{0}]", c));
            value.DuplicatesSkippedLastCycle.ForEach(c=>Console.WriteLine("Skipping dup: [{0}]", c));
            
            if(value.HostIdle)
            {
                Console.WriteLine("Host sleeping...");
            }
        }
    }
}
