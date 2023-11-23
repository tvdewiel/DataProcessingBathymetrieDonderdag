using DataSetManager;
using PredictionStats;

namespace SpatialInterpolationModel
{
    public class InverseDistanceInterpolation
    {
        public InverseDistanceInterpolation(int power, int numberOfPoints, DataSet dataSet)
        {
            Power = power;
            NumberOfPoints = numberOfPoints;
            this.dataSet = dataSet;
        }
        public int Power { get; set; }
        public int NumberOfPoints { get; set; }
        public DataSet dataSet { get; set; }
        public double Z(double x, double y)//,int power,List<XYZ> points)
        {
            try
            {
                double sumW = 0.0;
                double z = 0;
                foreach (XYZ point in FindNearestNeighbours(x, y, NumberOfPoints))
                {
                    double d = Math.Sqrt(Math.Pow(x - point.X, 2) + Math.Pow(y - point.Y, 2));
                    if (d == 0) return point.Z;
                    double w = 1.0 / Math.Pow(d, Power);
                    sumW += w;
                    z += w * point.Z;
                }
                return z / sumW;
            }
            catch (Exception ex)
            {
                SpatialInterpolationModelException simex = new("Z", ex);
                simex.Data.Add("Power", Power);
                simex.Data.Add("NumberOfPoints", NumberOfPoints);
                throw simex;
            }
        }
        private List<XYZ> ListFromSortedList(SortedList<double, List<XYZ>> nn)
        {
            List<XYZ> list = new List<XYZ>();
            foreach (List<XYZ> l in nn.Values)
            {
                foreach (XYZ v in l) list.Add(v);
            }
            return list;
        }
        public List<XYZ> FindNearestNeighbours(double x, double y, int n)
        {
            try
            {
                SortedList<double, List<XYZ>> nn = new SortedList<double, List<XYZ>>();
                double dsquare;
                double dmin;
                dsquare = Math.Pow(dataSet.data[0].X - x, 2) + Math.Pow(dataSet.data[0].Y - y, 2);
                dmin = dsquare;
                nn.Add(dsquare, new List<XYZ>() { dataSet.data[0] });
                for (int i = 1; i < n; i++)
                {
                    dsquare = Math.Pow(dataSet.data[i].X - x, 2) + Math.Pow(dataSet.data[i].Y - y, 2);
                    if (nn.ContainsKey(dsquare)) nn[dsquare].Add(dataSet.data[i]);
                    else nn.Add(dsquare, new List<XYZ>() { dataSet.data[i] });
                    if (dsquare > dmin) dmin = dsquare;
                }
                for (int i = n; i < dataSet.data.Count; i++)
                {
                    dsquare = Math.Pow(dataSet.data[i].X - x, 2) + Math.Pow(dataSet.data[i].Y - y, 2);
                    if (dsquare < dmin)
                    {
                        if (nn.ContainsKey(dsquare)) nn[dsquare].Add(dataSet.data[i]);
                        else nn.Add(dsquare, new List<XYZ>() { dataSet.data[i] });
                        if (nn.Count > n) nn.RemoveAt(n);
                        dmin = nn.Keys[n - 1];
                    }
                }
                return (List<XYZ>)ListFromSortedList(nn).Take(n).ToList();
            }
            catch (Exception ex)
            {
                throw new SpatialInterpolationModelException("FindNearestNeighbours", ex);
            }
        }
        public List<XYZoZp> Predict(List<XYZ> toPredict)
        {
            List<XYZoZp> pred = new List<XYZoZp>();
            try
            {
                foreach (XYZ p in toPredict)
                {
                    pred.Add(new XYZoZp(p.X, p.Y, p.Z, Z(p.X, p.Y)));
                }
                return pred;
            }
            catch (Exception ex) { throw new SpatialInterpolationModelException("Predict", ex); }
        }
    }
}