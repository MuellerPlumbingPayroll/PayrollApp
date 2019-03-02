using System;

namespace Timecard
{
    public class App
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "https://api-dot-muller-plumbing-salary.appspot.com";

        public static void Initialize()
        {
            if (UseMockDataStore)
                ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
            else
                ServiceLocator.Instance.Register<IDataStore<Item>, CloudDataStore>();
        }
    }
}
