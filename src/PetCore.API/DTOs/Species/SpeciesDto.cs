namespace PetCore.API.DTOs.Species;

public class SpeciesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
    public List<BreedDto> Breeds { get; set; } = [];
}

public class BreedDto
{
    public Guid Id { get; set; }
    public Guid SpeciesId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
}

public class CreateSpeciesRequest
{
    public string Name { get; set; } = string.Empty;
}

public class CreateBreedRequest
{
    public string Name { get; set; } = string.Empty;
}
