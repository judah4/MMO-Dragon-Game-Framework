using Lidgren.Network;
using Mmogf.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Core.Networking
{
    public enum DataStat
    {
        Command,
        Event,
        Update,
        Entity,
    }


    public class DataStatistics : IInternalBehavior
    {

        public DataSpan CurrentTimeSlice => _timeSlices.Count > 1 ? _timeSlices[1] : _timeSlices[0];

        List<DataSpan> _timeSlices = new List<DataSpan>(1000);

        float _tick = 0;
        int _maxTimeSlices = 1000; //in seconds
        MmoWorker _worker;

        public DataStatistics(MmoWorker worker)
        {
            _worker = worker;
            SetupSlice();
        }

        public void RecordMessage(int id, int bytes, DataStat stat)
        {
            var timeSlice = _timeSlices[0];
            switch (stat)
            {
                case DataStat.Command:
                    timeSlice.RecordMessage(id, bytes, timeSlice.Commands);
                    break;
                case DataStat.Event:
                    timeSlice.RecordMessage(id, bytes, timeSlice.Events);
                    break;
                case DataStat.Update:
                    timeSlice.RecordMessage(id, bytes, timeSlice.Updates);
                    break;
                case DataStat.Entity:
                    timeSlice.RecordMessage(id, bytes, timeSlice.Entities);
                    break;
            }
        }

        void SetupSlice()
        {
            _timeSlices.Insert(0,new DataSpan()
            {
                Commands = new Dictionary<int, DataBucket>(),
                Events = new Dictionary<int, DataBucket>(),
                Updates = new Dictionary<int, DataBucket>(),
                Entities = new Dictionary<int, DataBucket>(),
            });
        }

        public void Update()
        {
            _tick += UnityEngine.Time.deltaTime;

            if(_tick < 1f)
                return;
            
            //next timeslice
            SetupSlice();

            while(_timeSlices.Count > _maxTimeSlices)
            {
                _timeSlices.RemoveAt(_timeSlices.Count - 1);
            }
            _tick -= 1;
            
        }

        public override string ToString()
        {
            DataSpan timeSlice;
            if(_timeSlices.Count > 1)
                timeSlice = _timeSlices[1];
            else
                timeSlice = _timeSlices[0];
            var summary = timeSlice.ToString();
            return summary;
        }


    }
}
