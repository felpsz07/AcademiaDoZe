using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using AcademiaDoZe.Presentation.AppMaui.Helpers;

namespace AcademiaDoZe.Presentation.AppMaui
{
    // Application conflita com o nome da nossa camada de aplicação
    // Incluir o namespace completo, Microsoft.Maui.Controls.Application, para evitar conflito
    // Direcionando para a classe Application do MAUI
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
            // Registrar a instância nos recursos da aplicação, em App.xaml.cs após InitializeComponent()

            Current.Resources["LocalizedStrings"] = LocalizationManager.Instance;
            // aplicar o tema salvo nas preferências
            AplicarTema();

            // assinar para receber mensagens de alteração de preferências
            // toda vez que o usuário alterar o tema, essa mensagem será enviada
            // e o tema será atualizado
            WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (r, m) =>

            {
                // m.Value contém o valor enviado na mensagem
                AplicarTema();
            });
            // assinar para receber mensagens de alteração de preferências da cultura - toda vez que o usuário alterar a cultura, essa mensagem será enviada
            WeakReferenceMessenger.Default.Register<CulturaPreferencesUpdatedMessage>(this, (r, m) =>
            {
                // Troca o idioma e notifica a UI imediatamente. m traz o valor enviado na mensagem, no caso a cultura selecionada

                LocalizationManager.Instance.SetCulture(m.Value);

                // abrir dashboard

                Shell.Current.GoToAsync("//dashboard");

            });
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        private void AplicarTema()
        {
            UserAppTheme = Preferences.Get("Tema", "system") switch
            {
                "light" => AppTheme.Light,
                "dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified,

            };
        }
    }
}