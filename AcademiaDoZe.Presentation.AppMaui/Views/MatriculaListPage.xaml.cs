using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;


namespace AcademiaDoZe.Presentation.AppMaui.Views;


public partial class MatriculaListPage : ContentPage
{
    public MatriculaListPage(MatriculaListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MatriculaListViewModel vm)
        {
            await vm.LoadMatriculasCommand.ExecuteAsync(null);
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn &&
                btn.BindingContext is MatriculaDTO matricula &&
                BindingContext is MatriculaListViewModel vm)
            {
                await vm.EditMatriculaCommand.ExecuteAsync(matricula);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao editar matrícula: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn &&
                btn.BindingContext is MatriculaDTO matricula &&
                BindingContext is MatriculaListViewModel vm)
            {
                await vm.DeleteMatriculaCommand.ExecuteAsync(matricula);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao excluir matrícula: {ex.Message}", "OK");
        }
    }

    private CancellationTokenSource? _searchCts;

    private async void OnSearchDebounceTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            await Task.Delay(300, token);
            if (token.IsCancellationRequested)
                return;

            if (BindingContext is MatriculaListViewModel vm)
            {
                await vm.SearchMatriculasCommand.ExecuteAsync(null);
            }
        }
        catch (TaskCanceledException) { }
    }
}