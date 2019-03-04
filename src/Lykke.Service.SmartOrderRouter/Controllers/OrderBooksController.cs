using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class OrderBooksController : Controller, IOrderBooksApi
    {
        private readonly IAggregatedOrderBookService _aggregatedOrderBookService;
        private readonly IInternalOrderBookService _internalOrderBookService;
        private readonly IExternalOrderBookService _externalOrderBookService;

        public OrderBooksController(
            IAggregatedOrderBookService aggregatedOrderBookService,
            IInternalOrderBookService internalOrderBookService,
            IExternalOrderBookService externalOrderBookService)
        {
            _aggregatedOrderBookService = aggregatedOrderBookService;
            _internalOrderBookService = internalOrderBookService;
            _externalOrderBookService = externalOrderBookService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of aggregated order books.</response>
        [HttpGet("aggregated")]
        [ProducesResponseType(typeof(IReadOnlyList<AggregatedOrderBookModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<AggregatedOrderBookModel>> GetAggregatedAsync()
        {
            IReadOnlyList<AggregatedOrderBook> orderBooks = _aggregatedOrderBookService.GetAll();

            return Task.FromResult(Mapper.Map<IReadOnlyList<AggregatedOrderBookModel>>(orderBooks));
        }

        /// <inheritdoc/>
        /// <response code="200">The aggregated order book.</response>
        [HttpGet("aggregated/{assetPair}")]
        [ProducesResponseType(typeof(AggregatedOrderBookModel), (int) HttpStatusCode.OK)]
        public Task<AggregatedOrderBookModel> GetAggregatedAsync(string assetPair)
        {
            AggregatedOrderBook orderBook = _aggregatedOrderBookService.GetByAssetPair(assetPair);

            if (orderBook == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order book does not exist");

            return Task.FromResult(Mapper.Map<AggregatedOrderBookModel>(orderBook));
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of internal order books.</response>
        [HttpGet("internal")]
        [ProducesResponseType(typeof(IReadOnlyList<InternalOrderBookModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<InternalOrderBookModel>> GetInternalAsync()
        {
            IReadOnlyList<InternalOrderBook> orderBooks = _internalOrderBookService.GetAll();

            return Task.FromResult(Mapper.Map<IReadOnlyList<InternalOrderBookModel>>(orderBooks));
        }

        /// <inheritdoc/>
        /// <response code="200">The internal order book.</response>
        [HttpGet("internal/{assetPair}")]
        [ProducesResponseType(typeof(InternalOrderBookModel), (int) HttpStatusCode.OK)]
        public Task<InternalOrderBookModel> GetInternalAsync(string assetPair)
        {
            InternalOrderBook orderBook = _internalOrderBookService.GetByAssetPair(assetPair);

            if (orderBook == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order book does not exist");

            return Task.FromResult(Mapper.Map<InternalOrderBookModel>(orderBook));
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of external order books.</response>
        [HttpGet("external")]
        [ProducesResponseType(typeof(IReadOnlyList<ExternalOrderBookModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<ExternalOrderBookModel>> GetExternalAsync(string exchange = null,
            string assetPair = null)
        {
            IReadOnlyList<ExternalOrderBook> orderBooks = _externalOrderBookService.Filter(exchange, assetPair);

            return Task.FromResult(Mapper.Map<IReadOnlyList<ExternalOrderBookModel>>(orderBooks));
        }
    }
}
