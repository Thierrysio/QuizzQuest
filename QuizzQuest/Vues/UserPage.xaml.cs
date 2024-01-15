using QuizzQuest.Modeles;

namespace QuizzQuest.Vues;

public partial class UserPage : ContentPage
{
    public UserPage()
    {
        InitializeComponent();
        BindingContext = Constantes.CurrentUser; // Définir l'utilisateur comme contexte de liaison
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AccueilPage()); // Navigate to AccueilPage

    }
}