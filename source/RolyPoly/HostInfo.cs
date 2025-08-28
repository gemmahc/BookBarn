namespace RolyPoly
{
    public class HostInfo
    {
        public HostInfo()
        {
            ScheduledLastCycle = new List<Uri>();
            CurrentlyRunning = new List<Uri>();
            SucceededLastCycle = new List<Uri>();
            FailedLastCycle = new List<Uri>();
            RetriesQueuedLastCycle = new List<Uri>();
            ChildrenProcessedLastCycle = new List<Uri>();
            DuplicatesSkippedLastCycle = new List<Uri>();
        }

        public long Cycle { get; set; }

        public int CompletedLastCycle { get; set; }

        public List<Uri> ScheduledLastCycle { get; set; }

        public List<Uri> CurrentlyRunning { get; set; }

        public List<Uri> SucceededLastCycle { get; set; }

        public List<Uri> FailedLastCycle { get; set; }

        public List<Uri> RetriesQueuedLastCycle { get; set; }

        public List<Uri> ChildrenProcessedLastCycle { get; set; }

        public List<Uri> DuplicatesSkippedLastCycle { get; set; }

        public bool HostIdle { get; set; }

        public void StartCycle()
        {
            Cycle++;
            CompletedLastCycle = 0;
            HostIdle = false;
            ScheduledLastCycle.Clear();
            SucceededLastCycle.Clear();
            FailedLastCycle.Clear();
            RetriesQueuedLastCycle.Clear();
            ChildrenProcessedLastCycle.Clear();
            DuplicatesSkippedLastCycle.Clear();
        }

        public HostInfo Clone()
        {
            return new HostInfo()
            {
                Cycle = this.Cycle,
                HostIdle = this.HostIdle,
                CompletedLastCycle = this.CompletedLastCycle,
                ScheduledLastCycle = new List<Uri>(this.ScheduledLastCycle),
                CurrentlyRunning = new List<Uri>(this.CurrentlyRunning),
                FailedLastCycle = new List<Uri>(this.FailedLastCycle),
                SucceededLastCycle = new List<Uri>(this.SucceededLastCycle),
                RetriesQueuedLastCycle = new List<Uri>(this.RetriesQueuedLastCycle),
                ChildrenProcessedLastCycle = new List<Uri>(this.ChildrenProcessedLastCycle),
                DuplicatesSkippedLastCycle = new List<Uri>(this.DuplicatesSkippedLastCycle)
            };
        }
    }
}
