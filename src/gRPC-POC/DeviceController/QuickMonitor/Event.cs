namespace example
{
    class EventTest
    {
        private const int MAX_NUM_OF_LOG = 16;
        private const int MAX_NUM_OF_IMAGE_LOG = 2;
        private const string LOG_IMAGE_FILE = "./image_log.jpg";

        private EventSvc eventSvc;

        public EventTest(EventSvc svc)
        {
            eventSvc = svc;
        }

        public void Test(uint[] deviceIDs)
        {
            eventSvc.StartMonitoringMulti(deviceIDs);

            Console.WriteLine(">>> Generate real-time events for 20 seconds");
            Thread.Sleep(20000);

            eventSvc.StopMonitoringMulti(deviceIDs);
        }
    }
}

