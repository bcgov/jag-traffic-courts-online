using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Arc.Dispute.Service.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrafficCourts.Arc.Dispute.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TcoDisputeTicketController : ControllerBase
    {
        private readonly IMapper _mapper;

        // Assign the object in the constructor for dependency injection
        public TcoDisputeTicketController(IMapper mapper)
        {
            _mapper = mapper;
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
                var adnotatedTicketData = _mapper.Map<AdnotatedTicket>(disputeData);
                var disputedTicketData = _mapper.Map<DisputedTicket>(disputeData);
                return Ok(adnotatedTicketData);
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
