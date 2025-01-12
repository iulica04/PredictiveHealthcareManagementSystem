using Application.Commands.PatientRecordByIdCommands;
using Application.Commands.PatientRecordCommands;
using Application.DTOs;
using Application.Queries.PatientRecordQueries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientRecordController : ControllerBase
    {
        private readonly IMediator mediator;
        public PatientRecordController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreatePatientRecord(CreatePatientRecordCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                return CreatedAtAction(nameof(GetByID), new { Id = result.Data }, result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            try
            {
                var result = await mediator.Send(new GetPatientRecordByIdQuery { PatientRecordId = id });
                if (result.IsSuccess)
                {
                    return Ok(result.Data);
                }
                return NotFound(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientRecordDto>>> GetAll()
        {
            try
            {
                var result = await mediator.Send(new GetAllPatientRecordsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut("{id:guid}")]
        //public async Task<IActionResult> Update(Guid id, UpdatePatientRecordCommand command)
        //{
        //    if (id != command.PatientRecordId)
        //    {
        //        return BadRequest("The id should be identical with command.PatientRecordId");
        //    }

        //    var result = await mediator.Send(command);
        //    if (result.IsSuccess)
        //    {
        //        return NoContent();
        //    }
        //    return BadRequest(result.ErrorMessage);
        //}

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await mediator.Send(new DeletePatientRecordByIdCommand(id));
                if (result.IsSuccess)
                {
                    return NoContent();
                }
                return BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}