using MongoDB.Driver;
using Toy1Backend.Models;

namespace Toy1Backend.Db
{
    public class DbCollection
    {
        public IMongoCollection<User> users;

        public DbCollection(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("mongodb");
            var client = new MongoClient(connStr);
            var db = client.GetDatabase("toy1");
            users = db.GetCollection<User>("users");
        }
    }
}
