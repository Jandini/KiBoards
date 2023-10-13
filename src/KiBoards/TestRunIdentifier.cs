namespace KiBoards
{
    public class TestRunIdentifier
    {
        public Guid Id {  get; set; }
        public DateTime DateTime { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        


        public TestRunIdentifier()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;        
        }

    }
}
