using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using Utils;

namespace API.Infrastructure.Repositories
{
    public class UnitOfWorkOracle : IUnitOfWorkOracle
    {
        private OracleConnection _oracleConnection;
        private bool _disposed;
        private readonly string _connectionstring;
        public UnitOfWorkOracle()
        {
            //var databaseType = Helpers.GetConfig("ConnectionString:DbType");
            //if (string.IsNullOrEmpty(_connectionstring)) _connectionstring = Helpers.GetConfig("ConnectionString:" + databaseType);
            string connectionstring = "DATA SOURCE=" + Config.ConnectionDefautl.HOST + ":" + Config.ConnectionDefautl.PORT + "/" + Config.ConnectionDefautl.SERVICENAME + ";USER ID=" + Config.ConnectionDefautl.USERNAME + ";PASSWORD=" + EncryptedString.DecryptString(Config.ConnectionDefautl.PASSWORD, Config.ConnectionDefautl.KEY) + ";";
            if (string.IsNullOrEmpty(_connectionstring)) _connectionstring = connectionstring;
        }

        public IDbConnection GetConnection()
        {
            _oracleConnection = new OracleConnection(_connectionstring);
            if (_oracleConnection.State == ConnectionState.Closed) _oracleConnection.Open();
            return _oracleConnection;
        }
        public bool CheckConnection()
        {
            return _oracleConnection.State == ConnectionState.Open || _oracleConnection.State == ConnectionState.Connecting;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _oracleConnection.Close();
                    _oracleConnection.Dispose();
                    _disposed = true;
                }

            _disposed = false;
        }     
    }
}