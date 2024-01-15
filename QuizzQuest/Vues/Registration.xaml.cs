using QuizzQuest.Apis;
using QuizzQuest.Modeles;
using Microsoft.Maui.Controls;
using System;

namespace QuizzQuest.Vues
{
    public partial class Registration : ContentPage
    {
        private readonly GestionApi _apiServices = new GestionApi();

        public Registration()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var nom = NameEntry.Text;
            var prenom = PrenomEntry.Text;

            User U1 = new User(1, nom, prenom);
            bool res = await U1.GetUserRegistration();

            if (res)
            {
                // La requ�te API est la m�me dans les deux cas, donc on la d�place hors du if
                // var result = await _apiServices.GetAllAsyncByID<User>("api/mobile/GetAllUsers", "Id", U1.Id);

                if (U1.Nom == "le gall" && U1.Prenom == "thierry")
                {
                    await Navigation.PushAsync(new NewQuestionnairePage());
                }
                else
                {
                    // Redirection vers la page Accueil
                    await Navigation.PushAsync(new AccueilPage());
                }
            }
            else
            {
                // G�rer le cas o� GetUserRegistration retourne false (si n�cessaire)
                // Exemple : Afficher un message d'erreur
            }
        }


        private async void UserButton_Clicked(object sender, EventArgs e)
        {
            var nom = NameEntry.Text;
            var prenom = PrenomEntry.Text;

            User U1 = new User(1, nom, prenom);
            bool res = await U1.GetUserRegistration();

            if (res)
            {
                // La requ�te API est la m�me dans les deux cas, donc on la d�place hors du if
                // var result = await _apiServices.GetAllAsyncByID<User>("api/mobile/GetAllUsers", "Id", U1.Id);


                // Redirection vers la page Accueil
                await Navigation.PushAsync(new UserPage());

            }
            else
            {
                // G�rer le cas o� GetUserRegistration retourne false (si n�cessaire)
                // Exemple : Afficher un message d'erreur
            }
        }
    }
    }

