using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentModels.BeMobile
{
    public class TraveltimeStatic
    {
        public List<TraveltimeSegmentStatic> SegmentList { get; }
        private TraveltimeSegmentStatic _merged;
        public TraveltimeSegmentStatic Merged
        {
            get {
                if (_merged == null && SegmentList?.Count > 0)
                {
                    CalculateCoordinateChain();
                }
                return _merged;
            }
            set { _merged = value; }
        }


        public TraveltimeStatic(List<TraveltimeSegmentStatic> segmentList)
        {
            SegmentList = segmentList;
        }

        public TraveltimeStatic()
        {
        }

        public void CalculateCoordinateChain()
        {

            //Create queue to process all segments
            Queue<TraveltimeSegmentStatic> toProcess = new Queue<TraveltimeSegmentStatic>();
            for (var i = 1; i < SegmentList.Count; i++)
            {
                //for some reason duplicate segments exist, duplicates have empty coordinates. Ignore them.
                if (SegmentList[i].beginnodelatitude > 0)
                {
                    toProcess.Enqueue(SegmentList[i]);
                }
            }
            //set first element
            TraveltimeSegmentStatic biggest = SegmentList[0].Clone() as TraveltimeSegmentStatic;
            biggest.SegmentID = 0;//SegmentID will keep track of how much segments are added

            //counter to check if we are in a infinite loop. If there are more elements checked between merge operations than there are in the toProcess queue, stop the loop.
            int processedAfterAdd = 0;

            double tolerance = 0.000000001;

            Console.WriteLine("-------------------Started Building new chain---------------------");
            while (toProcess.Count > 0)
            {
                if (processedAfterAdd > toProcess.Count)
                {
                    tolerance *= 10; //increase tolerance if no matches found
                }

                TraveltimeSegmentStatic current = toProcess.Dequeue();
                if (Math.Abs(biggest.endnodelatitude - current.beginnodelatitude) < tolerance && Math.Abs(biggest.endnodelatitude - current.beginnodelatitude) < tolerance) 
                {
                    //new segment comes behind current one
                    biggest.lengthmm += current.lengthmm;
                    biggest.endnodelatitude = current.endnodelatitude;
                    biggest.endnodelongitude = current.endnodelongitude;
                    biggest.SegmentID++;
                    processedAfterAdd = 0;
                //    Console.WriteLine($"Matching segment found, new length: {biggest.lengthmm}");
                }
                else if (Math.Abs(current.endnodelatitude - biggest.beginnodelatitude) < tolerance &&
                         Math.Abs(current.endnodelatitude - biggest.beginnodelatitude) < tolerance)
                {
                    //new segment comes before current one
                    biggest.lengthmm += current.lengthmm;
                    biggest.beginnodelatitude = current.beginnodelatitude;
                    biggest.beginnodelongitude = current.beginnodelongitude;
                    biggest.SegmentID++;
                    processedAfterAdd = 0;
                   // Console.WriteLine($"Matching segment found, new length: {biggest.lengthmm}");

                }
                else
                {
                    processedAfterAdd++;
                    toProcess.Enqueue(current);
                   // Console.WriteLine($"Segment doesn't match. Not matched in a row: {processedAfterAdd}, Queue size: {toProcess.Count}");
                }
            }
            //The queue must be empty for the operation to be succesfull

            Console.WriteLine($"All segments matched with max tolerance of {tolerance}");
            _merged = biggest;
        }

    }
}
