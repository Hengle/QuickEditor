namespace QuickEditor.Builder
{
    using System;

    [Serializable]
    public class BuildTimer
    {
        public long StartTime;
        public long EndTime;

        public TimeSpan Duration
        {
            get { return BuildHelper.CalDuration(DateTime.Now.Ticks, StartTime, EndTime); }
        }
    }
}