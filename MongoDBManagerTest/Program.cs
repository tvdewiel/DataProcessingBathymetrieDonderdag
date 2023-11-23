using DataSetManager;
using MongoDBManager;

namespace MongoDBManagerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string source=@"c:\data\belgica\belgica.txt";
            string campaignCode = "Juli 2022";
            string dataSeries = "ModelTest ID A";

            string connectionString = "mongodb://localhost:27017";
            MongoDBDataSetRepository repository = new MongoDBDataSetRepository(connectionString);

            var data = FileDataSetManager.ReadData(@"C:\data\belgica\belgica.txt");
            var sets = FileDataSetManager.MakeDataSetsWithTestSet(data.data, new List<int> { 5000, 5000, 2500, 15000, 1000 });

            DataSetMetaInfo metaInfo = new DataSetMetaInfo(sets[0].data.Count,source,campaignCode,dataSeries,DataSetType.TestSet);

            List<MongoDBDataSet> mongoDBDataSets = new List<MongoDBDataSet>();
            MongoDBDataSet mdsSet = new MongoDBDataSet(metaInfo,sets[0]);
            mongoDBDataSets.Add(mdsSet);

            for(int i=1;i<sets.Count;i++)
            {
                metaInfo = new DataSetMetaInfo(sets[i].data.Count, source, campaignCode, dataSeries, DataSetType.DataSet);
                mdsSet=new MongoDBDataSet(metaInfo, sets[i]);
                mongoDBDataSets.Add(mdsSet);
            }

            repository.WriteDataSets(mongoDBDataSets);
            Console.WriteLine("end");
        }
    }
}