using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Species;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/species")]
[Authorize]
public class SpeciesController : ControllerBase
{
    private readonly SpeciesService _speciesService;
    private readonly IMapper _mapper;

    public SpeciesController(SpeciesService speciesService, IMapper mapper)
    {
        _speciesService = speciesService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var species = await _speciesService.GetAllAsync();
        return Ok(_mapper.Map<List<SpeciesDto>>(species));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSpeciesRequest request)
    {
        try
        {
            var species = await _speciesService.CreateAsync(request.Name);
            return Created($"/api/species", _mapper.Map<SpeciesDto>(species));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/breeds")]
    public async Task<IActionResult> GetBreeds(Guid id)
    {
        var breeds = await _speciesService.GetBreedsAsync(id);
        return Ok(_mapper.Map<List<BreedDto>>(breeds));
    }

    [HttpPost("{id:guid}/breeds")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateBreed(Guid id, [FromBody] CreateBreedRequest request)
    {
        try
        {
            var breed = await _speciesService.CreateBreedAsync(id, request.Name);
            return Created($"/api/species/{id}/breeds", _mapper.Map<BreedDto>(breed));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
