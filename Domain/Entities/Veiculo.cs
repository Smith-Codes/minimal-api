using System.ComponentModel.DataAnnotations;

public class Veiculo
{
    [Key]
    public int Id { get; set; } = default!;
    
    [Required]
    [StringLength(100)]
    public string Nome {get; set;} = default!;
    [Required]
    [StringLength(100)]
    public string Marca {get; set;} = default!;
    [Required]
    public int Ano {get; set;} = default!;

}