using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Paralect.Core.Transitions.Mongo
{
    public class MongoTransitionRepository : ITransitionRepository
    {
        private const string ConcurrencyException = "E1100";
        private readonly IDataTypeRegistry _dataTypeRegistry;
        private readonly MongoTransitionServer _server;
        private readonly MongoTransitionSerializer _serializer;

        public MongoTransitionRepository(IDataTypeRegistry dataTypeRegistry, string connectionString, string collectionName = "transitions")
        {
            _dataTypeRegistry = dataTypeRegistry;
            _serializer = new MongoTransitionSerializer(dataTypeRegistry);
            _server = new MongoTransitionServer(connectionString);

            EnsureIndexes();
        }

        private Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>> RequiredIndexes()
        {
            return new Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>>()
            {
                {new BsonDocument("_id.StreamId", 1), Builders<BsonDocument>.IndexKeys.Ascending("_id.StreamId")},
                {new BsonDocument("_id.Version", 1), Builders<BsonDocument>.IndexKeys.Ascending("_id.Version")},
                {new BsonDocument("Timestamp", 1), Builders<BsonDocument>.IndexKeys.Ascending("Timestamp")},
                {
                    new BsonDocument
                    {
                        new BsonElement("Timestamp", 1),
                        new BsonElement("_id.Version", 1),
                         
                    },
                    Builders<BsonDocument>.IndexKeys.Ascending("Timestamp").Ascending("_id.Version")
                }
            };
        }

        public void EnsureIndexes()
        {
            var indexes = _server.Transitions.Indexes
                .ListAsync()
                .GetAwaiter()
                .GetResult()
                .Current
                .Select(x => x["key"] as BsonDocument).ToList();

            foreach (var index in RequiredIndexes())
            {
                if (!indexes.Contains(index.Key))
                    _server.Transitions.Indexes.CreateOneAsync(index.Value).GetAwaiter().GetResult();
            }
        }

        public void SaveTransition(Transition transition)
        {
            // skip saving empty transition
            if (transition.Events.Count < 1)
                return;

            var doc = _serializer.Serialize(transition);

            try
            {
                _server.Transitions.InsertOneAsync(doc);
            }
            catch (MongoException e)
            {
                if (!e.Message.Contains(ConcurrencyException))
                    throw;

                throw new DuplicateTransitionException(transition.Id.StreamId, transition.Id.Version, e);
            }
        }

        public List<Transition> GetTransitions(string streamId, int fromVersion, int toVersion)
        {

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id.StreamId", streamId) &
                         builder.Gte("_id.Version", fromVersion) &
                         builder.Lte("_id.Version", toVersion);

            var docs = _server.Transitions.Find(filter)
                .Sort(Builders<BsonDocument>.Sort.Ascending("_id.Version"))
                .ToListAsync()
                .GetAwaiter()
                .GetResult();

            // Check that such stream exists
            if (docs.Count < 1)
                throw new ArgumentException($"There is no stream in store with id {streamId}");

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp of transiton
        /// Should be used only for testing and for very simple event replying 
        /// </summary>
        public List<Transition> GetTransitions()
        {
            var docs = _server.Transitions.Find(new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Ascending("Timestamp"))
                .Sort(Builders<BsonDocument>.Sort.Ascending("_id.Version"))
                .ToListAsync()
                .GetAwaiter()
                .GetResult();

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        public void RemoveTransition(string streamId, int version)
        {
            var id = _serializer.SerializeTransitionId(new TransitionId(streamId, version));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

            _server.Transitions.DeleteOneAsync(filter).GetAwaiter().GetResult();
        }

        public void RemoveStream(string streamId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id.StreamId", streamId);
            _server.Transitions.DeleteOneAsync(filter).GetAwaiter().GetResult();
        }
    }
}
