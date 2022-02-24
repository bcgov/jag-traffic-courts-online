using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrafficCourts.Arc.Dispute.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TcoDisputeTicketController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IArcFileService _arcFileService;

        // Assign the object in the constructor for dependency injection
        public TcoDisputeTicketController(IMapper mapper, IArcFileService arcFileService)
        {
            _mapper = mapper;
            _arcFileService = arcFileService;
        }

        // GET: api/<TcoDisputeTicketController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TcoDisputeTicketController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TcoDisputeTicketController>
        [HttpPost]
        public async Task<IActionResult> CreateArcFile([FromBody] TcoDisputeTicket disputeData)
        {
            if (ModelState.IsValid)
            {
                var arcFileRecords = _mapper.Map<List<ArcFileRecord>>(disputeData);

                await _arcFileService.createArcFile(arcFileRecords);

                return Ok(arcFileRecords);
            }
            
            return BadRequest(ModelState);
        }

        // PUT api/<TcoDisputeTicketController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TcoDisputeTicketController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
