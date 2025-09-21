using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    [Fact]
    public async Task Matricula_Adicionar()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var alunoId = 1;
        var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
        Aluno? aluno = await repoAlunoObterPorId.ObterPorId(alunoId);


        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Mensal,
            new DateOnly(2025, 10, 12),
            new DateOnly(2025, 11, 12),
            "Hipertrofia",
            EMatriculaRestricoes.Diabetes,
            "Diabetes faz mal",
            arquivo
            );

        var repoMatriculaAdicionar = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaInserida = await repoMatriculaAdicionar.Adicionar(matricula);
        Assert.NotNull(matriculaInserida);
        Assert.True(matriculaInserida.Id > 0);
    }
    [Fact]
    public async Task Matricula_Atualizar_DeveAlterarDados()
    {
        var matriculaIdParaTestar = 1;
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaExistente = await repoMatricula.ObterPorId(matriculaIdParaTestar);

        Assert.NotNull(matriculaExistente);

        var novoObjetivo = "Emagrecer";
        var matriculaAtualizada = Matricula.Criar(
            id: matriculaExistente.Id,
            alunoMatricula: matriculaExistente.AlunoMatricula, 
            planoMatricula: matriculaExistente.Plano, 
            dataInicio: matriculaExistente.DataInicio,
            dataFinal: matriculaExistente.DataFim, 
            objetivo: novoObjetivo, 
            restricoesMedicas: matriculaExistente.RestricoesMedicas, 
            observacoesRestricoes: matriculaExistente.ObservacoesRestricoes,
            laudoMedico: matriculaExistente.LaudoMedico
        );

        var idProperty = typeof(Entity).GetProperty("Id");
        idProperty?.SetValue(matriculaAtualizada, matriculaExistente.Id);

        var repoAtualizar = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultadoAtualizacao = await repoAtualizar.Atualizar(matriculaAtualizada);

        Assert.NotNull(resultadoAtualizacao);
        Assert.Equal(matriculaExistente.Id, resultadoAtualizacao.Id); 
        Assert.Equal(novoObjetivo, resultadoAtualizacao.Objetivo); 
    }
    [Fact]
    public async Task Matricula_ObterPorAluno_Remover()
    {
        var matriculaIdParaTestar = 1;

        var repoMatriculaObterPorId = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaExistente = await repoMatriculaObterPorId.ObterPorId(matriculaIdParaTestar);

        Assert.NotNull(matriculaExistente);

        var repoRemover = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultadoRemover = await repoRemover.Remover(matriculaExistente.Id);

        Assert.True(resultadoRemover);

        var repoVerificarRemocao = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultadoRemovido = await repoVerificarRemocao.ObterPorId(matriculaExistente.Id);

        Assert.Null(resultadoRemovido);

    }
    [Fact]
    public async Task Matricula_ObterTodos()
    {
        var repoMatriculaRepository = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultado = await repoMatriculaRepository.ObterTodos();
        Assert.NotNull(resultado);
    }
}
