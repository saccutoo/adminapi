using Microsoft.EntityFrameworkCore;

namespace Utils
{
    public interface IDatabaseFactory
    {
        DbContext GetDbContext();
        string GetPrefix();
    }
}