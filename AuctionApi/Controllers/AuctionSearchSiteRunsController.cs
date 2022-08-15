using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionRepository.Models;
using AuctionRepository.Services.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionSearchSiteRunsController : ControllerBase
    {

        private readonly IAuctionsRepository _auctionRepository;
        private readonly IMapper _mapper;
        public AuctionSearchSiteRunsController(IAuctionsRepository auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository ??
                throw new ArgumentNullException(nameof(auctionRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [Authorize]
        [HttpPut(Name = "UpsertAuctionSearchSiteRun")]
        public ActionResult UpsertAuctionSearchSiteRuns([FromBody] SearchSiteRunDto auctionItems)
        {
            _auctionRepository.UpsertAuctionSearchSiteRun(auctionItems);
            return Ok();

        }



        [Authorize]
        [HttpGet(Name = "GetAuctionSearchSiteRuns")]
        public ActionResult<List<SearchSiteRunDto>> GetAuctionSearchSiteRuns()
        {
            var auctionItemsFromRepo = _auctionRepository.GetAuctionSearchSiteRun();
            return Ok(_mapper.Map<List<SearchSiteRunDto>>(auctionItemsFromRepo));
        }
    }
}
