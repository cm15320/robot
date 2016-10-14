using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace robotTracking
{
    [Serializable]
    public class BodePlot : ICollection
    {
        [XmlArrayAttribute("bodePlot")]
        private List<BodeDataPoint> bodePlot= new List<BodeDataPoint>();

        public BodeDataPoint this[int index]
        {
            get { return (BodeDataPoint)bodePlot[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            Array tableDataArray = bodePlot.ToArray();
            tableDataArray.CopyTo(a, index);
        }

        public int Count
        {
            get { return bodePlot.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            return bodePlot.GetEnumerator();
        }

        public void Add(BodeDataPoint newBodeDataPoint)
        {
            bodePlot.Add(newBodeDataPoint);
        }

        public void RemoveAt(int index)
        {
            bodePlot.RemoveAt(index);
        }


    }
}
