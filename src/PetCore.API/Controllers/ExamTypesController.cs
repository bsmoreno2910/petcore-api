using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Exams;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/exam-types")]
[Authorize]
public class ExamTypesController : ControllerBase
{
    private readonly ExamService _examService;
    private readonly IMapper _mapper;

    public ExamTypesController(ExamService examService, IMapper mapper)
    {
        _examService = examService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _examService.GetExamTypesAsync();
        return Ok(_mapper.Map<List<ExamTypeDto>>(types));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateExamTypeRequest request)
    {
        var examType = await _examService.CreateExamTypeAsync(request.Name, request.Category, request.DefaultPrice);
        return Created("/api/exam-types", _mapper.Map<ExamTypeDto>(examType));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExamTypeRequest request)
    {
        var examType = await _examService.UpdateExamTypeAsync(id, request.Name, request.Category, request.DefaultPrice);
        return examType == null ? NotFound() : Ok(_mapper.Map<ExamTypeDto>(examType));
    }
}
