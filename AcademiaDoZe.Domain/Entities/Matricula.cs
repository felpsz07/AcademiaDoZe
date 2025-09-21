//Felipe Bueno de Oliveira
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System.Numerics;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity//Classe de Matrícula
{
    public Aluno AlunoMatricula { get; private set; }
    public EMatriculaPlano Plano {  get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly DataFim { get; private set; }
    public string Objetivo { get; private set; }
    public EMatriculaRestricoes RestricoesMedicas { get; private set; }
    public string ObservacoesRestricoes { get; private set; }
    public Arquivo LaudoMedico { get; private set; }

    private Matricula(int id,
                      Aluno aluno,
                      EMatriculaPlano planoMatricula,
                      DateOnly dataInicio,
                      DateOnly dataFinal,
                      string objetivo,
                      EMatriculaRestricoes restricoesAluno,
                      string observacaoRestricao,
                      Arquivo laudoMedico)
    {
        Id = id;
        AlunoMatricula = aluno;
        Plano = planoMatricula;
        DataInicio = dataInicio;
        DataFim = dataFinal;
        Objetivo = objetivo;
        RestricoesMedicas = restricoesAluno;
        ObservacoesRestricoes = observacaoRestricao;
        LaudoMedico = laudoMedico;
    }
    //Método de fábrica de Matrícula
    public static Matricula Criar(int id,
                                  Aluno alunoMatricula,
                                  EMatriculaPlano planoMatricula,
                                  DateOnly dataInicio,
                                  DateOnly dataFinal,
                                  string objetivo,
                                  EMatriculaRestricoes restricoesMedicas,
                                  string observacoesRestricoes,
                                  Arquivo laudoMedico)
        //Validações
    {
        if (alunoMatricula == null) throw new DomainException("ALUNO_INVALIDO");
        
        if (alunoMatricula.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)) && laudoMedico == null) throw new DomainException("MENOR16_LAUDO_OBRIGATORIO");
        
//        if (!Enum.IsDefined(planoMatricula)) throw new DomainException("PLANO_INVALIDO");
        
        if (dataInicio == default) throw new DomainException("DATA_INICIO_OBRIGATORIO");
        
        if (TextoNormalizadoService.TextoVazioOuNulo(objetivo)) throw new DomainException("OBJETIVO_OBRIGATORIO");
        objetivo = TextoNormalizadoService.LimparEspacos(objetivo);
        
        if (restricoesMedicas != EMatriculaRestricoes.None && laudoMedico == null) throw new DomainException("RESTRICOES_LAUDO_OBRIGATORIO");
        observacoesRestricoes = TextoNormalizadoService.LimparEspacos(observacoesRestricoes);
        

        return new Matricula(id, alunoMatricula, planoMatricula, dataInicio, dataFinal, objetivo, restricoesMedicas,
            observacoesRestricoes, laudoMedico);
    }
}
