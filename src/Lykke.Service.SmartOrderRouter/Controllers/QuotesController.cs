using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.Quotes;
using Lykke.Service.SmartOrderRouter.Domain.Entities;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class QuotesController : Controller, IQuotesApi
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of quotes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<QuoteModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<QuoteModel>> GetAsync(string exchange = null, string assetPair = null)
        {
            IEnumerable<Quote> quotes = _quoteService.GetAll();

            if (!string.IsNullOrEmpty(exchange))
                quotes = quotes.Where(o => o.Exchange == exchange);
            
            if (!string.IsNullOrEmpty(assetPair))
                quotes = quotes.Where(o => o.AssetPair == assetPair);

            return Task.FromResult<IReadOnlyList<QuoteModel>>(Mapper.Map<List<QuoteModel>>(quotes));
        }
    }
}
