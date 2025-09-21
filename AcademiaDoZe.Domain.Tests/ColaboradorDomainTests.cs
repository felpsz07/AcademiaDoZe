//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests;

public class ColaboradorDomainTests
{
    [Fact]
    public void CriarColaborador_Valido_NaoDeveLancarExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        var colaborador = Colaborador.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro,
            "412", "Perto de uma igreja", "Cleber27!@", arquivo, 
            DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)), EColaboradorTipo.Atendente, EColaboradorVinculo.CLT);
        Assert.NotNull(colaborador);
    }

    [Fact]
    public void CriarColaborador_Invalido_DeveLancarExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0 ,"12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        Assert.Throws<DomainException>(() => Colaborador.Criar(9, "Gustavo", "0104", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49541509",
            "guvieirawalter@gmail.com", logradouro,
            "412", "Perto de uma igreja", "Cleber27!@", arquivo,  
            DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)), EColaboradorTipo.Administrador, EColaboradorVinculo.CLT));
    }

    [Fact]
    public void CriarColaborador_Valido_VerificarNormalizado()
    {
        //Arrange
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        
        //Act
        var colaborador = Colaborador.Criar(0, "Gustavo", "01350791954", DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro,
            "412", "Perto de uma igreja", "Cleber27!@", arquivo, DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)), 
            EColaboradorTipo.Atendente, EColaboradorVinculo.CLT);

        //Assert
        Assert.Equal("Gustavo", colaborador.Nome);
        Assert.Equal("01350791954", colaborador.Cpf);
        Assert.Equal("49999541509", colaborador.Telefone);
        Assert.Equal("guvieirawalter@gmail.com", colaborador.Email);
    }

    [Fact]
    public void CriarColaborador_Invalido_VerificarMessageExcecao()
    {
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
        var logradouro = Logradouro.Criar(0, "12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        var exception = Assert.Throws<DomainException>(() => Colaborador.Criar(0, "", "01350791954", 
            DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), "49999541509",
            "guvieirawalter@gmail.com", logradouro,
            "412", "Perto de uma igreja", "Cleber27!@", arquivo, 
            DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)), 
            EColaboradorTipo.Administrador, EColaboradorVinculo.CLT));
    }
}
