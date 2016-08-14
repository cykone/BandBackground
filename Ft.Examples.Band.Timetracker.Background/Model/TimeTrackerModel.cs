using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Ft.Examples.Band.Timetracker.Background.Model
{
    public sealed class TimeTrackerModel
    {
        public TimeTrackerModel()
        {

        }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public DateTimeOffset LastBreakStarted { get; set; }

        public double TotalBreakHours { get; set; }

        public bool IsPaused { get; set; }

        public bool IsFinished { get; set; }

        public bool IsStarted { get; set; }

        public void Start()
        {
            this.StartTime = DateTimeOffset.UtcNow;
            this.IsStarted = true;
        }

        public void Pause()
        {
            this.LastBreakStarted = DateTimeOffset.UtcNow;
            this.IsPaused = true;
        }

        public void Resume()
        {
            var duration = Math.Round((DateTimeOffset.UtcNow - this.LastBreakStarted).TotalHours, 2);
            this.TotalBreakHours += duration;

            this.IsPaused = false;
        }

        public void FinishTracking()
        {
            this.IsFinished = true;
            this.IsStarted = false;

            this.EndTime = DateTimeOffset.Now;
        }

        public double CalculateWorkedHours()
        {
            if (this.IsFinished)
            {
                return Math.Round((this.EndTime - this.StartTime).TotalHours, 2) - this.TotalBreakHours;
            }

            return Math.Round((DateTimeOffset.UtcNow - this.StartTime).TotalHours, 2) - this.TotalBreakHours;
        }

        public static string ToJson(TimeTrackerModel model)
        {
            using (var mem = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(TimeTrackerModel));
                ser.WriteObject(mem, model);

                mem.Position = 0;
                using (var reader = new StreamReader(mem))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static TimeTrackerModel FromJson(string json)
        {
            using (var mem = new MemoryStream(Encoding.ASCII.GetBytes(json)))
            {
                var ser = new DataContractJsonSerializer(typeof(TimeTrackerModel));
                return (TimeTrackerModel)ser.ReadObject(mem);
            }
        }
    }
}