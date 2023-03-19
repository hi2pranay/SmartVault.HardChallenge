using System.Data.SQLite;
using System.Threading.Tasks;

namespace SmartVault.DataGeneration
{
    public interface IPerformanceImprovement
    {
        Task ImprovePerformanceAsync(SQLiteConnection connection);
    }
}
