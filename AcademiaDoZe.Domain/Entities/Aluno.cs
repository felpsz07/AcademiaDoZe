//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa //Classe de aluno
{
    private Aluno(
        int id,
        string nome, 
        string cpf, 
        DateOnly dataNascimento, 
        string telefone, 
        string email, 
        Logradouro endereco, 
        string numero, 
        string complemento, 
        string senha,
        Arquivo foto) 
    : base(id, nome, cpf, dataNascimento, telefone, email, endereco, numero, complemento, senha, foto)
    {
        
    }
    //Método de fábrica de Aluno
    public static Aluno Criar(
                  int id, 
                  string nome,
                  string cpf,
                  DateOnly dataNascimento,
                  string telefone,
                  string email,
                  Logradouro endereco,
                  string numero,
                  string complemento,
                  string senha,
                  Arquivo foto)
        //Validações
    {
        if (TextoNormalizadoService.TextoVazioOuNulo(nome)) throw new DomainException("NOME_OBRIGATORIO");
        nome = TextoNormalizadoService.LimparEspacos(nome);
        
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
        //if (foto == null) throw new DomainException("FOTO_OBRIGATORIO");
        if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");

        if (TextoNormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
        numero = TextoNormalizadoService.LimparEspacos(numero);
        complemento = TextoNormalizadoService.LimparEspacos(complemento);

        return new Aluno(id, nome, cpf, dataNascimento, telefone, email, endereco, numero, complemento, senha, foto); 
    }
}
