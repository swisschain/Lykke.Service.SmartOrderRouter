using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Orders
{
    public class MarketOrderRepository : IMarketOrderRepository
    {
        private readonly INoSQLTableStorage<MarketOrderEntity> _storage;

        public MarketOrderRepository(INoSQLTableStorage<MarketOrderEntity> storage)
        {
            _storage = storage;
        }

        public async Task<MarketOrder> GetByIdAsync(string marketOrderId)
        {
            string filter = TableQuery.GenerateFilterCondition(nameof(ITableEntity.RowKey), QueryComparisons.Equal,
                GetRowKey(marketOrderId));

            var query = new TableQuery<MarketOrderEntity>().Where(filter);

            IEnumerable<MarketOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<MarketOrder>(entities.SingleOrDefault());
        }

        public async Task<IReadOnlyList<MarketOrder>> GetByStatusAsync(MarketOrderStatus marketOrderStatus)
        {
            string filter = TableQuery.GenerateFilterCondition(nameof(MarketOrderEntity.Status), QueryComparisons.Equal,
                marketOrderStatus.ToString());

            var query = new TableQuery<MarketOrderEntity>().Where(filter);

            IEnumerable<MarketOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<MarketOrder>>(entities);
        }

        public async Task<IReadOnlyList<MarketOrder>> GetByClientIdAsync(string clientId)
        {
            string filter = TableQuery.GenerateFilterCondition(nameof(MarketOrderEntity.ClientId),
                QueryComparisons.Equal, clientId);

            var query = new TableQuery<MarketOrderEntity>().Where(filter);

            IEnumerable<MarketOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<MarketOrder>>(entities);
        }

        public async Task<IReadOnlyList<MarketOrder>> GetAllAsync(DateTime startDate, DateTime endDate, int? limit)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            var query = new TableQuery<MarketOrderEntity>().Where(filter).Take(limit);

            IEnumerable<MarketOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<MarketOrder>>(entities);
        }

        public async Task InsertAsync(MarketOrder marketOrder)
        {
            var entity = new MarketOrderEntity(GetPartitionKey(marketOrder.CreatedDate), GetRowKey(marketOrder.Id));

            Mapper.Map(marketOrder, entity);

            await _storage.InsertAsync(entity);
        }

        public Task UpdateAsync(MarketOrder marketOrder)
        {
            return _storage.MergeAsync(GetPartitionKey(marketOrder.CreatedDate), GetRowKey(marketOrder.Id), entity =>
            {
                Mapper.Map(marketOrder, entity);
                return entity;
            });
        }

        private static string GetPartitionKey(DateTime timestamp)
            => (DateTime.MaxValue.Ticks - timestamp.Date.Ticks).ToString("D19");

        private static string GetRowKey(string marketOrderId)
            => marketOrderId;
    }
}
