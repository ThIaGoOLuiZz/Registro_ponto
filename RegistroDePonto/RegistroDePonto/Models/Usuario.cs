using System.ComponentModel.DataAnnotations;

namespace RegistroDePonto.Models
{
    public class Usuario
    {
        [Key]
        public int Matricula { get; set; }

        public string Senha { get; set; }
        public string Perfil { get; set; }
    }
}