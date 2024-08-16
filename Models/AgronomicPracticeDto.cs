﻿namespace Agri_Smart.Models
{
    public class AgronomicPracticeDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Description { get; set; }
        public string? Image { get; set; }
        public List<AgronomicDetailDto>? AgronomicDetails { get; set; }
    }
    public class AgronomicDetailDto
    {
        public Guid Id { get; set; }
        public string? CoffeeType { get; set; }
        public string? PlantingPhase { get; set; }
        public string? DetailType { get; set; }
        public List<string>? Description { get; set; }
    }
}
