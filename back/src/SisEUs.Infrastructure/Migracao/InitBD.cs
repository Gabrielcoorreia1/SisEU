using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using SisEUs.Infrastructure.Repositorios;
using SisEUs.Domain.Checkin.Entidades; 
using System;
using System.Collections.Generic;
using System.Linq; 
using BCryptNet = BCrypt.Net.BCrypt; 

namespace SisEUs.Infrastructure.Migracao
{
    public static class InitBD
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Usuarios.AnyAsync())
            {
                return;
            }
            
            string senhaTexto = "Senha@123";
            string senhaHash = BCryptNet.HashPassword(senhaTexto);
            
            var admin = Usuario.CriarAdmin(NomeCompleto.Criar("Admin", "Root"), Cpf.Criar("15887784016"), Email.Criar("admin@siseus.com"), Senha.Criar(senhaHash));   
            var professor = Usuario.CriarEstudante(NomeCompleto.Criar("Carlos", "Professor"), Cpf.Criar("54449817001"), Email.Criar("professor@siseus.com"), Senha.Criar(senhaHash), "67890");
            var ouvinte1 = Usuario.CriarEstudante(NomeCompleto.Criar("Ana", "Ouvinte"), Cpf.Criar("77489284015"), Email.Criar("ouvinte1@siseus.com"), Senha.Criar(senhaHash), "12345");
            var ouvinte2 = Usuario.CriarEstudante(NomeCompleto.Criar("Bruno", "Aluno"), Cpf.Criar("42294419081"), Email.Criar("ouvinte2@siseus.com"), Senha.Criar(senhaHash), "54321");
            var ouvinte3 = Usuario.CriarEstudante(NomeCompleto.Criar("Carla", "Visitante"), Cpf.Criar("22812554096"), Email.Criar("ouvinte3@siseus.com"), Senha.Criar(senhaHash), "98765");

            await context.Usuarios.AddRangeAsync(admin, professor, ouvinte1, ouvinte2, ouvinte3);
            await context.SaveChangesAsync(); 

            Random random = new Random();
            string pinInicial = random.Next(100000, 1000000).ToString("D6");
            var pinAtivo = CheckinPin.Criar(pinInicial);

            await context.CheckinPins.AddAsync(pinAtivo);
            await context.SaveChangesAsync(); 

            var avaliadoresEvento1 = new List<NomeCompleto> { NomeCompleto.Criar("Juliana", "Silva") };
            string avaliadoresConcatenados1 = string.Join("; ", avaliadoresEvento1.Select(a => a.ToString()));

            var evento1 = Evento.Criar(
                Titulo.Criar("A Jornada da Inteligência Artificial"),
                new System.DateTime(2026, 01, 20, 9, 0, 0), 
                new System.DateTime(2026, 01, 20, 10, 0, 0), 
                Local.Criar("Crateus", "1", "B", "2"),
                new List<int> { admin.Id, professor.Id },
                avaliadoresConcatenados1,
                Localizacao.Criar("-5.184846", "-40.651807"),
                "", 
                "I1HGF9",
                ETipoEvento.Oral 
            );

            var avaliadoresEvento2 = new List<NomeCompleto> { NomeCompleto.Criar("Renato", "Oliveira") };
            string avaliadoresConcatenados2 = string.Join("; ", avaliadoresEvento2.Select(a => a.ToString()));

            var evento2 = Evento.Criar(
                Titulo.Criar("Descomplicando o Front-End Moderno"),
                new System.DateTime(2026, 01, 21, 10, 30, 0), 
                new System.DateTime(2026, 01, 21, 11, 30, 0), 
                Local.Criar("Crateus", "1", "B", "2"),
                new List<int> { professor.Id },
                avaliadoresConcatenados2,
                Localizacao.Criar("-5.184846", "-40.651807"),
                "", 
                "7L5B3E",
                ETipoEvento.Banner 
            );
    
            await context.Sessoes.AddRangeAsync(evento1, evento2);
            await context.SaveChangesAsync(); 

            var apresentacao1 = Apresentacao.Criar(evento1.Id, Titulo.Criar("IA Generativa"), "Ana", "Carlos");
            var apresentacao2 = Apresentacao.Criar(evento1.Id, Titulo.Criar("Redes Neurais"), "Bruno", "Carlos"); 
            var apresentacao3 = Apresentacao.Criar(evento2.Id, Titulo.Criar("React vs Vue"), "Carla", "Carlos");

            await context.Apresentacoes.AddRangeAsync(apresentacao1, apresentacao2, apresentacao3);
            await context.SaveChangesAsync();
        }
    }
}