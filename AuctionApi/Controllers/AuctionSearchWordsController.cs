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
    public class AuctionSearchWordsController : ControllerBase
    {

        private readonly IAuctionsRepository _auctionRepository;
        private readonly IMapper _mapper;
        public AuctionSearchWordsController(IAuctionsRepository auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository ??
                throw new ArgumentNullException(nameof(auctionRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [Authorize]
        [HttpGet(Name = "GetAuctionSearchWords")]
        public ActionResult<SearchWordDto> GetAuctionSearchWords()
        {


            var auctionSearchWordsFromRepo = _auctionRepository.GetAuctionSearchWords();


            return Ok(_mapper.Map<List<SearchWordDto>>(auctionSearchWordsFromRepo));

        }


        [Authorize]
        [HttpPut(Name = "UpsertAuctionSearchWord")]
        public ActionResult UpsertAuctionSearchWord([FromBody] SearchWordDto auctionSearchWord)
        {
            var searchWordId = _auctionRepository.UpsertAuctionSearchWords(auctionSearchWord);
            if (searchWordId == 0)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
        [Authorize]
        [HttpDelete()]
        [HttpDelete("{searchWordId}", Name = "DeleteAuctionSearchWordRecord")]
        public ActionResult DeleteAuctionSearchWordRecord(int searchWordId)
        {
            if (!_auctionRepository.AuctionSearchWordExists(searchWordId))
            {
                return NotFound();
            }

            _auctionRepository.DeleteAuctionItems(searchWordId);
            _auctionRepository.DeleteStockScreenerAlertsHistory(searchWordId);
            _auctionRepository.DeleteAuctionSiteCategoryWords(searchWordId);

            _auctionRepository.DeleteAuctionSearchWord(searchWordId);


            return Ok();

        }


    }
}
