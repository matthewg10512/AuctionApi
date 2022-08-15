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
    public class AuctionSiteCategoryWordsController : ControllerBase
    {

        private readonly IAuctionsRepository _auctionRepository;
        private readonly IMapper _mapper;
        public AuctionSiteCategoryWordsController(IAuctionsRepository auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository ??
                throw new ArgumentNullException(nameof(auctionRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [Authorize]
        [HttpGet(Name = "GetAuctionSiteCategoryWords")]
        public ActionResult<SiteCategoryWordDto> GetAuctionSiteCategoryWords()
        {


            var auctionSiteCategoryWordsFromRepo = _auctionRepository.GetAuctionSiteCategoryWords();


            return Ok(_mapper.Map<List<SiteCategoryWordDto>>(auctionSiteCategoryWordsFromRepo));

        }


        [Authorize]
        [HttpPut(Name = "UpsertAuctionSiteCategoryWords")]
        public ActionResult UpsertAuctionSiteCategoryWords(SiteCategoryWordDto auctionSiteCategoryWord)
        {
            var auctionSiteCategory = _auctionRepository.GetAuctionCategorySite(auctionSiteCategoryWord.CategoryId);
            SiteCategoryWordsResourceParameters auctionSiteCategoryWordParams = new SiteCategoryWordsResourceParameters();
            auctionSiteCategoryWordParams.SiteCategoryWordId = auctionSiteCategoryWord.SearchWordId;
            var auctionSiteCategoryWordsFromRepo = _auctionRepository.SearchAuctionSiteCategoryWords(auctionSiteCategoryWordParams);

            foreach (var auctionSiteCategoryWordFromRepo in auctionSiteCategoryWordsFromRepo)
            {

                var auctionSiteCategoryFromRepo = _auctionRepository.GetAuctionCategorySite(auctionSiteCategoryWord.CategoryId);

                if (auctionSiteCategoryFromRepo.SiteId == auctionSiteCategory.SiteId //site matches as well as the word ID.  There can only be one category associated with a word and site
                    && auctionSiteCategoryWordFromRepo.SearchWordId == auctionSiteCategoryWord.SearchWordId)
                {
                    auctionSiteCategoryWord.Id = auctionSiteCategoryWordFromRepo.Id;
                    break;
                }
            }


            _auctionRepository.UpsertAuctionSiteCategoryWords(auctionSiteCategoryWord);


            return Ok();

        }


        [Authorize]
        [HttpDelete()]
        [HttpDelete("{auctionSiteCategoryWordId}", Name = "DeleteauctionSiteCategoryWord")]
        public ActionResult DeleteAuctionSearchSiteCategoryWordRecord(int auctionSiteCategoryWordId)
        {
            if (!_auctionRepository.AuctionSiteCategoryWordExists(auctionSiteCategoryWordId))
            {
                return NotFound();
            }
            _auctionRepository.DeleteAuctionSiteCategoryWord(auctionSiteCategoryWordId);

            return Ok();

        }



    }
}
