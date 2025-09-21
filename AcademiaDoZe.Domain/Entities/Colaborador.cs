//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa // Classe de colaborador
{
    public DateOnly DataAdmissao { get; }
    public EColaboradorTipo Tipo { get; }
    public EColaboradorVinculo Vinculo { get; }

    private Colaborador(int id,
                       string nomeCompleto,
                       string cpf,
                       DateOnly dataNascimento,
                       string telefone,
                       string email,
                       Logradouro endereco,
                       string numero,
                       string complemento,
                       string senha,
                       Arquivo foto,
                       DateOnly dataAdmissao,
                       EColaboradorTipo tipo,
                       EColaboradorVinculo vinculo)

 : base(id, nomeCompleto, cpf, dataNascimento, telefone, email, endereco, numero, complemento, senha, foto)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }
    //Método de fábrica
    public static Colaborador Criar(int id, string nomeCompleto, string cpf, DateOnly dataNascimento, string telefone, 
        string email, Logradouro endereco, string numero, string complemento, string senha,
        Arquivo foto, DateOnly dataAdmissao, EColaboradorTipo tipo, EColaboradorVinculo vinculo)
        //Validações
    {
        if (TextoNormalizadoService.TextoVazioOuNulo(nomeCompleto)) throw new DomainException("NOME_OBRIGATORIO");
        nomeCompleto = TextoNormalizadoService.LimparEspacos(nomeCompleto);

        if (TextoNormalizadoService.TextoVazioOuNulo(cpf)) throw new DomainException("CPF_OBRIGATORIO");
        cpf = TextoNormalizadoService.LimparEDigitos(cpf);

        if (cpf.Length != 11) throw new DomainException("CPF_DIGITOS");
        
        if (dataNascimento == default) throw new DomainException("DATA_NASCIMENTO_OBRIGATORIO");
        
        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-12))) throw new DomainException("DATA_NASCIMENTO_MINIMA_INVALIDA");
        
        if (TextoNormalizadoService.TextoVazioOuNulo(telefone)) throw new DomainException("TELEFONE_OBRIGATORIO");
        telefone = TextoNormalizadoService.LimparEDigitos(telefone);
        
        if (telefone.Length != 11) throw new DomainException("TELEFONE_DIGITOS");
        email = TextoNormalizadoService.LimparEspacos(email);
        
        if (TextoNormalizadoService.ValidarFormatoEmail(email)) throw new DomainException("EMAIL_FORMATO");
        
        if (TextoNormalizadoService.TextoVazioOuNulo(senha)) throw new DomainException("SENHA_OBRIGATORIO");
        senha = TextoNormalizadoService.LimparEspacos(senha);
        
        if (TextoNormalizadoService.ValidarFormatoSenha(senha)) throw new DomainException("SENHA_FORMATO");
        
        if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");
        
        if (TextoNormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
        numero = TextoNormalizadoService.LimparEspacos(numero);
        complemento = TextoNormalizadoService.LimparEspacos(complemento);
        
        if (dataAdmissao == default) throw new DomainException("DATA_ADMISSAO_OBRIGATORIO");
        
        if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today)) throw new DomainException("DATA_ADMISSAO_MAIOR_ATUAL");
        
        if (!Enum.IsDefined(tipo)) throw new DomainException("TIPO_COLABORADOR_INVALIDO");
        
        if (!Enum.IsDefined(vinculo)) throw new DomainException("VINCULO_COLABORADOR_INVALIDO");
        
//        if (tipo == EColaboradorTipo.Administrador && vinculo == EColaboradorVinculo.CLT) throw new DomainException("ADMINISTRADOR_CLT_INVALIDO");

        return new Colaborador(id, nomeCompleto, cpf, dataNascimento, telefone, email, endereco, numero, complemento, senha, foto, dataAdmissao, tipo, vinculo);

    }
}    

