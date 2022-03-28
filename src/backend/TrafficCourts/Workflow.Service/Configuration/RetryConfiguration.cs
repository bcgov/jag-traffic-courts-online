namespace TrafficCourts.Workflow.Service.Configuration
{
    public class RetryConfiguration
    {
        public RetryConfiguration()
        {
            this.RetryTimes = 5;
            this.RetryInterval = 2;
            this.ConcurrencyLimit = 2;
        }


        /// <summary>
        /// Retry Times
        /// </summary>
        public int RetryTimes { get; set; }

        /// <summary>
        /// Re Try Interval 
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// Concurrency limit for consumer
        /// </summary>
        public int ConcurrencyLimit { get; set; }
    }
}
