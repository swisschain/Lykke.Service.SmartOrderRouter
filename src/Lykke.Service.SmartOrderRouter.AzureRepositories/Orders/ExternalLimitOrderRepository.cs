using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Orders
{
    public class ExternalLimitOrderRepository : IExternalLimitOrderRepository
    {
        private readonly INoSQLTableStorage<ExternalLimitOrderEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indices;

        public ExternalLimitOrderRepository(
            INoSQLTableStorage<ExternalLimitOrderEntity> storage,
            INoSQLTableStorage<AzureIndex> indices)
        {
            _storage = storage;
            _indices = indices;
        }

        public async Task<IReadOnlyList<ExternalLimitOrder>> GetAllAsync()
        {
            IList<ExternalLimitOrderEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<List<ExternalLimitOrder>>(entities);
        }

        public async Task<ExternalLimitOrder> GetByIdAsync(string externalLimitOrderId)
        {
            ExternalLimitOrderEntity entity = await _storage.GetDataAsync(GetPartitionKey(externalLimitOrderId),
                GetRowKey(externalLimitOrderId));

            return Mapper.Map<ExternalLimitOrder>(entity);
        }

        public async Task<IReadOnlyList<ExternalLimitOrder>> GetByParentIdAsync(string parentOrderId)
        {
            IEnumerable<AzureIndex> indices = await _indices.GetDataAsync(GetIndexPartitionKey(parentOrderId));

            IEnumerable<ExternalLimitOrderEntity> entities = await _storage.GetDataAsync(indices);

            return Mapper.Map<List<ExternalLimitOrder>>(entities);
        }

        public async Task InsertAsync(string parentId, ExternalLimitOrder externalLimitOrder)
        {
            var entity = new ExternalLimitOrderEntity(GetPartitionKey(externalLimitOrder.Id),
                GetRowKey(externalLimitOrder.Id));

            Mapper.Map(externalLimitOrder, entity);

            await _storage.InsertAsync(entity);

            var index = new AzureIndex(GetIndexPartitionKey(parentId), GetIndexRowKey(externalLimitOrder.Id), entity);

            await _indices.InsertAsync(index);
        }

        public Task UpdateAsync(ExternalLimitOrder externalLimitOrder)
        {
            return _storage.MergeAsync(GetPartitionKey(externalLimitOrder.Id), GetRowKey(externalLimitOrder.Id),
                entity =>
                {
                    Mapper.Map(externalLimitOrder, entity);
                    return entity;
                });
        }

        private static string GetPartitionKey(string externalLimitOrderId)
            => externalLimitOrderId.CalculateHexHash32(3);

        private static string GetRowKey(string externalLimitOrderId)
            => externalLimitOrderId;

        private static string GetIndexPartitionKey(string parentId)
            => parentId;

        private static string GetIndexRowKey(string externalLimitOrderId)
            => externalLimitOrderId;
    }
}
