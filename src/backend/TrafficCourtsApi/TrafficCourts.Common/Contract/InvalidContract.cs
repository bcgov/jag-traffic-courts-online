namespace TrafficCourts.Common.Contract
{
    [QueueName("InvalidContract_queue")]
    public class InvalidContract<T>
    {
        /// <summary>
        /// Invalid Contract
        /// </summary>
        public T Contract { get; set; }

        /// <summary>
        /// The reason the contract is invalid.
        /// </summary>
        public string Reason { get; set; }

    }
}
