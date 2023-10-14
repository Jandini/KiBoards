namespace KiBoards
{
    class TestRunSummary
    {
        /// <summary>
        /// The total number of tests run.
        /// </summary>
        public int Total { get; internal set; }

        /// <summary>
        /// The number of failed tests.
        /// </summary>
        public int Failed { get; internal set; }

        /// <summary>
        /// The number of skipped tests.
        /// </summary>
        public int Skipped { get; internal set; }

        /// <summary>
        /// Compute number of passed tests
        /// </summary>
        public int Passed { get => Total - Failed - Skipped; }

        /// <summary>
        /// The total time taken to run the tests, in seconds.
        /// </summary>
        public decimal Time { get; internal set; }
    }
}
