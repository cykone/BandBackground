using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Ft.Examples.Band.Timetracker.Background.Model;

namespace Ft.Examples.Band.Timetracker.Background
{
    public static class JsonHelper
    {
        public static string ModelToJson(TimeTrackerModel model)
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

        public static TimeTrackerModel ModelFromJson(string json)
        {
            using (var mem = new MemoryStream(Encoding.ASCII.GetBytes(json)))
            {
                var ser = new DataContractJsonSerializer(typeof(TimeTrackerModel));
                return (TimeTrackerModel)ser.ReadObject(mem);
            }
        }

        public static string ItemsToJson(IList<TimeTrackerModel> model)
        {
            using (var mem = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(IEnumerable<TimeTrackerModel>));
                ser.WriteObject(mem, model);

                mem.Position = 0;
                using (var reader = new StreamReader(mem))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static IList<TimeTrackerModel> ItemsFromJson(string json)
        {
            using (var mem = new MemoryStream(Encoding.ASCII.GetBytes(json)))
            {
                var ser = new DataContractJsonSerializer(typeof(IList<TimeTrackerModel>));
                return (IList<TimeTrackerModel>)ser.ReadObject(mem);
            }
        }
    }
}