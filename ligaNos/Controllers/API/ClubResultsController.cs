using ligaNos.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ligaNos.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClubResultsController : Controller
    {
        private readonly IClubResultsRepository _clubResultsRepository;

        public ClubResultsController(IClubResultsRepository clubResultsRepository)
        {
            _clubResultsRepository = clubResultsRepository;
        }


        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_clubResultsRepository.GetClubResults());
        }
    }
}
