using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    [Fact]
    public async Task Aluno_LogradouroPorId_CpfJaExiste_Adicionar()
    {

        var logradouroId = 2;
        var repoLogradouroObterPorId = new LogradouroRepository(ConnectionString, DatabaseType);
        Logradouro logradouro = await repoLogradouroObterPorId.ObterPorId(logradouroId);
        Assert.NotNull(logradouro); // Garante que o logradouro existe

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");

        var _cpf = "12345678900";

        var repoAlunoCpf = new AlunoRepository(ConnectionString, DatabaseType);

        var cpfExistente = await repoAlunoCpf.CpfJaExiste(_cpf);
        Assert.False(cpfExistente, "CPF já existe no banco de dados.");
        var aluno = Aluno.Criar(
        0,
        "zé",
        _cpf,

        new DateOnly(2010, 10, 09),
        "49999999999",
        "ze@com.br",
        logradouro,
        "123",
        "complemento casa",
        "abcBolinhas",
        arquivo
        );
        // Adicionar

        var repoAlunoAdicionar = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoInserido = await repoAlunoAdicionar.Adicionar(aluno);
        Assert.NotNull(alunoInserido);
        Assert.True(alunoInserido.Id > 0);

    }
    [Fact]
    public async Task Aluno_ObterPorCpf_Atualizar()
    {
        var _cpf = "12345678900";
        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);

        var alunoAtualizado = Aluno.Criar(
        0,
        "zé dos testes 123",
        alunoExistente.Cpf,
        alunoExistente.DataNascimento,
        alunoExistente.Telefone,
        alunoExistente.Email,
        alunoExistente.Endereco,
        alunoExistente.Numero,
        alunoExistente.Complemento,
        alunoExistente.Senha,
        arquivo
        );

        var idProperty = typeof(Entity).GetProperty("Id");

        idProperty?.SetValue(alunoAtualizado, alunoExistente.Id);
        // Teste de Atualização

        var repoAlunoAtualizar = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoAtualizacao = await repoAlunoAtualizar.Atualizar(alunoAtualizado);
        Assert.NotNull(resultadoAtualizacao);

        Assert.Equal("zé dos testes 123", resultadoAtualizacao.Nome);

    }
    [Fact]
    public async Task Aluno_ObterPorCpf_TrocarSenha()
    {
        var _cpf = "12345678900";
        //Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);
        var novaSenha = "novaSenha123";
        var repoAlunoTrocarSenha = new AlunoRepository(ConnectionString, DatabaseType);

        var resultadoTrocaSenha = await repoAlunoTrocarSenha.TrocarSenha(alunoExistente.Id, novaSenha);
        Assert.True(resultadoTrocaSenha);

        var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoAtualizado = await repoAlunoObterPorId.ObterPorId(alunoExistente.Id);
        Assert.NotNull(alunoAtualizado);
        Assert.Equal(novaSenha, alunoAtualizado.Senha);
    }
    [Fact]
    public async Task Aluno_ObterPorCpf_Remover_ObterPorId()
    {
        var _cpf = "12345678900";
        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);

        var repoAlunoRemover = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoRemover = await repoAlunoRemover.Remover(alunoExistente.Id);
        Assert.True(resultadoRemover);

        var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoRemovido = await repoAlunoObterPorId.ObterPorId(alunoExistente.Id);
        Assert.Null(resultadoRemovido);
    }
    [Fact]
    public async Task Aluno_ObterTodos()
    {
        var repoAlunoRepository = new ColaboradorRepository(ConnectionString, DatabaseType);
        var resultado = await repoAlunoRepository.ObterTodos();
        Assert.NotNull(resultado);
    } 
}
