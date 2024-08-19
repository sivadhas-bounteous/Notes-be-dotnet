namespace Notes.DTO
{
    public class LogEntry
    {
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public int LogLevel { get; set; }
        public DateTime Timestamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; } // Serialized JSON string for properties
    }

}
