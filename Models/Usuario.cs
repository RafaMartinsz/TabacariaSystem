using System.ComponentModel.DataAnnotations;

namespace TabacariaSystem.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NomeUsuario { get; set; }

        [Required]
        public string Senha { get; set; } // Aqui vai ficar o Hash da senha
    }
}