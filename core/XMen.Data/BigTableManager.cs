using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using Grpc.Core;
using Microsoft.Extensions.Options;
using XMen.Data.Models;

namespace XMen.Data
{
    public class BigTableManager : IBigTableManager
    {
        private readonly BigTableOptions _options;

        private readonly BigtableTableAdminClient _bigtableTableAdminClient;

        private readonly BigtableClient _bigtableClient;

        public BigTableManager(IOptions<BigTableOptions> options)
        {
            _options = options.Value;

            // BigtableTableAdminClient API lets us create, manage and delete tables.
            _bigtableTableAdminClient = BigtableTableAdminClient.Create();

            // BigtableClient API lets us read and write to a table.
            _bigtableClient = BigtableClient.Create();
        }

        public void Initialize()
        {
            if (!TableExist())
            {
                _bigtableTableAdminClient.CreateTable(
                    new InstanceName(_options.ProjectId, _options.InstanceId),
                    _options.TableId,
                    new Table
                    {
                        Granularity = Table.Types.TimestampGranularity.Millis,
                        ColumnFamilies =
                        {
                            {
                                _options.ColumnFamily, 
                                new ColumnFamily
                                {
                                    GcRule = new GcRule
                                    {
                                        MaxNumVersions = 1
                                    }
                                }
                            }
                        }
                    });
            }
        }

        private bool TableExist()
        {
            var request = new GetTableRequest
            {
                TableName = new TableName(_options.ProjectId, _options.InstanceId, _options.TableId),
                View = Table.Types.View.NameOnly
            };
            try
            {
                var tables = _bigtableTableAdminClient.GetTable(request);
                return true;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        /// <param name="detection"></param>
        /// <returns></returns>
        public async Task<bool> InsertAndGet(string[] dna, Func<string[], bool> detection)
        {
            var key = new BigtableByteString(GetUniqueId(dna));
            var tableName = new TableName(_options.ProjectId, _options.InstanceId, _options.TableId);
            var filter = RowFilters.CellsPerRowLimit(1);

            var rowRead = await _bigtableClient.ReadRowAsync(tableName, key, filter);
            if (rowRead != null)
            {
                return rowRead.Families[0].Columns[0].Cells[0].Value.ToStringUtf8().Equals("1");
            }

            var isMutant = detection(dna);
            var mutation = Mutations.SetCell(_options.ColumnFamily, "isMutant", new BigtableByteString(isMutant ? "1" : "0"), new BigtableVersion(DateTime.UtcNow));
            var response = _bigtableClient.MutateRowAsync(tableName, key, mutation);

            return isMutant;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Stats> Stats()
        {
            var tableName = new TableName(_options.ProjectId, _options.InstanceId, _options.TableId);

            var rows = _bigtableClient.ReadRows(tableName);
            var e = rows.GetAsyncEnumerator(CancellationToken.None);
            var stat = new Stats(); 
            while (await e.MoveNextAsync())
            {
                stat.Mutants += e.Current.Families[0].Columns[0].Cells[0].Value.ToStringUtf8() == "1" ? 1 : 0;
                stat.Humans += e.Current.Families[0].Columns[0].Cells[0].Value.ToStringUtf8() == "0" ? 1 : 0;
            }

            return stat;
        }

        /// <summary>
        /// Generate unique id for dna matrix
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        public string GetUniqueId(string[] dna)
        {
            return string.Join("", dna);
        }
    }
}