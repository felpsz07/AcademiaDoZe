using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(MatriculaId), "Id")]
    public partial class MatriculaViewModel : BaseViewModel
    {
        private readonly IMatriculaService _matriculaService;
        private readonly IAlunoService _alunoService;

        public List<AlunoDTO> Alunos { get; set; } = new();
        public IEnumerable<EAppMatriculaPlano> Planos =>
            Enum.GetValues(typeof(EAppMatriculaPlano)).Cast<EAppMatriculaPlano>();


        private string _cpfDigitado;
        public string CpfDigitado
        {
            get => _cpfDigitado;
            set
            {
                _cpfDigitado = value;
                OnPropertyChanged();
            }
        }

        private AlunoDTO _aluno;
        public AlunoDTO Aluno
        {
            get => _aluno;
            set
            {
                _aluno = value;
                Matricula.AlunoMatricula = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(AlunoNomeCpf));
            }
        }

        private bool _alunoEncontrado;
        public bool AlunoEncontrado
        {
            get => _alunoEncontrado;
            set
            {
                _alunoEncontrado = value;
                OnPropertyChanged();
            }
        }

        public string AlunoNomeCpf =>
            Aluno == null ? "" : $"{Aluno.Nome} - {Aluno.Cpf}";

     
        public IRelayCommand SelecionarAlunoCommand { get; }
        public IRelayCommand BuscarAlunoCommand { get; }


        public MatriculaDTO Matricula { get; set; } =
            new()
            {
                AlunoMatricula = new AlunoDTO
                {
                    Id = 0,
                    Nome = "",
                    Cpf = "",
                    DataNascimento = DateOnly.FromDateTime(DateTime.Today),
                    Telefone = "",
                    Endereco = new LogradouroDTO
                    {
                        Id = 0,
                        Cep = "",
                        Nome = "",
                        Bairro = "",
                        Cidade = "",
                        Estado = "",
                        Pais = ""
                    },
                    Numero = ""
                },
                Plano = EAppMatriculaPlano.Mensal,
                DataInicio = DateOnly.FromDateTime(DateTime.Today),
                DataFim = DateOnly.FromDateTime(DateTime.Today).AddMonths(1),
                Objetivo = "",
                RestricoesMedicas = EAppMatriculaRestricoes.None
            };

        public int MatriculaId { get; set; }
        public bool IsEditMode { get; set; }

        public MatriculaViewModel(IMatriculaService matriculaService, IAlunoService alunoService)
        {
            _matriculaService = matriculaService;
            _alunoService = alunoService;

            Title = "Cadastro de Matrícula";

            SelecionarAlunoCommand = new RelayCommand(async () => await SelecionarAluno());
            BuscarAlunoCommand = new RelayCommand(async () => await BuscarAluno());

           
           
        }


        public async Task InitializeAsync()
        {
            Alunos = (await _alunoService.ObterTodosAsync()).ToList();

            if (MatriculaId > 0)
            {
                IsEditMode = true;

                Matricula = await _matriculaService.ObterPorIdAsync(MatriculaId);
                Aluno = Matricula.AlunoMatricula;

                Title = "Editar Matrícula";

                OnPropertyChanged(nameof(Diabetes));
                OnPropertyChanged(nameof(PressaoAlta));
                OnPropertyChanged(nameof(Alergias));
                OnPropertyChanged(nameof(Labirintite));
                OnPropertyChanged(nameof(ProblemasRespiratorios));
                OnPropertyChanged(nameof(RemedioContinuo));
            }
        }

       
        public bool Diabetes
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.Diabetes);
            set => Toggle(EAppMatriculaRestricoes.Diabetes, value);
        }

        public bool PressaoAlta
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.PressaoAlta);
            set => Toggle(EAppMatriculaRestricoes.PressaoAlta, value);
        }

        public bool Alergias
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.Alergias);
            set => Toggle(EAppMatriculaRestricoes.Alergias, value);
        }

        public bool Labirintite
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.Labirintite);
            set => Toggle(EAppMatriculaRestricoes.Labirintite, value);
        }

        public bool ProblemasRespiratorios
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.ProblemasRespiratorios);
            set => Toggle(EAppMatriculaRestricoes.ProblemasRespiratorios, value);
        }

        public bool RemedioContinuo
        {
            get => Matricula.RestricoesMedicas.HasFlag(EAppMatriculaRestricoes.RemedioContinuo);
            set => Toggle(EAppMatriculaRestricoes.RemedioContinuo, value);
        }

        private void Toggle(EAppMatriculaRestricoes flag, bool active)
        {
            if (active)
                Matricula.RestricoesMedicas |= flag;
            else
                Matricula.RestricoesMedicas &= ~flag;

            OnPropertyChanged(nameof(Matricula));
        }

  
        public EAppMatriculaPlano Plano
        {
            get => Matricula.Plano;
            set
            {
                Matricula.Plano = value;
                AtualizarDataFim();
            }
        }

        public DateOnly DataInicio
        {
            get => Matricula.DataInicio;
            set
            {
                Matricula.DataInicio = value;
                AtualizarDataFim();
            }
        }

        public DateOnly DataFim
        {
            get => Matricula.DataFim;
            set => Matricula.DataFim = value;
        }

        private void AtualizarDataFim()
        {
            int meses = Plano switch
            {
                EAppMatriculaPlano.Mensal => 1,
                EAppMatriculaPlano.Trimestral => 3,
                EAppMatriculaPlano.Semestral => 6,
                EAppMatriculaPlano.Anual => 12,
                _ => 1
            };

            DataFim = DataInicio.AddMonths(meses);
            OnPropertyChanged(nameof(DataFim));
        }

        
        private async Task SelecionarAluno()
        {
            await Shell.Current.DisplayAlert("Info", "Seleção por lista ainda não implementada.", "OK");
        }


        private async Task BuscarAluno()
        {
            if (string.IsNullOrWhiteSpace(CpfDigitado))
            {
                await Shell.Current.DisplayAlert("Erro", "Digite um CPF.", "OK");
                return;
            }

            var cpfLimpo = CpfDigitado.Replace(".", "").Replace("-", "");

            var aluno = Alunos.FirstOrDefault(a =>
                a.Cpf.Replace(".", "").Replace("-", "") == cpfLimpo);

            if (aluno == null)
            {
                AlunoEncontrado = false;
                await Shell.Current.DisplayAlert("Não encontrado", "Aluno não localizado.", "OK");
                return;
            }

            Aluno = aluno;
            AlunoEncontrado = true;

            OnPropertyChanged(nameof(Aluno));
        }

        [RelayCommand]
        public async Task SelecionarLaudoAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione o Laudo Médico (PDF)",
                    FileTypes = FilePickerFileType.Pdf
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);

                    Matricula.LaudoMedico = new ArquivoDTO
                    {
                        Nome = result.FileName,
                        Conteudo = ms.ToArray()
                    };

                    OnPropertyChanged(nameof(Matricula));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Falha ao selecionar arquivo: {ex.Message}", "OK");
            }
        }


        [RelayCommand]
        public async Task SaveAsync()
        {
            if (!Validar(Matricula)) return;

            if (IsEditMode)
                await _matriculaService.AtualizarAsync(Matricula);
            else
                await _matriculaService.AdicionarAsync(Matricula);

            await Shell.Current.GoToAsync("..");
        }

        private bool Validar(MatriculaDTO m)
        {
            if (m.AlunoMatricula == null || m.AlunoMatricula.Id <= 0)
            {
                Shell.Current.DisplayAlert("Erro", "Selecione um aluno.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(m.Objetivo))
            {
                Shell.Current.DisplayAlert("Erro", "Objetivo obrigatório.", "OK");
                return false;
            }

            if (m.DataFim <= m.DataInicio)
            {
                Shell.Current.DisplayAlert("Erro", "Data final inválida.", "OK");
                return false;
            }

            int idade = DateTime.Today.Year - m.AlunoMatricula.DataNascimento.Year;

            if (m.AlunoMatricula.DataNascimento >
                DateOnly.FromDateTime(DateTime.Today.AddYears(-idade)))
                idade--;

            if ((idade >= 12 && idade <= 16) && m.LaudoMedico == null)
            {
                Shell.Current.DisplayAlert("Erro",
                    "Laudo é obrigatório para menores de 16.",
                    "OK");
                return false;
            }

            if (m.RestricoesMedicas != EAppMatriculaRestricoes.None &&
                m.LaudoMedico == null)
            {
                Shell.Current.DisplayAlert("Erro",
                    "A matrícula possui restrições. Anexe o laudo.",
                    "OK");
                return false;
            }

            return true;
        }
    }
}