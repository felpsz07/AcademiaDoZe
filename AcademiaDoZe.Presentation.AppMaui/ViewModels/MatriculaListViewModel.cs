using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class MatriculaListViewModel : BaseViewModel
    {
        public ObservableCollection<string> FilterTypes { get; } = new() { "Id", "Aluno" };

        private readonly IMatriculaService _matriculaService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _selectedFilterType = "Aluno";
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }

        private ObservableCollection<MatriculaDTO> _matriculas = new();
        public ObservableCollection<MatriculaDTO> Matriculas
        {
            get => _matriculas;
            set => SetProperty(ref _matriculas, value);
        }

        private MatriculaDTO? _selectedMatricula;
        public MatriculaDTO? SelectedMatricula
        {
            get => _selectedMatricula;
            set => SetProperty(ref _selectedMatricula, value);
        }

        public MatriculaListViewModel(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
            Title = "Matrículas";
        }

        // comandos

        [RelayCommand]
        private async Task AddMatriculaAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("matricula");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao navegar para tela de cadastro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditMatriculaAsync(MatriculaDTO matricula)
        {
            try
            {
                if (matricula == null)
                    return;

                await Shell.Current.GoToAsync($"matricula?Id={matricula.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao navegar para tela de edição: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadMatriculasAsync();
        }

        [RelayCommand]
        private async Task LoadMatriculasAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Matriculas.Clear();
                    OnPropertyChanged(nameof(Matriculas));
                });

                var lista = await _matriculaService.ObterTodasAsync();

                if (lista != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var m in lista)
                            Matriculas.Add(m);

                        OnPropertyChanged(nameof(Matriculas));
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao carregar matrículas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task SearchMatriculasAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await MainThread.InvokeOnMainThreadAsync(() => Matriculas.Clear());

                IEnumerable<MatriculaDTO> resultados = Enumerable.Empty<MatriculaDTO>();

                var todas = await _matriculaService.ObterTodasAsync()
                            ?? Enumerable.Empty<MatriculaDTO>();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    resultados = todas;
                }
                else if (SelectedFilterType == "Id" && int.TryParse(SearchText, out int id))
                {
                    var m = await _matriculaService.ObterPorIdAsync(id);
                    if (m != null)
                        resultados = new[] { m };
                }
                else if (SelectedFilterType == "Aluno")
                {
                    var termo = SearchText.Trim().ToLowerInvariant();
                    resultados = todas.Where(m =>
                        m.AlunoMatricula != null &&
                        (
                            (!string.IsNullOrWhiteSpace(m.AlunoMatricula.Nome) &&
                             m.AlunoMatricula.Nome.ToLowerInvariant().Contains(termo))
                            ||
                            (!string.IsNullOrWhiteSpace(m.AlunoMatricula.Cpf) &&
                             m.AlunoMatricula.Cpf.Contains(termo))
                        ));
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var item in resultados)
                        Matriculas.Add(item);

                    OnPropertyChanged(nameof(Matriculas));
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao buscar matrículas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteMatriculaAsync(MatriculaDTO matricula)
        {
            if (matricula == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmar Exclusão",
                $"Deseja realmente excluir a matrícula do aluno {matricula.AlunoMatricula?.Nome}?",
                "Sim", "Não");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;

                bool success = await _matriculaService.RemoverAsync(matricula.Id);

                if (success)
                {
                    Matriculas.Remove(matricula);
                    await Shell.Current.DisplayAlert("Sucesso",
                        "Matrícula excluída com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro",
                        "Não foi possível excluir a matrícula.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao excluir matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}