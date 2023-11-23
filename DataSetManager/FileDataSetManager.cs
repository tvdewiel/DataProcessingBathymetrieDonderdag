using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataSetManager
{
    public class FileDataSetManager
    {
        public static DataSet ReadData(string fileName)
        {
            try
            {
                DataSet ds = new DataSet();
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string? line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        string[] parts = line.Split(',');
                        double x = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                        double y = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                        double z = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                        ds.AddXYZ(new XYZ(x, y, z));
                    }
                }
                return ds;
            }
            catch (Exception ex) { throw new DataSetManagerException("ReadDataSet", ex); }
        }       
        public static List<DataSet> MakeDataSets(List<XYZ> data, List<int> size)
        {
            try
            {
                List<DataSet> sets = new List<DataSet>();
                Random random = new Random();
                for (int i = 0; i < size.Count; i++)
                {
                    List<XYZ> set = new List<XYZ>();
                    int n = 0;
                    int index;
                    if (size[i] > data.Count) throw new DataSetManagerException("MakeDataSets - size to high");
                    while (n < size[i])
                    {
                        index = random.Next(data.Count);
                        if (!set.Contains(data[index]))
                        {
                            n++;
                            set.Add(data[index]);
                        }
                    }
                    sets.Add(new DataSet(set));
                }
                return sets;
            }
            catch (Exception ex) { throw new DataSetManagerException("MakeDataSets", ex); }
        }
        public static List<DataSet> MakeDataSetsWithTestSet(List<XYZ> data, List<int> size) //first is test set
        {
            try
            {
                List<DataSet> sets = new List<DataSet>();
                Random random = new Random();
                //test set
                //List<XYZ> testset=new List<XYZ>();
                Dictionary<int,XYZ> testset=new Dictionary<int,XYZ>();
                int n = 0;
                int index;
                if (size[0] > data.Count) throw new DataSetManagerException($"MakeDataSetsWithTestSet - size to high - 0,{size[0]}");
                while (n < size[0])
                {
                    index = random.Next(data.Count);
                    //if (!testset.Contains(data[index]))
                    if (!testset.ContainsKey(index))
                    {
                        n++;
                        //testset.Add(data[index]);
                        testset.Add(index, data[index]);
                    }
                }
                sets.Add(new DataSet(testset.Values.ToList()));
                //data sets
                for (int i = 1; i < size.Count; i++)
                {
                    //List<XYZ> set = new List<XYZ>();
                    Dictionary<int,XYZ> set= new Dictionary<int,XYZ>();
                    n = 0;
                    if (size[i] > data.Count) throw new DataSetManagerException($"MakeDataSetsWithTestSet - size to high - {i},{size[i]}");
                    while (n < size[i])
                    {
                        index = random.Next(data.Count);
                        //if (!set.Contains(data[index]) && !testset.Contains(data[index]))
                        if (!set.ContainsKey(index) && !testset.ContainsKey(index))
                        {
                            n++;
                            //set.Add(data[index]);
                            set.Add(index, data[index]);
                        }
                    }
                    sets.Add(new DataSet(set.Values.ToList()));
                }
                return sets;
            }
            catch (Exception ex) { throw new DataSetManagerException("MakeDataSetsWithTestSet", ex); }
        }      
        public static void WriteDataSet(DataSet data, string fileName)
        {
            Console.WriteLine($"start write {fileName}");
            try
            {
                string jsonString = JsonSerializer.Serialize(data);
                File.WriteAllText(fileName, jsonString);
            }
            catch (Exception ex) { throw new DataSetManagerException("WriteDataSets", ex); }
        }
        public static void WriteDataSets(List<DataSet> data, List<string> fileNames)
        {
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    string jsonString = JsonSerializer.Serialize(data[i]);
                    File.WriteAllText(fileNames[i], jsonString);
                }
            }
            catch (Exception ex) { throw new DataSetManagerException("WriteDataSets", ex); }
        }        
        public static DataSet ReadDataSet(string fileName)
        {
            try
            {
                string jsonString = File.ReadAllText(fileName);
                DataSet? dataSet = JsonSerializer.Deserialize<DataSet>(jsonString);
                if (dataSet == null) throw new DataSetManagerException("ReadDataSet");
                return dataSet;
            }
            catch (Exception ex)
            {
                throw new DataSetManagerException("ReadDataSet", ex);
            }
        }       
    }
}
