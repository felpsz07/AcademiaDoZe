//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests;

public class AlunoDomainTests
{
    [Fact]
    public void CriarAluno_Valido_NaoDeveLancarExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        var aluno = Aluno.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509", 
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);
        
        Assert.NotNull(aluno); 
    }

    [Fact]
    public void CriarAluno_Invalido_DeveLancarExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        Assert.Throws<DomainException>(() => Aluno.Criar(0, "Gustavo", "0135", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "cleber", arquivo));
    }

    [Fact]
    public void CriarAluno_Valido_VerificarNormalizado()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        var aluno = Aluno.Criar(0,  "Gustavo ", "013.507.919-54", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "(49) 9 9954-1509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);
        
        Assert.Equal("Gustavo", aluno.Nome);
        Assert.Equal("01350791954", aluno.Cpf);
        Assert.Equal("49999541509", aluno.Telefone);
        Assert.Equal("guvieirawalter@gmail.com", aluno.Email);
    }

    [Fact]
    public void CriarAluno_Invalido_VerificarMessageExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        var exception = Assert.Throws<DomainException>(() => Aluno.Criar(0, "Gustavo", "", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo));
    }
}
