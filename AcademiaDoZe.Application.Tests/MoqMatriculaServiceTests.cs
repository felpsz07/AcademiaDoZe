//Felipe Bueno de Oliveira
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using Moq;

namespace AcademiaDoZe.Application.Tests;

public class MoqMatriculaServiceTests
{
    private readonly Mock<IMatriculaService> _matriculaServiceMock;
    private readonly IMatriculaService _matriculaService;

    public MoqMatriculaServiceTests()
    {
        _matriculaServiceMock = new Mock<IMatriculaService>();
        _matriculaService = _matriculaServiceMock.Object;
    }

    private MatriculaDTO CriarMatriculaPadrao(int id = 1)
    {
        return new MatriculaDTO
        {
            Id = id,
            AlunoMatricula = new AlunoDTO {
                Id = id,
                Nome = "Aluno Teste",
                Cpf = "12345678900",
                DataNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)),
                Telefone = "49999541509",
                Email = "aluno@teste.com",
                Endereco = new LogradouroDTO { Id = 1, Cep = "12345678", Nome = "Rua Teste", Bairro = "Centro", Cidade = "São Paulo", Estado = "SP", Pais = "Brasil" },
                Numero = "123",
                Complemento = "Apto 123",
                Senha = "SenhaForte!23"
            },
            Plano = Enums.EAppMatriculaPlano.Mensal,
            DataInicio = new DateOnly(2025, 9, 1),
            DataFim = new DateOnly(2025, 10, 1),
            Objetivo = "Hipertrofia",
            RestricoesMedicas = Enums.EAppMatriculaRestricoes.None,
            ObservacoesRestricoes = string.Empty
        };
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarMatricula_QuandoExistir()
    {
        // Arrange
        var matriculaId = 1;
        var matriculaDto = CriarMatriculaPadrao(matriculaId);
        _matriculaServiceMock.Setup(s => s.ObterPorIdAsync(matriculaId)).ReturnsAsync(matriculaDto);

        // Act
        var result = await _matriculaService.ObterPorIdAsync(matriculaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(matriculaId, result.Id);
        _matriculaServiceMock.Verify(s => s.ObterPorIdAsync(matriculaId), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExistir()
    {
        // Arrange
        var matriculaId = 999;
        _matriculaServiceMock.Setup(s => s.ObterPorIdAsync(matriculaId)).ReturnsAsync((MatriculaDTO)null!);

        // Act
        var result = await _matriculaService.ObterPorIdAsync(matriculaId);

        // Assert
        Assert.Null(result);
        _matriculaServiceMock.Verify(s => s.ObterPorIdAsync(matriculaId), Times.Once);
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarMatriculas_QuandoExistirem()
    {
        // Arrange
        var matriculasDto = new List<MatriculaDTO>
        {
            CriarMatriculaPadrao(1),
            CriarMatriculaPadrao(2)
        };
        _matriculaServiceMock.Setup(s => s.ObterTodasAsync()).ReturnsAsync(matriculasDto);

        // Act
        var result = await _matriculaService.ObterTodasAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _matriculaServiceMock.Verify(s => s.ObterTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarListaVazia_QuandoNaoHouverMatriculas()
    {
        // Arrange
        _matriculaServiceMock.Setup(s => s.ObterTodasAsync()).ReturnsAsync(new List<MatriculaDTO>());

        // Act
        var result = await _matriculaService.ObterTodasAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _matriculaServiceMock.Verify(s => s.ObterTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarMatricula_QuandoDadosValidos()
    {
        // Arrange
        var matriculaDto = CriarMatriculaPadrao(0); // ID 0 para novo registro
        var matriculaCriada = CriarMatriculaPadrao(1); // Simula o retorno com ID 1
        _matriculaServiceMock.Setup(s => s.AdicionarAsync(It.IsAny<MatriculaDTO>())).ReturnsAsync(matriculaCriada);

        // Act
        var result = await _matriculaService.AdicionarAsync(matriculaDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _matriculaServiceMock.Verify(s => s.AdicionarAsync(It.IsAny<MatriculaDTO>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarMatricula_QuandoDadosValidos()
    {
        // Arrange
        var matriculaId = 1;
        var matriculaAtualizada = CriarMatriculaPadrao(matriculaId);
        matriculaAtualizada.Objetivo = "Definição Muscular";
        _matriculaServiceMock.Setup(s => s.AtualizarAsync(It.IsAny<MatriculaDTO>())).ReturnsAsync(matriculaAtualizada);

        // Act
        var result = await _matriculaService.AtualizarAsync(matriculaAtualizada);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Definição Muscular", result.Objetivo);
        _matriculaServiceMock.Verify(s => s.AtualizarAsync(It.IsAny<MatriculaDTO>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRetornarTrue_QuandoExistir()
    {
        // Arrange
        var matriculaId = 1;
        _matriculaServiceMock.Setup(s => s.RemoverAsync(matriculaId)).ReturnsAsync(true);

        // Act
        var result = await _matriculaService.RemoverAsync(matriculaId);

        // Assert
        Assert.True(result);
        _matriculaServiceMock.Verify(s => s.RemoverAsync(matriculaId), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRetornarFalse_QuandoNaoExistir()
    {
        // Arrange
        var matriculaId = 999;
        _matriculaServiceMock.Setup(s => s.RemoverAsync(matriculaId)).ReturnsAsync(false);

        // Act
        var result = await _matriculaService.RemoverAsync(matriculaId);

        // Assert
        Assert.False(result);
        _matriculaServiceMock.Verify(s => s.RemoverAsync(matriculaId), Times.Once);
    }

    [Fact]
    public async Task ObterVencendoEmDiasAsync_DeveLancarNotImplementedException()
    {
        // Arrange
        var dias = 30;
        _matriculaServiceMock.Setup(s => s.ObterVencendoEmDiasAsync(dias)).ThrowsAsync(new NotImplementedException());

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _matriculaService.ObterVencendoEmDiasAsync(dias));
        _matriculaServiceMock.Verify(s => s.ObterVencendoEmDiasAsync(dias), Times.Once);
    }
}