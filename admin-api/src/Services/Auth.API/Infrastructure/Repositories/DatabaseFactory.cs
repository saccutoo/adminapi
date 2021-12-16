using Admin.API.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using Utils;

namespace API.Infrastructure.Repositories
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly DbContext _dataContext;
        public string Prefix;

        public DatabaseFactory(string prefix = "", string connectionstring = "")
        {
            Prefix = prefix;
            var databaseType = Helpers.GetConfig("ConnectionString:DbType");
            if (string.IsNullOrEmpty(connectionstring)) connectionstring = Helpers.GetConfig("ConnectionString:" + databaseType);
            _dataContext = new ModelContext(prefix, databaseType, connectionstring);

            // Get randomize Id
            var random = new Random(DateTime.Now.Millisecond);
            Id = random.Next(1000000).ToString();
        }

        public string Id { get; set; }

        public DbContext GetDbContext()
        {
            return _dataContext;
        }

        public string GetPrefix()
        {
            return Prefix;
        }
    }
}