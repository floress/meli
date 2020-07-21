using System;
using System.Threading.Tasks;
using XMen.Data.Models;

namespace XMen.Data
{
    public interface IMutantStoreManager
    {
        Task<bool> InsertAndGet(string[] dna, Func<string[], bool> detection);

        Task<Stats> Stats();
    }
}