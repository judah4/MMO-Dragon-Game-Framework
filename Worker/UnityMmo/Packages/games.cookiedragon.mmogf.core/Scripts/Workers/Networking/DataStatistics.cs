using Mmogf.Core.Behaviors;
using System.Collections.Generic;

namespace Mmogf.Core.Networking
{
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
            timeSlice.RecordMessage(stat, id, bytes);
        }

        void SetupSlice()
        {
            _timeSlices.Insert(0,DataSpan.Create);
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
