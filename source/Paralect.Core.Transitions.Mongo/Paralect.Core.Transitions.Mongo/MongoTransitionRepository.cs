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
        private readonly MongoTransitionServer _transitionServer;
        private readonly MongoTransitionSerializer _serializer;

        public MongoTransitionRepository(IDataTypeRegistry dataTypeRegistry, string connectionString)
        {
            _dataTypeRegistry = dataTypeRegistry;
            _serializer = new MongoTransitionSerializer(dataTypeRegistry);
            _transitionServer = new MongoTransitionServer(connectionString);

            EnsureIndexes();
        }

        private Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>> RequiredIndexes()
        {
            return new Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>>
            {
                {new BsonDocument("_id.StreamId", 1), Builders<BsonDocument>.IndexKeys.Ascending("_id.StreamId")},
                {new BsonDocument("_id.Version", 1), Builders<BsonDocument>.IndexKeys.Ascending("_id.Version")},
                {new BsonDocument("Timestamp", 1), Builders<BsonDocument>.IndexKeys.Ascending("Timestamp")},
                {
                    new BsonDocument
                    {
                        new BsonElement("Timestamp", 1),
                        new BsonElement("_id.Version", 1)
                    },
                    Builders<BsonDocument>.IndexKeys.Ascending("Timestamp").Ascending("_id.Version")
                }
            };
        }

        public void EnsureIndexes()
        {
            var indexes = _transitionServer.Transitions.Indexes
                .List()
                .ToList()
                .Select(x => x["key"] as BsonDocument).ToList();

            foreach (var index in RequiredIndexes())
                if (!indexes.Contains(index.Key))
                    _transitionServer.Transitions.Indexes.CreateOneAsync(index.Value).GetAwaiter().GetResult();

            _transitionServer.Snapshots.Indexes.CreateOne(Builders<BsonDocument>.IndexKeys.Ascending("_id.StreamId")
                .Descending("_id.Version"));
            _transitionServer.Snapshots.Indexes.CreateOne(Builders<BsonDocument>.IndexKeys.Ascending("_id.StreamId"));
        }

        public void SaveTransition(Transition transition)
        {
            // skip saving empty transition
            if (transition.Events.Count < 1)
                return;

            var doc = _serializer.Serialize(transition);

            try
            {
                _transitionServer.Transitions.InsertOneAsync(doc);
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

            var docs = _transitionServer.Transitions.Find(filter)
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

        public List<Transition> GetTransitions(int startIndex, int count)
        {
            var docs = _transitionServer.Transitions.Find(new BsonDocument())
                .Skip(startIndex)
                .Limit(count)
                .Sort(Builders<BsonDocument>.Sort.Ascending("Timestamp").Ascending("_id.Version"))
                .ToList();

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        public long CountTransitions()
        {
            return _transitionServer.Transitions.Count(new BsonDocument());
        }

        /// <summary>
        ///     Get all transitions ordered ascendantly by Timestamp of transiton
        ///     Should be used only for testing and for very simple event replying
        /// </summary>
        public List<Transition> GetTransitions()
        {
            var docs = _transitionServer.Transitions.Find(new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Ascending("Timestamp").Ascending("_id.Version"))
                .ToList();

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        public void RemoveTransition(string streamId, int version)
        {
            var id = _serializer.SerializeTransitionId(new TransitionId(streamId, version));
            var query = new BsonDocument {{"_id", id}};

            _transitionServer.Transitions.DeleteOne(query);
        }

        public void RemoveStream(string streamId)
        {
            var query = new BsonDocument {{"_id.StreamId", streamId}};
            _transitionServer.Transitions.DeleteOne(query);
        }
    }
}
