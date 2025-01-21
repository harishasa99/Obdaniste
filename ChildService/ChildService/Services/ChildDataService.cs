using ChildService.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ChildService.Services
{
    public class ChildDataService
    {
        private readonly IMongoCollection<Child> _childrenCollection;

        public ChildDataService(IConfiguration configuration)
        {
            // U appsettings.json treba da imate ConnectionStrings:MongoDb
            // npr. "mongodb://localhost:27017" ili Atlas string
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);

            // Naziv baze po želji (ovde "ChildDatabase")
            var database = client.GetDatabase("ChildDatabase");

            // Kolekcija u kojoj čuvate Child dokumente, npr. "Children"
            _childrenCollection = database.GetCollection<Child>("Children");
        }

        // GET ALL
        public async Task<List<Child>> GetAllAsync()
        {
            // Find(_ => true) -> svi dokumenti
            return await _childrenCollection.Find(_ => true).ToListAsync();
        }

        // GET by Id
        public async Task<Child> GetAsync(int id)
        {
            return await _childrenCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        // CREATE
        public async Task CreateAsync(Child newChild)
        {
            await _childrenCollection.InsertOneAsync(newChild);
        }

        // UPDATE
        public async Task UpdateAsync(int id, Child updatedChild)
        {
            await _childrenCollection.ReplaceOneAsync(
                x => x.Id == id,
                updatedChild
            );
        }

        // DELETE
        public async Task RemoveAsync(int id)
        {
            await _childrenCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
