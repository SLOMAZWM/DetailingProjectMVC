using ProjektLABDetailing.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CarImage
{
    [Key]
    public int ImageId { get; set; }

    [Required]
    public int CarId { get; set; }

    [ForeignKey("CarId")]
    public virtual Car Car { get; set; }

    [Required]
    public string ImagePath { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
}
