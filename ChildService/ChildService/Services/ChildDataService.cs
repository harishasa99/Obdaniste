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
           
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase("ChildDatabase");

            
            _childrenCollection = database.GetCollection<Child>("Children");
        }

        
        public async Task<List<Child>> GetAllAsync()
        {
            
            return await _childrenCollection.Find(_ => true).ToListAsync();
        }

        
        public async Task<Child> GetAsync(int id)
        {
            return await _childrenCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        
        public async Task CreateAsync(Child newChild)
        {
            await _childrenCollection.InsertOneAsync(newChild);
        }

       
        public async Task UpdateAsync(int id, Child updatedChild)
        {
            await _childrenCollection.ReplaceOneAsync(
                x => x.Id == id,
                updatedChild
            );
        }

        
        public async Task RemoveAsync(int id)
        {
            await _childrenCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
