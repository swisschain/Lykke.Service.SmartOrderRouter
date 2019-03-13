using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class TimerSettingsEntity : AzureTableEntity
    {
        private TimeSpan _marketOrders;
        private TimeSpan _balances;

        public TimerSettingsEntity()
        {
        }

        public TimerSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public TimeSpan MarketOrders
        {
            get => _marketOrders;
            set
            {
                if (_marketOrders != value)
                {
                    _marketOrders = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public TimeSpan Balances
        {
            get => _balances;
            set
            {
                if (_balances != value)
                {
                    _balances = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
