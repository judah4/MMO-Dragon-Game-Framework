using System.Collections.Generic;
using System.Text;

namespace Mmogf.Core.Networking
{
    public struct DataSpan
    {
        private Dictionary<DataStat, Dictionary<int, DataBucket>> _dataBuckets;

        public Dictionary<DataStat, Dictionary<int, DataBucket>> DataBuckets => _dataBuckets;

        public static DataSpan Create 
        { 
            get
            {
                var span = new DataSpan();
                span._dataBuckets = new Dictionary<DataStat, Dictionary<int, DataBucket>>();
                return span;
            }
        }

        public void RecordMessage(DataStat dataStat, int id, int bytes)
        {
            var buckets = GetBuckets(dataStat);
            DataBucket bucket;
            if (!buckets.TryGetValue(id, out bucket))
                bucket = new DataBucket();

            bucket.Messages++;
            bucket.Bytes += bytes;

            buckets[id] = bucket;
        }

        public void RecordMessage(int id, int bytes, Dictionary<int, DataBucket> buckets)
        {
            DataBucket bucket;
            if(!buckets.TryGetValue(id, out bucket))
                bucket = new DataBucket();

            bucket.Messages++;
            bucket.Bytes += bytes;

            buckets[id] = bucket;
        }

        public Dictionary<int, DataBucket> GetBuckets(DataStat dataStat)
        {
            if (!DataBuckets.TryGetValue(dataStat, out var buckets))
            {
                buckets = new Dictionary<int, DataBucket>();
                DataBuckets.Add(dataStat, buckets);
            }
            return buckets;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(var spans in DataBuckets)
            {
                foreach(var data in spans.Value)
                {
                    stringBuilder.AppendLine($"{data.Key}: Messages {data.Value.Messages}, {(data.Value.Bytes / 1000f).ToString("F3")} KB");
                }
            }

            return stringBuilder.ToString();
        }

        public DataBucket Sum(Dictionary<int, DataBucket> buckets)
        {
            var sum = new DataBucket();
            foreach (var pair in buckets)
            {
                sum.Messages += pair.Value.Messages;
                sum.Bytes += pair.Value.Bytes;
            }

            return sum;
        }

        public DataBucket Sum(IList<DataBucket> buckets)
        {
            var sum = new DataBucket();
            foreach (var pair in buckets)
            {
                sum.Messages += pair.Messages;
                sum.Bytes += pair.Bytes;
            }

            return sum;
        }

    }
}
