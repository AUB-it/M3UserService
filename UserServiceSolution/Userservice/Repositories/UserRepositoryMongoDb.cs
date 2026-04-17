using Microsoft.Extensions.Caching.Memory;
using Models;
using MongoDB.Driver;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories;

public class UserRepositoryMongoDb : IUserRepository
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    private MongoClient _client;
    private IMongoDatabase _database;
    private IMongoCollection<User> _collection;
    private ILogger<UserRepositoryMongoDb> _logger;
    
    private IMemoryCache _cache;

    public UserRepositoryMongoDb(ILogger<UserRepositoryMongoDb> logger, IMemoryCache memoryCache)
    {
        _cache = memoryCache;
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        _databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

        _client = new MongoClient(_connectionString);
        _database = _client.GetDatabase(_databaseName);
        _collection = _database.GetCollection<User>("Users");
    }

    public async Task<User?> TryLogin(LoginCredentials credentials)
    {
        _logger.LogDebug($"Trying to login user {credentials.Username}");
        Console.WriteLine($"{credentials.Username}, {credentials.Password}");
        var filter = Builders<User>.Filter.Eq("Username", credentials.Username);
        var filter2 = Builders<User>.Filter.Eq("Password", credentials.Password);
        var finalFilter = Builders<User>.Filter.And(filter, filter2);
        
        var result = await _collection.Find(finalFilter).FirstOrDefaultAsync();
        return result;
    }

    // CREATE
    public async Task<User> CreateUser(UserDTO user)
    {
        // Password burde hashes men whatever
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            Password = user.Password,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Address1 = user.Address1,
            Address2 = user.Address2,
            PostalCode = user.PostalCode,
            FaxNumber = user.FaxNumber,
            City = user.City,
            Email = user.Email,
            Telephone = user.Telephone
        };
        _logger.LogDebug($"Creating user with username {user.Username} and userId {newUser.Id}");
        await _collection.InsertOneAsync(newUser);

        return newUser;
    }

    // READ ONE
    public async Task<User?> GetUserById(Guid id)
    {
        var user = GetUserFromCache(id);
        if (user == null)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            _logger.LogDebug($"Getting user with id {id} from database");
            var userFromDb = await _collection.Find(filter).FirstOrDefaultAsync();
            SetUserInCache(userFromDb);
            return userFromDb;
        }
        _logger.LogDebug($"Getting user with id {id} from cache");
        return user;
    }

    // READ ALL
    public async Task<List<User>> GetAllUsers()
    {
        _logger.LogDebug($"Getting all users");
        return await _collection.Find(Builders<User>.Filter.Empty).ToListAsync();
    }

    // UPDATE
    public async Task<User?> UpdateUser(Guid id, UserDTO user)
    {
        _logger.LogDebug($"Updating user with id {id}");
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var existingUser = await _collection.Find(filter).FirstOrDefaultAsync();

        if (existingUser == null)
        {
            _logger.LogError($"Could not find user with id {id} to update");
            return null;
        }

        var updatedUser = new User
        {
            Id = existingUser.Id,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Address1 = user.Address1,
            Address2 = user.Address2,
            PostalCode = user.PostalCode,
            FaxNumber = user.FaxNumber,
            City = user.City,
            Email = user.Email,
            Telephone = user.Telephone
        };

        await _collection.ReplaceOneAsync(filter, updatedUser);

        return updatedUser;
    }

    // DELETE
    public async Task<bool> DeleteUser(Guid id)
    {
        _logger.LogDebug($"Deleting user with id {id}");
        RemoveFromCache(id);
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
    
    // Cache funktioner
    
    private void SetUserInCache(User user)
    {
        _logger.LogDebug($"Setting user with id {user.Id} in cache");
        var cacheExpiryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Priority = CacheItemPriority.High
        };
        _cache.Set(user.Id, user, cacheExpiryOptions);
    }
    
    private User? GetUserFromCache(Guid userId)
    {
        _logger.LogDebug($"Getting user with id {userId} from cache");
        User user = null;
        _cache.TryGetValue(userId, out user);
        return user;
    }
    
    private void RemoveFromCache(Guid userId)
    {
        _logger.LogDebug($"Removing user with id {userId} from cache");
        _cache.Remove(userId);
    }
}