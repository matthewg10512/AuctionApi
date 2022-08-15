using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionRepository.Models;
using AuctionRepository.ResourceParameters;
using AuctionRepository.Services.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionItemsController : ControllerBase
    {
        private readonly IAuctionsRepository _auctionRepository;
        private readonly IMapper _mapper;
        public AuctionItemsController(IAuctionsRepository auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository ??
                throw new ArgumentNullException(nameof(auctionRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [Authorize]
        [HttpPut(Name = "UpsertAuctionItems")]
        public ActionResult UpsertAuctionItems([FromBody] List<ItemDto> auctionItems)
        {
            _auctionRepository.UpsertAuctionItems(auctionItems);
            return Ok();

        }



        [Authorize]
        [HttpGet(Name = "GetAuctionItems")]
        public ActionResult<List<ItemDto>> GetAuctionItems([FromQuery] ItemsResourceParameters auctionItemsResourceParameters)
        {
            var auctionItemsFromRepo = _auctionRepository.GetAuctionItems(auctionItemsResourceParameters).Take(5000);
            return Ok(_mapper.Map<List<ItemDto>>(auctionItemsFromRepo));
        }

        [Authorize]
        [HttpGet()]
        [Route("~/api/GetAuctionItemStatistics")]
        public ActionResult<List<StatisticDetailDto>> GetAuctionItemStatistics([FromQuery] ItemsResourceParameters auctionItemsResourceParameters)
        {
            var auctionStats = _auctionRepository.GetAuctionStatistics(auctionItemsResourceParameters);
            return Ok(auctionStats);
        }
    }
}
