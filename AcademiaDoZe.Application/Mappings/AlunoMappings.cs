using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class AlunoMappings
{
    public static AlunoDTO ToDto(this Aluno aluno)
    {
        return new AlunoDTO
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone,
            Email = aluno.Email,
            Endereco = aluno.Endereco.ToDto(), // Mapeia o logradouro para DTO
            Numero = aluno.Numero,
            Complemento = aluno.Complemento,
            Senha = null, // a senha não deve ser exposta no DTO
           //Foto = aluno.Foto != null ? new ArquivoDTO { Conteudo = aluno.Foto.Conteudo } : null 
        };
    }
    public static Aluno ToEntity(this AlunoDTO alunoDto)
    {
        return Aluno.Criar(
        alunoDto.Id, alunoDto.Nome, alunoDto.Cpf, alunoDto.DataNascimento, alunoDto.Telefone, alunoDto.Email!,
        alunoDto.Endereco.ToEntity(), // Mapeia o logradouro do DTO para a entidade
        alunoDto.Numero, alunoDto.Complemento!, alunoDto.Senha!, (alunoDto.Foto?.Conteudo != null) ? Arquivo.Criar(alunoDto.Foto.Conteudo, ".jpg") : null!
        );
    }

    public static Aluno ToEntityMatricula(this AlunoDTO alunoDto)
    {
        return Aluno.Criar(
        alunoDto.Id, alunoDto.Nome, alunoDto.Cpf, alunoDto.DataNascimento, alunoDto.Telefone, alunoDto.Email!, alunoDto.Endereco.ToEntity(), alunoDto.Numero, alunoDto.Complemento!,
        "senhaFalsaSomenteParaAtenderDominio@123",
        (alunoDto.Foto?.Conteudo != null) ? Arquivo.Criar(alunoDto.Foto.Conteudo, ".jpg") : null!
        );
    }
    public static Aluno UpdateFromDto(this Aluno aluno, AlunoDTO alunoDto)
    {
        return Aluno.Criar(
        aluno.Id, alunoDto.Nome ?? aluno.Nome, aluno.Cpf, // CPF não pode ser alterado
        alunoDto.DataNascimento != default ? alunoDto.DataNascimento : aluno.DataNascimento,
        alunoDto.Telefone ?? aluno.Telefone, alunoDto.Email ?? aluno.Email, alunoDto.Endereco.ToEntity() ?? aluno.Endereco, alunoDto.Numero ?? aluno.Numero, alunoDto.Complemento ?? aluno.Complemento,
        alunoDto.Senha ?? aluno.Senha, (alunoDto.Foto?.Conteudo != null) ? Arquivo.Criar(alunoDto.Foto.Conteudo, ".jpg") : aluno.Foto // Atualiza a foto se fornecida
        );
    }
}
