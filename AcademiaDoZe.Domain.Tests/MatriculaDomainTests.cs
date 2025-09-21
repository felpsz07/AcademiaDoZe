//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests;

public class MatriculaDomainTests
{

    [Fact]
    public void CriarMatricula_Valido_NaoDeveLancarExcecao()
    {

        //Arrange
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        var aluno = Aluno.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);

        //Act
        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        var matricula = Matricula.Criar(0, aluno, EMatriculaPlano.Mensal, dataInicio, dataFinal, "Hipertrofia", 
            EMatriculaRestricoes.None, "Nenhum", arquivo);
        
        //Assert
        Assert.NotNull(matricula);
    }

    [Fact]
    public void CriarMatricula_Invalido_DeveLancarExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        var aluno = Aluno.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);

        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        Assert.Throws<DomainException>(() => Matricula.Criar(0, aluno, EMatriculaPlano.Mensal, dataInicio, dataFinal, "",
            EMatriculaRestricoes.None, "Nenhum", arquivo));
    }

    [Fact]
    public void CriarMatricula_Invalido_VerificarNormalizado()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        var aluno = Aluno.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);

        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        var matricula = Matricula.Criar(0, aluno, EMatriculaPlano.Mensal, dataInicio, dataFinal, "Hipertrofia ",
            EMatriculaRestricoes.None, "Nenhum ", arquivo);

        Assert.Equal("Hipertrofia", matricula.Objetivo);
        Assert.Equal("Nenhum", matricula.ObservacoesRestricoes);
    }

    [Fact]
    public void CriarMatricula_Invalido_VerificarMessageExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        var aluno = Aluno.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro, "412", "Perto de uma igreja", "Cleber27!@", arquivo);

        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        var exception = Assert.Throws<DomainException>(() => Matricula.Criar(0, aluno, EMatriculaPlano.Mensal, dataInicio, dataFinal, "",
            EMatriculaRestricoes.None, " ", arquivo));
    }
 }
