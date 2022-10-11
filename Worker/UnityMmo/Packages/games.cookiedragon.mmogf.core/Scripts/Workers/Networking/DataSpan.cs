using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;
using static UnityEngine.EventSystems.EventTrigger;

namespace Mmogf.Core.Networking
{
    public struct DataSpan
    {
        public Dictionary<int, DataBucket> Commands { get; set; }
        public Dictionary<int, DataBucket> Events { get; set; }

        public Dictionary<int, DataBucket> Updates { get; set; }

        public Dictionary<int, DataBucket> Entities { get; set; }

        public void RecordMessage(int id, int bytes, Dictionary<int, DataBucket> buckets)
        {
            DataBucket bucket;
            if(!buckets.TryGetValue(id, out bucket))
                bucket = new DataBucket();

            bucket.Messages++;
            bucket.Bytes += bytes;

            buckets[id] = bucket;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Commands");
            foreach (var command in Commands)
            {
                stringBuilder.AppendLine($"{command.Key}: Messages {command.Value.Messages}, {(command.Value.Bytes / 1000f).ToString("F3")} KB");
            }
            stringBuilder.AppendLine("Events");
            foreach (var evn in Events)
            {
                stringBuilder.AppendLine($"{evn.Key}: Messages {evn.Value.Messages}, {(evn.Value.Bytes / 1000f).ToString("F3")} KB");
            }
            stringBuilder.AppendLine("Updates");
            foreach (var update in Updates)
            {
                stringBuilder.AppendLine($"{update.Key}: Messages {update.Value.Messages}, {(update.Value.Bytes / 1000f).ToString("F3")} KB");
            }
            stringBuilder.AppendLine("Entities");
            var entitySum = Sum(Entities);
            stringBuilder.AppendLine($"Messages {entitySum.Messages}, {(entitySum.Bytes / 1000f).ToString("F3")} KB");

            return stringBuilder.ToString();
        }


        DataBucket Sum(Dictionary<int, DataBucket> buckets)
        {
            var sum = new DataBucket();
            foreach (var pair in buckets)
            {
                sum.Messages += pair.Value.Messages;
                sum.Bytes += pair.Value.Bytes;
            }

            return sum;
        }
    }
}
