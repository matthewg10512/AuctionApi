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
    public class AuctionCategorySitesController : ControllerBase
    {
        private readonly IAuctionsRepository _auctionRepository;
        private readonly IMapper _mapper;
        public AuctionCategorySitesController(IAuctionsRepository auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository ??
                throw new ArgumentNullException(nameof(auctionRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [Authorize]
        [HttpGet(Name = "GetAuctionCategorySites")]
        public ActionResult<CategorySiteDto> GetAuctionCategorySites()
        {


            var auctionCategorySitesFromRepo = _auctionRepository.GetAuctionCategorySites();


            return Ok(_mapper.Map<List<CategorySiteDto>>(auctionCategorySitesFromRepo));

        }
    }
}
