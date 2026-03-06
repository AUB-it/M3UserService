using Models;
using MongoDB.Driver;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    private MongoClient _client;
    private IMongoDatabase _database;
    private IMongoCollection<User> _collection;

    public UserRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        _databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

        _client = new MongoClient(_connectionString);
        _database = _client.GetDatabase(_databaseName);
        _collection = _database.GetCollection<User>("Users");
    }

    // CREATE
    public async Task<User> CreateUser(UserDTO user)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            givenName = user.givenName,
            familyName = user.familyName,
            Address1 = user.Address1,
            Address2 = user.Address2,
            PostalCode = user.PostalCode,
            faxNumber = user.faxNumber,
            City = user.City,
            email = user.email,
            telephone = user.telephone
        };

        await _collection.InsertOneAsync(newUser);

        return newUser;
    }

    // READ ONE
    public async Task<User?> GetUserById(Guid id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    // READ ALL
    public async Task<List<User>> GetAllUsers()
    {
        return await _collection.Find(Builders<User>.Filter.Empty).ToListAsync();
    }

    // UPDATE
    public async Task<User?> UpdateUser(Guid id, UserDTO user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var existingUser = await _collection.Find(filter).FirstOrDefaultAsync();

        if (existingUser == null)
            return null;

        var updatedUser = new User
        {
            Id = existingUser.Id,
            givenName = user.givenName,
            familyName = user.familyName,
            Address1 = user.Address1,
            Address2 = user.Address2,
            PostalCode = user.PostalCode,
            faxNumber = user.faxNumber,
            City = user.City,
            email = user.email,
            telephone = user.telephone
        };

        await _collection.ReplaceOneAsync(filter, updatedUser);

        return updatedUser;
    }

    // DELETE
    public async Task<bool> DeleteUser(Guid id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
}