using System.Threading.Tasks;

namespace XMen.Data
{
    public interface IPostgresqlManager : IMutantStoreManager
    { 
        Task<bool> Test();
    }
}