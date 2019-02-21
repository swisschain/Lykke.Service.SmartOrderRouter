using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SmartOrderRouter.Client
{
    /// <summary>
    /// SmartOrderRouter client settings.
    /// </summary>
    [PublicAPI]
    public class SmartOrderRouterServiceClientSettings
    {
        /// <summary>
        /// Service url.
        /// </summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
