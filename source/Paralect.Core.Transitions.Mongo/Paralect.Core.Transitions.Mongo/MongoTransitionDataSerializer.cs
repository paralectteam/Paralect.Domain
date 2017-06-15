using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Paralect.Core.Transitions.Mongo
{
    public class MongoTransitionDataSerializer
    {
        public object Deserialize(BsonDocument doc, Type type)
        {
            return BsonSerializer.Deserialize(doc, type);
        }

        public BsonDocument Serialize(object obj)
        {
            var data = new BsonDocument();
            var sw = new StringWriter();
            var jw = new JsonWriter(sw);
            BsonSerializer.Serialize(jw, data);

            return data;
        }
    }
}
