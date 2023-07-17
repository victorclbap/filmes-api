using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Models
{
    public class Cinema
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "O Campo de nome é obrigatório!")]
        public string? Nome { get; set; }

        // para um cinema ser criado, o endereço é obrigatório
        public int EnderecoId { get; set; }
        
        //virtual é possível recuperar uma instância de endereço
        public virtual Endereco Endereco { get; set; }

        public virtual ICollection<Sessao> Sessoes { get; set; }

    }
}
