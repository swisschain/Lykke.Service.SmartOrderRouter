using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Orders
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class MarketOrderEntity : AzureTableEntity
    {
        private OrderType _type;
        private decimal _volume;
        private decimal _executedPrice;
        private MarketOrderStatus _status;
        private DateTime _createdDate;

        public MarketOrderEntity()
        {
        }

        public MarketOrderEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id { get; set; }

        public string ClientId { get; set; }

        public string AssetPair { get; set; }

        public OrderType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal ExecutedPrice
        {
            get => _executedPrice;
            set
            {
                if (_executedPrice != value)
                {
                    _executedPrice = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public MarketOrderStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
