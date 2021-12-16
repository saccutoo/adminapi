using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Utils;

namespace API.Infrastructure.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {
        }
        public DataContext(string prefix, string databaseType, string connectionString)
        {
            Prefix = prefix;
            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }
        public string Prefix { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                #region Config database

                switch (DatabaseType)
                {
                    case "MSSQL":
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                    case "Oracle":
                        optionsBuilder.UseOracle(ConnectionString);
                        break;
                    default:
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                }

                #endregion

                optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
            }
        }        
    }
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var databaseType = Helpers.GetConfig("ConnectionString:DbType");
            var connectionString = Helpers.GetConfig("ConnectionString:" + databaseType);
            if (context is DataContext dynamicContext)
            {
                return (context.GetType(), dynamicContext.Prefix, databaseType, connectionString);
            }

            return context.GetType();
        }
    }
}
