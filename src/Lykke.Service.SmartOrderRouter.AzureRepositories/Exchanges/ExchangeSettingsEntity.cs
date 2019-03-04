using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class ExchangeSettingsEntity : AzureTableEntity
    {
        private ExchangeStatus _status;
        private decimal _marketFee;
        private decimal _transactionFee;

        public ExchangeSettingsEntity()
        {
        }

        public ExchangeSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Name { get; set; }

        public ExchangeStatus Status
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

        [JsonValueSerializer]
        public IReadOnlyList<string> Instruments { get; set; }

        public decimal MarketFee
        {
            get => _marketFee;
            set
            {
                if (_marketFee != value)
                {
                    _marketFee = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal TransactionFee
        {
            get => _transactionFee;
            set
            {
                if (_transactionFee != value)
                {
                    _transactionFee = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
