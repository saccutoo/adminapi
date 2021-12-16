using System;
using System.Data;

namespace Utils
{
    public interface IUnitOfWorkOracle : IDisposable
    {
        IDbConnection GetConnection();
        bool CheckConnection();
    }
}