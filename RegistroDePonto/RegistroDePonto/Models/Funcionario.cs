using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

public class Funcionario
{
    [Key]
    public int Matricula { get; set; }

    public string NomeCompleto { get; set; }
    public string Senha { get; set; }
    public string Perfil { get; set; }

    public TimeSpan EntradaManha { get; set; }
    public TimeSpan SaidaManha { get; set; }
    public TimeSpan EntradaTarde { get; set; }
    public TimeSpan SaidaTarde { get; set; }

    [BindNever]
    public ICollection<RegistroPonto> RegistrosPonto { get; set; } = new List<RegistroPonto>();
}
