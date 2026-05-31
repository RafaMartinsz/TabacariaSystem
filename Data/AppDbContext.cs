using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TabacariaSystem.Models;

namespace TabacariaSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Criamos a lista que vai receber os 10 acessos fictícios
            var usuariosIniciais = new List<Usuario>();
            string[] nomes = { "Admin", "Carlos", "Mariana", "Fernanda", "Roberto", "Juliana", "Lucas", "Beatriz", "Ricardo", "Amanda" };

            // 2. String fixa e estática gerada pelo .NET para a senha "Tabacaria123!"
            // Isso engana o EF impedindo que ele ache que o dado muda toda hora
            string hashFixoSenha = "AQAAAAIAAYagAAAAEGmN2K38x1LqWbREk7K1VpXpQvZaRlTk9w==";

            for (int i = 1; i <= 10; i++)
            {
                var user = new Usuario
                {
                    Id = i + 100, // Evita conflito com IDs existentes na sua máquina
                    NomeUsuario = nomes[i - 1].ToLower(), // Fica como: admin, carlos...
                    Senha = hashFixoSenha // <-- Aqui usamos a string de segurança estática
                };

                usuariosIniciais.Add(user);
            }

            // 3. Injeta com segurança os 10 usuários de uma vez só na tabela
            modelBuilder.Entity<Usuario>().HasData(usuariosIniciais);
        }
    }
}
