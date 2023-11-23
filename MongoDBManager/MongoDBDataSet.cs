using DataSetManager;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBManager
{
    public class MongoDBDataSet
    {
        public MongoDBDataSet(DataSetMetaInfo metaInfo, DataSet dataSet)
        {
            this.metaInfo = metaInfo;
            this.dataSet = dataSet;
        }

        public MongoDBDataSet(ObjectId id, DataSetMetaInfo metaInfo, DataSet dataSet)
        {
            this.id = id;
            this.metaInfo = metaInfo;
            this.dataSet = dataSet;
        }

        public ObjectId id {  get; set; }
        public DataSetMetaInfo metaInfo { get; set; }
        public DataSet dataSet { get; set; }
    }
}
