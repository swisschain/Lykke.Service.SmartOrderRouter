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
    public class ExternalLimitOrderEntity : AzureTableEntity
    {
        private decimal _price;
        private decimal _volume;
        private OrderType _type;
        private ExternalLimitOrderStatus _status;
        private DateTime _createdDate;
        private decimal? _executedVolume;
        private decimal? _executedPrice;

        public ExternalLimitOrderEntity()
        {
        }

        public ExternalLimitOrderEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id { get; set; }

        public string Exchange { get; set; }

        public string AssetPair { get; set; }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
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

        public decimal? ExecutedVolume
        {
            get => _executedVolume;
            set
            {
                if (_executedVolume != value)
                {
                    _executedVolume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal? ExecutedPrice
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

        public ExternalLimitOrderStatus Status
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
