using System.ComponentModel.DataAnnotations;

public class Administrador{
    [Key]
    public int Id {get; set;} = default!;

    [Required]
    [StringLength(100)]
    public string Email {get; set;} = default!;
    [Required]
    [StringLength(50)]
    public string Senha {get; set;} = default!;

}