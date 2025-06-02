using System.ComponentModel.DataAnnotations;
public class PlaceBetRequestDto
{
    [Required]
    public Guid PlayerId { get; set; }

    [Required]
    [Range(1.00, (double)decimal.MaxValue, ErrorMessage = "Stake amount must be greater than 1.00.")]
    public decimal Stake { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 200 characters.")]
    public string Description { get; set; } = string.Empty;
}
