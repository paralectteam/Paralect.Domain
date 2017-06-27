using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Paralect.Core.Transitions.Mongo
{
    public class MongoTransitionServer
    {
        private readonly MongoClient _client;

        private readonly string _databaseName;

        /// <summary>
        /// Collection for storing commits data
        /// </summary>
        private const string TransaitonsCollectionName = "transitions";
        private const string SnapshotsCollectionName = "snapshots";

        private readonly MongoCollectionSettings _transitionSettings;
        private readonly MongoCollectionSettings _snapshotSettings;

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoTransitionServer(string connectionString)
        {
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _client = new MongoClient(connectionString);

            _transitionSettings = new MongoCollectionSettings { AssignIdOnInsert = false };

            _snapshotSettings = new MongoCollectionSettings { AssignIdOnInsert = false };
        }

        /// <summary>
        /// Get database
        /// </summary>
        public IMongoDatabase Database => _client.GetDatabase(_databaseName);

        /// <summary>
        /// Get commits collection
        /// </summary>
        public IMongoCollection<BsonDocument> Transitions => Database.GetCollection<BsonDocument>(TransaitonsCollectionName, _transitionSettings);

        public IMongoCollection<BsonDocument> Snapshots => Database.GetCollection<BsonDocument>(SnapshotsCollectionName, _snapshotSettings);
    }
}