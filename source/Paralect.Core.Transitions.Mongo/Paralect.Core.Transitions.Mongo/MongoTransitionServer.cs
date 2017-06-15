using MongoDB.Bson;
using MongoDB.Driver;

namespace Paralect.Core.Transitions.Mongo
{
    public class MongoTransitionServer
    {
        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// Collection for storing commits data
        /// </summary>
        private readonly string _collectionName;

        private readonly MongoCollectionSettings _transitionSettings;

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoTransitionServer(string connectionString, string collectionName)
        {
            _collectionName = collectionName;
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            Client = new MongoClient(connectionString);

            _transitionSettings = new MongoCollectionSettings()
            {
                AssignIdOnInsert = false
            };
        }

        /// <summary>
        /// MongoDB Client
        /// </summary>
        public MongoClient Client { get; }

        /// <summary>
        /// Get database
        /// </summary>
        public IMongoDatabase Database => Client.GetDatabase(_databaseName);

        /// <summary>
        /// Get commits collection
        /// </summary>
        public IMongoCollection<BsonDocument> Transitions => Database.GetCollection<BsonDocument>(_collectionName, _transitionSettings);
    }
}