namespace example
{
    public class EventCodeEntry
    {
        public uint event_code { get; set; }
        public string event_code_str { get; set; }
        public uint sub_code { get; set; }
        public string sub_code_str { get; set; }
        public string desc { get; set; }
    }

    public class EventCodeMap
    {
        public string title { get; set; }
        public string version { get; set; }
        public string date { get; set; }
        public IList<EventCodeEntry> entries { get; set; }
    }
}
