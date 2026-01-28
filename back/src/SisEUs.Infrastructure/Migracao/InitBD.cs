using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using SisEUs.Infrastructure.Repositorios;
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

            var admin = Usuario.CriarAdmin(
                NomeCompleto.Criar("Admin", "Root"),
                Cpf.Criar("15887784016"),
                Email.Criar("admin@siseus.com"),
                Senha.Criar(senhaHash));

            var profCarlos = Usuario.CriarProfessor(
                NomeCompleto.Criar("Carlos", "Eduardo Silva"),
                Cpf.Criar("54449817001"),
                Email.Criar("carlos.silva@siseus.com"),
                Senha.Criar(senhaHash), "P001");

            var profMaria = Usuario.CriarProfessor(
                NomeCompleto.Criar("Maria", "Fernanda Costa"),
                Cpf.Criar("82736451092"),
                Email.Criar("maria.costa@siseus.com"),
                Senha.Criar(senhaHash), "P002");

            var profRoberto = Usuario.CriarProfessor(
                NomeCompleto.Criar("Roberto", "Almeida Junior"),
                Cpf.Criar("91827364501"),
                Email.Criar("roberto.almeida@siseus.com"),
                Senha.Criar(senhaHash), "P003");

            var avalJuliana = Usuario.CriarProfessor(
                NomeCompleto.Criar("Juliana", "Mendes"),
                Cpf.Criar("63606935091"),
                Email.Criar("juliana.mendes@siseus.com"),
                Senha.Criar(senhaHash), "A001");

            var avalRenato = Usuario.CriarProfessor(
                NomeCompleto.Criar("Renato", "Oliveira"),
                Cpf.Criar("34824360064"),
                Email.Criar("renato.oliveira@siseus.com"),
                Senha.Criar(senhaHash), "A002");

            var avalPatricia = Usuario.CriarProfessor(
                NomeCompleto.Criar("Patricia", "Santos"),
                Cpf.Criar("72615948302"),
                Email.Criar("patricia.santos@siseus.com"),
                Senha.Criar(senhaHash), "A003");

            var avalFernando = Usuario.CriarProfessor(
                NomeCompleto.Criar("Fernando", "Souza"),
                Cpf.Criar("48291637502"),
                Email.Criar("fernando.souza@siseus.com"),
                Senha.Criar(senhaHash), "A004");

            var avalCamila = Usuario.CriarProfessor(
                NomeCompleto.Criar("Camila", "Rodrigues"),
                Cpf.Criar("85739264101"),
                Email.Criar("camila.rodrigues@siseus.com"),
                Senha.Criar(senhaHash), "A005");

            var avalLucas = Usuario.CriarProfessor(
                NomeCompleto.Criar("Lucas", "Pereira"),
                Cpf.Criar("19283746502"),
                Email.Criar("lucas.pereira@siseus.com"),
                Senha.Criar(senhaHash), "A006");

            var estAna = Usuario.CriarEstudante(
                NomeCompleto.Criar("Ana", "Beatriz Lima"),
                Cpf.Criar("77489284015"),
                Email.Criar("ana.lima@siseus.com"),
                Senha.Criar(senhaHash), "2024001");

            var estBruno = Usuario.CriarEstudante(
                NomeCompleto.Criar("Bruno", "Henrique Castro"),
                Cpf.Criar("42294419081"),
                Email.Criar("bruno.castro@siseus.com"),
                Senha.Criar(senhaHash), "2024002");

            var estCarla = Usuario.CriarEstudante(
                NomeCompleto.Criar("Carla", "Dias Ferreira"),
                Cpf.Criar("22812554096"),
                Email.Criar("carla.ferreira@siseus.com"),
                Senha.Criar(senhaHash), "2024003");

            var estDaniel = Usuario.CriarEstudante(
                NomeCompleto.Criar("Daniel", "Martins"),
                Cpf.Criar("36925814702"),
                Email.Criar("daniel.martins@siseus.com"),
                Senha.Criar(senhaHash), "2024004");

            var estElena = Usuario.CriarEstudante(
                NomeCompleto.Criar("Elena", "Ribeiro"),
                Cpf.Criar("74185296301"),
                Email.Criar("elena.ribeiro@siseus.com"),
                Senha.Criar(senhaHash), "2024005");

            var estFabio = Usuario.CriarEstudante(
                NomeCompleto.Criar("Fabio", "Gomes"),
                Cpf.Criar("95173846201"),
                Email.Criar("fabio.gomes@siseus.com"),
                Senha.Criar(senhaHash), "2024006");

            var estGabriela = Usuario.CriarEstudante(
                NomeCompleto.Criar("Gabriela", "Nunes"),
                Cpf.Criar("68427159302"),
                Email.Criar("gabriela.nunes@siseus.com"),
                Senha.Criar(senhaHash), "2024007");

            var estHugo = Usuario.CriarEstudante(
                NomeCompleto.Criar("Hugo", "Teixeira"),
                Cpf.Criar("31597428601"),
                Email.Criar("hugo.teixeira@siseus.com"),
                Senha.Criar(senhaHash), "2024008");

            var estIsabela = Usuario.CriarEstudante(
                NomeCompleto.Criar("Isabela", "Moreira"),
                Cpf.Criar("52749618302"),
                Email.Criar("isabela.moreira@siseus.com"),
                Senha.Criar(senhaHash), "2024009");

            var estJoao = Usuario.CriarEstudante(
                NomeCompleto.Criar("João", "Victor Souza"),
                Cpf.Criar("84629175301"),
                Email.Criar("joao.souza@siseus.com"),
                Senha.Criar(senhaHash), "2024010");

            var estKarina = Usuario.CriarEstudante(
                NomeCompleto.Criar("Karina", "Lopes"),
                Cpf.Criar("17395824601"),
                Email.Criar("karina.lopes@siseus.com"),
                Senha.Criar(senhaHash), "2024011");

            var estLeonardo = Usuario.CriarEstudante(
                NomeCompleto.Criar("Leonardo", "Barros"),
                Cpf.Criar("49572836102"),
                Email.Criar("leonardo.barros@siseus.com"),
                Senha.Criar(senhaHash), "2024012");

            await context.Usuarios.AddRangeAsync(
                admin,
                profCarlos, profMaria, profRoberto,
                avalJuliana, avalRenato, avalPatricia, avalFernando, avalCamila, avalLucas,
                estAna, estBruno, estCarla, estDaniel, estElena, estFabio,
                estGabriela, estHugo, estIsabela, estJoao, estKarina, estLeonardo
            );
            await context.SaveChangesAsync();

            Random random = new();
            string pinInicial = random.Next(100000, 1000000).ToString("D6");
            var pinAtivo = CheckinPin.Criar(pinInicial);
            await context.CheckinPins.AddAsync(pinAtivo);
            await context.SaveChangesAsync();

            var eventoTech = Evento.Criar(
                Titulo.Criar("Semana de Tecnologia e Inovação 2024"),
                new DateTime(2024, 11, 15, 8, 0, 0),
                new DateTime(2024, 11, 15, 18, 0, 0),
                Local.Criar("Crateus", "Computação", "B", "Auditório"),
                [admin.Id, profCarlos.Id, profMaria.Id],
                [avalJuliana.Id, avalRenato.Id, avalPatricia.Id],
                Localizacao.Criar("-5.184846", "-40.651807"),
                "https://exemplo.com/semana-tech.jpg",
                "TECH24",
                ETipoEvento.Oral
            );

            var eventoIA = Evento.Criar(
                Titulo.Criar("Encontro de Inteligência Artificial"),
                new DateTime(2025, 01, 20, 9, 0, 0),
                new DateTime(2025, 01, 20, 17, 0, 0),
                Local.Criar("Crateus", "Computação", "A", "101"),
                [profCarlos.Id, profRoberto.Id],
                [avalFernando.Id, avalCamila.Id, avalLucas.Id],
                Localizacao.Criar("-5.184846", "-40.651807"),
                "https://exemplo.com/encontro-ia.jpg",
                "IA2025",
                ETipoEvento.Oral
            );

            var eventoMostra = Evento.Criar(
                Titulo.Criar("Mostra de Projetos de Extensão"),
                new DateTime(2025, 03, 10, 14, 0, 0),
                new DateTime(2025, 03, 10, 20, 0, 0),
                Local.Criar("Fortaleza", "Engenharia", "C", "Salão Principal"),
                [profMaria.Id],
                [avalJuliana.Id, avalRenato.Id],
                Localizacao.Criar("-3.7436587", "-38.5410718"),
                "https://exemplo.com/mostra-projetos.jpg",
                "MOSTRA25",
                ETipoEvento.Banner
            );

            var eventoWeb = Evento.Criar(
                Titulo.Criar("Workshop de Desenvolvimento Web Moderno"),
                new DateTime(2024, 10, 05, 8, 0, 0),
                new DateTime(2024, 10, 05, 12, 0, 0),
                Local.Criar("Crateus", "Computação", "B", "Lab 01"),
                [profRoberto.Id],
                [avalPatricia.Id, avalFernando.Id],
                Localizacao.Criar("-5.184846", "-40.651807"),
                "https://exemplo.com/workshop-web.jpg",
                "WEB2024",
                ETipoEvento.Pitch
            );

            var eventoSimposio = Evento.Criar(
                Titulo.Criar("Simpósio de Pesquisa Científica"),
                new DateTime(2025, 05, 20, 8, 0, 0),
                new DateTime(2025, 05, 22, 18, 0, 0),
                Local.Criar("Fortaleza", "Centro de Ciências", "D", "Auditório Central"),
                [admin.Id, profCarlos.Id, profMaria.Id, profRoberto.Id],
                [avalJuliana.Id, avalRenato.Id, avalPatricia.Id, avalFernando.Id, avalCamila.Id, avalLucas.Id],
                Localizacao.Criar("-3.7436587", "-38.5410718"),
                "https://exemplo.com/simposio.jpg",
                "SIMP25",
                ETipoEvento.Oral
            );

            var eventoHack = Evento.Criar(
                Titulo.Criar("Hackathon de Soluções Sustentáveis"),
                new DateTime(2025, 01, 25, 8, 0, 0),
                new DateTime(2025, 01, 26, 20, 0, 0),
                Local.Criar("Crateus", "Computação", "A", "Labs 01-03"),
                [profCarlos.Id, profRoberto.Id],
                [avalCamila.Id, avalLucas.Id],
                Localizacao.Criar("-5.184846", "-40.651807"),
                "https://exemplo.com/hackathon.jpg",
                "HACK25",
                ETipoEvento.Pitch
            );

            await context.Eventos.AddRangeAsync(eventoTech, eventoIA, eventoMostra, eventoWeb, eventoSimposio, eventoHack);
            await context.SaveChangesAsync();

            // Criando apresentações com AutorId e OrientadorId
            var apTech1 = Apresentacao.Criar(eventoTech.Id, Titulo.Criar("Aplicações de Machine Learning na Saúde"), estAna.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apTech2 = Apresentacao.Criar(eventoTech.Id, Titulo.Criar("Blockchain para Rastreabilidade Alimentar"), estBruno.Id, profMaria.Id, EModalidadeApresentacao.Oral);
            var apTech3 = Apresentacao.Criar(eventoTech.Id, Titulo.Criar("IoT na Agricultura Familiar"), estCarla.Id, profRoberto.Id, EModalidadeApresentacao.Oral);
            var apTech4 = Apresentacao.Criar(eventoTech.Id, Titulo.Criar("Realidade Aumentada na Educação"), estDaniel.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apTech5 = Apresentacao.Criar(eventoTech.Id, Titulo.Criar("Chatbots Inteligentes para Atendimento"), estElena.Id, profMaria.Id, EModalidadeApresentacao.Oral);

            var apIA1 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("Redes Neurais Convolucionais para Diagnóstico Médico"), estFabio.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apIA2 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("Processamento de Linguagem Natural em Português"), estGabriela.Id, profRoberto.Id, EModalidadeApresentacao.Oral);
            var apIA3 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("Visão Computacional para Segurança Pública"), estHugo.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apIA4 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("IA Generativa: Tendências e Aplicações"), estIsabela.Id, profRoberto.Id, EModalidadeApresentacao.Oral);
            var apIA5 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("Sistemas de Recomendação com Deep Learning"), estJoao.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apIA6 = Apresentacao.Criar(eventoIA.Id, Titulo.Criar("Ética em Inteligência Artificial"), estKarina.Id, profRoberto.Id, EModalidadeApresentacao.Oral);

            var apMostra1 = Apresentacao.Criar(eventoMostra.Id, Titulo.Criar("Horta Comunitária Inteligente"), estLeonardo.Id, profMaria.Id, EModalidadeApresentacao.Poster);
            var apMostra2 = Apresentacao.Criar(eventoMostra.Id, Titulo.Criar("App de Carona Solidária"), estAna.Id, profMaria.Id, EModalidadeApresentacao.Poster);
            var apMostra3 = Apresentacao.Criar(eventoMostra.Id, Titulo.Criar("Plataforma de Doações"), estBruno.Id, profMaria.Id, EModalidadeApresentacao.Poster);
            var apMostra4 = Apresentacao.Criar(eventoMostra.Id, Titulo.Criar("Sistema de Gestão para ONGs"), estCarla.Id, profMaria.Id, EModalidadeApresentacao.Poster);

            var apWeb1 = Apresentacao.Criar(eventoWeb.Id, Titulo.Criar("API REST com .NET 8"), estDaniel.Id, profRoberto.Id, EModalidadeApresentacao.Pitch);
            var apWeb2 = Apresentacao.Criar(eventoWeb.Id, Titulo.Criar("Frontend com React e TypeScript"), estElena.Id, profRoberto.Id, EModalidadeApresentacao.Pitch);
            var apWeb3 = Apresentacao.Criar(eventoWeb.Id, Titulo.Criar("Microsserviços com Docker"), estFabio.Id, profRoberto.Id, EModalidadeApresentacao.Pitch);

            var apSimp1 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("Computação Quântica: Estado da Arte"), estGabriela.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apSimp2 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("Segurança Cibernética em Infraestruturas Críticas"), estHugo.Id, profMaria.Id, EModalidadeApresentacao.Oral);
            var apSimp3 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("Big Data na Tomada de Decisões"), estIsabela.Id, profRoberto.Id, EModalidadeApresentacao.Oral);
            var apSimp4 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("Computação em Nuvem Verde"), estJoao.Id, profCarlos.Id, EModalidadeApresentacao.Oral);
            var apSimp5 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("Acessibilidade Digital"), estKarina.Id, profMaria.Id, EModalidadeApresentacao.Oral);
            var apSimp6 = Apresentacao.Criar(eventoSimposio.Id, Titulo.Criar("DevOps e Cultura de Qualidade"), estLeonardo.Id, profRoberto.Id, EModalidadeApresentacao.Oral);

            var apHack1 = Apresentacao.Criar(eventoHack.Id, Titulo.Criar("App de Coleta Seletiva Gamificada"), estAna.Id, profCarlos.Id, EModalidadeApresentacao.Pitch);
            var apHack2 = Apresentacao.Criar(eventoHack.Id, Titulo.Criar("Plataforma de Energia Solar Compartilhada"), estBruno.Id, profRoberto.Id, EModalidadeApresentacao.Pitch);
            var apHack3 = Apresentacao.Criar(eventoHack.Id, Titulo.Criar("Sistema de Monitoramento de Água"), estCarla.Id, profCarlos.Id, EModalidadeApresentacao.Pitch);
            var apHack4 = Apresentacao.Criar(eventoHack.Id, Titulo.Criar("Marketplace de Produtos Orgânicos"), estDaniel.Id, profRoberto.Id, EModalidadeApresentacao.Pitch);

            await context.Apresentacoes.AddRangeAsync(
                apTech1, apTech2, apTech3, apTech4, apTech5,
                apIA1, apIA2, apIA3, apIA4, apIA5, apIA6,
                apMostra1, apMostra2, apMostra3, apMostra4,
                apWeb1, apWeb2, apWeb3,
                apSimp1, apSimp2, apSimp3, apSimp4, apSimp5, apSimp6,
                apHack1, apHack2, apHack3, apHack4
            );
            await context.SaveChangesAsync();

            var avaliacoes = new List<Avaliacao>();

            var avTech1_1 = Avaliacao.Iniciar(apTech1.Id, avalJuliana.Id);
            avTech1_1.Avaliar(9.5m, "Excelente trabalho! Metodologia sólida e resultados promissores. Aplicação prática muito relevante para a área de saúde.");
            var avTech1_2 = Avaliacao.Iniciar(apTech1.Id, avalRenato.Id);
            avTech1_2.Avaliar(8.5m, "Boa apresentação, conteúdo bem estruturado. Sugiro aprofundar a discussão sobre limitações do modelo.");
            var avTech1_3 = Avaliacao.Iniciar(apTech1.Id, avalPatricia.Id);
            avTech1_3.Avaliar(9.0m, "Trabalho inovador com grande potencial de impacto social. Apresentação clara e objetiva.");

            var avTech2_1 = Avaliacao.Iniciar(apTech2.Id, avalJuliana.Id);
            avTech2_1.Avaliar(8.0m, "Proposta interessante. A implementação do blockchain está bem fundamentada, mas faltou demonstração prática.");
            var avTech2_2 = Avaliacao.Iniciar(apTech2.Id, avalRenato.Id);
            avTech2_2.Avaliar(7.5m, "Conceito válido, porém a escalabilidade precisa ser melhor abordada.");

            var avTech3_1 = Avaliacao.Iniciar(apTech3.Id, avalPatricia.Id);
            avTech3_1.Avaliar(9.0m, "Projeto com forte impacto social. A integração IoT foi bem executada.");
            var avTech3_2 = Avaliacao.Iniciar(apTech3.Id, avalJuliana.Id);
            avTech3_2.Avaliar(8.5m, "Muito relevante para o contexto regional. Boa documentação técnica.");

            var avTech4_1 = Avaliacao.Iniciar(apTech4.Id, avalRenato.Id);
            avTech4_1.Avaliar(8.0m, "Aplicação educacional interessante. Interface bem projetada.");
            var avTech4_2 = Avaliacao.Iniciar(apTech4.Id, avalPatricia.Id);
            avTech4_2.Avaliar(8.5m, "Potencial pedagógico significativo. Testes com usuários foram convincentes.");

            var avTech5_1 = Avaliacao.Iniciar(apTech5.Id, avalJuliana.Id);
            avTech5_1.Avaliar(7.5m, "Chatbot funcional, mas precisa melhorar a compreensão de contexto.");
            var avTech5_2 = Avaliacao.Iniciar(apTech5.Id, avalRenato.Id);
            avTech5_2.Avaliar(7.0m, "Ideia boa, implementação básica. Sugestão: incorporar mais intents.");

            var avIA1_1 = Avaliacao.Iniciar(apIA1.Id, avalFernando.Id);
            avIA1_1.Avaliar(9.0m, "Modelo CNN bem treinado com excelente acurácia. Dataset adequado.");
            var avIA1_2 = Avaliacao.Iniciar(apIA1.Id, avalCamila.Id);
            avIA1_2.Avaliar(8.5m, "Resultados impressionantes. Sugiro validação com mais especialistas médicos.");

            var avIA2_1 = Avaliacao.Iniciar(apIA2.Id, avalLucas.Id);
            var avIA2_2 = Avaliacao.Iniciar(apIA2.Id, avalFernando.Id);
            avIA2_2.Avaliar(8.0m, "Corpus bem construído. Modelo apresenta bons resultados para português brasileiro.");

            var avIA3_1 = Avaliacao.Iniciar(apIA3.Id, avalCamila.Id);
            avIA3_1.Avaliar(7.5m, "Questões de privacidade precisam ser melhor endereçadas.");
            var avIA3_2 = Avaliacao.Iniciar(apIA3.Id, avalLucas.Id);

            var avIA5_1 = Avaliacao.Iniciar(apIA5.Id, avalFernando.Id);

            var avIA6_1 = Avaliacao.Iniciar(apIA6.Id, avalCamila.Id);
            avIA6_1.Avaliar(9.5m, "Discussão ética fundamental e muito bem conduzida. Referências atualizadas.");

            var avWeb1_1 = Avaliacao.Iniciar(apWeb1.Id, avalPatricia.Id);
            avWeb1_1.Avaliar(9.0m, "API bem estruturada seguindo boas práticas. Documentação Swagger completa.");
            var avWeb1_2 = Avaliacao.Iniciar(apWeb1.Id, avalFernando.Id);
            avWeb1_2.Avaliar(8.5m, "Código limpo e bem organizado. Testes unitários presentes.");

            var avWeb2_1 = Avaliacao.Iniciar(apWeb2.Id, avalPatricia.Id);
            avWeb2_1.Avaliar(8.0m, "Componentes bem modularizados. Pode melhorar a gestão de estado.");
            var avWeb2_2 = Avaliacao.Iniciar(apWeb2.Id, avalFernando.Id);
            avWeb2_2.Avaliar(8.5m, "Boa aplicação de TypeScript. UI responsiva e acessível.");

            var avWeb3_1 = Avaliacao.Iniciar(apWeb3.Id, avalPatricia.Id);
            avWeb3_1.Avaliar(7.5m, "Docker compose funcional. Faltou demonstrar orquestração com Kubernetes.");
            var avWeb3_2 = Avaliacao.Iniciar(apWeb3.Id, avalFernando.Id);
            avWeb3_2.Avaliar(8.0m, "Arquitetura de microsserviços bem planejada. Comunicação entre serviços adequada.");

            var avHack1_1 = Avaliacao.Iniciar(apHack1.Id, avalCamila.Id);
            var avHack1_2 = Avaliacao.Iniciar(apHack1.Id, avalLucas.Id);
            avHack1_2.Avaliar(8.5m, "Gamificação bem implementada. App intuitivo e engajante.");

            var avHack2_1 = Avaliacao.Iniciar(apHack2.Id, avalCamila.Id);

            var avHack3_1 = Avaliacao.Iniciar(apHack3.Id, avalLucas.Id);
            avHack3_1.Avaliar(9.0m, "Solução completa com sensores e dashboard. Grande potencial de mercado.");

            avaliacoes.AddRange(new[]
            {
                avTech1_1, avTech1_2, avTech1_3,
                avTech2_1, avTech2_2,
                avTech3_1, avTech3_2,
                avTech4_1, avTech4_2,
                avTech5_1, avTech5_2,
                avIA1_1, avIA1_2,
                avIA2_1, avIA2_2,
                avIA3_1, avIA3_2,
                avIA5_1,
                avIA6_1,
                avWeb1_1, avWeb1_2,
                avWeb2_1, avWeb2_2,
                avWeb3_1, avWeb3_2,
                avHack1_1, avHack1_2,
                avHack2_1,
                avHack3_1
            });

            await context.Avaliacoes.AddRangeAsync(avaliacoes);
            await context.SaveChangesAsync();

            var presencas = new List<Presenca>();

            var presTech1 = Presenca.Criar(estAna.Id, eventoTech.Id, "-5.184846", "-40.651807");
            presTech1.RealizarCheckIn(new DateTime(2024, 11, 15, 8, 15, 0));
            presTech1.RealizarCheckOut(new DateTime(2024, 11, 15, 17, 45, 0));

            var presTech2 = Presenca.Criar(estBruno.Id, eventoTech.Id, "-5.184846", "-40.651807");
            presTech2.RealizarCheckIn(new DateTime(2024, 11, 15, 8, 05, 0));
            presTech2.RealizarCheckOut(new DateTime(2024, 11, 15, 18, 00, 0));

            var presTech3 = Presenca.Criar(estCarla.Id, eventoTech.Id, "-5.184846", "-40.651807");
            presTech3.RealizarCheckIn(new DateTime(2024, 11, 15, 8, 30, 0));
            presTech3.RealizarCheckOut(new DateTime(2024, 11, 15, 16, 30, 0));

            var presTech4 = Presenca.Criar(estDaniel.Id, eventoTech.Id, "-5.184846", "-40.651807");
            presTech4.RealizarCheckIn(new DateTime(2024, 11, 15, 9, 00, 0));
            presTech4.RealizarCheckOut(new DateTime(2024, 11, 15, 17, 00, 0));

            var presTech5 = Presenca.Criar(estElena.Id, eventoTech.Id, "-5.184846", "-40.651807");
            presTech5.RealizarCheckIn(new DateTime(2024, 11, 15, 8, 10, 0));
            presTech5.RealizarCheckOut(new DateTime(2024, 11, 15, 17, 50, 0));

            var presWeb1 = Presenca.Criar(estDaniel.Id, eventoWeb.Id, "-5.184846", "-40.651807");
            presWeb1.RealizarCheckIn(new DateTime(2024, 10, 05, 8, 00, 0));
            presWeb1.RealizarCheckOut(new DateTime(2024, 10, 05, 12, 00, 0));

            var presWeb2 = Presenca.Criar(estElena.Id, eventoWeb.Id, "-5.184846", "-40.651807");
            presWeb2.RealizarCheckIn(new DateTime(2024, 10, 05, 8, 10, 0));
            presWeb2.RealizarCheckOut(new DateTime(2024, 10, 05, 11, 50, 0));

            var presWeb3 = Presenca.Criar(estFabio.Id, eventoWeb.Id, "-5.184846", "-40.651807");
            presWeb3.RealizarCheckIn(new DateTime(2024, 10, 05, 8, 05, 0));
            presWeb3.RealizarCheckOut(new DateTime(2024, 10, 05, 12, 00, 0));

            presencas.AddRange(new[] { presTech1, presTech2, presTech3, presTech4, presTech5, presWeb1, presWeb2, presWeb3 });

            await context.Presencas.AddRangeAsync(presencas);
            await context.SaveChangesAsync();

            var checkins = new List<Checkin>
            {
                Checkin.Criar(estAna.Id, pinAtivo.Id, -5.184846, -40.651807),
                Checkin.Criar(estBruno.Id, pinAtivo.Id, -5.184846, -40.651807),
                Checkin.Criar(estCarla.Id, pinAtivo.Id, -5.184846, -40.651807),
            };

            checkins[0].RegistrarCheckOut();
            checkins[1].RegistrarCheckOut();

            await context.Checkins.AddRangeAsync(checkins);
            await context.SaveChangesAsync();
        }
    }
}