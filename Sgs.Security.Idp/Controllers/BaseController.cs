using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sgs.Security.Idp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public BaseController(
            IMapper mapper
            ,ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }
    }
}
