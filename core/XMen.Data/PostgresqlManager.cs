using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using XMen.Data.Models;

namespace XMen.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class PostgresqlManager : IPostgresqlManager
    {
        private readonly PostgresqlOptions _options;

        public PostgresqlManager(IOptions<PostgresqlOptions> options)
        {
            _options = options.Value;
        }

        private NpgsqlConnectionStringBuilder GetConnectionCloudBuilder()
        {
            var builder = new NpgsqlConnectionStringBuilder(_options.ConnectionString)
            {
                SslMode = SslMode.Disable
            };
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Test()
        {
            await using var cnn = new NpgsqlConnection(GetConnectionCloudBuilder().ConnectionString);
            await cnn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT 1", cnn);
            var response = await cmd.ExecuteScalarAsync();
            return response != null;
        }

        /// <summary>
        /// Calculate mutant rate
        /// </summary>
        /// <returns></returns>
        public async Task<Stats> Stats()
        {
            await using var cnn = new NpgsqlConnection(GetConnectionCloudBuilder().ConnectionString);
            await cnn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(isMutant), 0) mutants, -1 * COALESCE(SUM(isMutant - 1), 0) humans FROM mutants", cnn);
            await using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (dr.Read())
                    return new Stats
                        {
                            Mutants = (long)dr[0],
                            Humans = (long)dr[1]
                        };
            }
            return new Stats();
        }

        /// <summary>
        /// Insert mutant with array concatenated by semicolon
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="id"></param>
        /// <param name="dna"></param>
        /// <param name="isMutant"></param>
        /// <returns></returns>
        private async Task Insert(NpgsqlConnection cnn, long id, string[] dna, int isMutant)
        {
            await using var cmd = new NpgsqlCommand("INSERT INTO mutants VALUES (@id, @dna, @isMutant)", cnn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Bigint));
            cmd.Parameters.Add(new NpgsqlParameter("@dna", NpgsqlDbType.Text));
            cmd.Parameters.Add(new NpgsqlParameter("@isMutant", NpgsqlDbType.Integer));

            cmd.Parameters[0].Value = id;
            cmd.Parameters[1].Value = string.Join(';', dna);
            cmd.Parameters[2].Value = isMutant;
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        /// <param name="detection"></param>
        /// <returns></returns>
        public async Task<bool> InsertAndGet(string[] dna, Func<string[], bool> detection)
        {
            var id = GetUniqueId(dna);

            await using var cnn = new NpgsqlConnection(GetConnectionCloudBuilder().ConnectionString);
            await cnn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT isMutant FROM mutants WHERE id = @id", cnn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Bigint));
            cmd.Parameters[0].Value = id;

            var response = await cmd.ExecuteScalarAsync();
            if (response != null)
            {
                return (int) response == 1;
            }

            var isMutant = detection(dna);
            await Insert(cnn, id, dna, isMutant ? 1 : 0);

            return isMutant;
        }

        /// <summary>
        /// Deterministic hashcode generator (possible hash collision)
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        public long GetUniqueId(string[] dna)
        {
            var str = string.Join(';', dna);
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
    }
}
