using Microsoft.Maui.Controls;
using QuizzQuest.Modeles;
using System;
using System.Threading.Tasks;

namespace QuizzQuest.Vues
{
    public partial class AccueilPage : ContentPage
    {
        public AccueilPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            User.CollClasse.Clear();
            Questionnaire.CollClasse.Clear();
            Question.CollClasse.Clear();
            Choix.CollClasse.Clear();
            Reponse.CollClasse.Clear();
            Score.CollClasse.Clear();

            // Démarrage des animations de rotation en parallèle
            var rotateTask1 = RotateImage(ImageFrame1, 0, 3000);
            var rotateTask2 = RotateImage(ImageFrame2, 250, 2000);

            await Task.WhenAll(rotateTask1, rotateTask2);
        }

        private async void OnImageFrame1Tapped(object sender, EventArgs e)
        {
            StopAllAnimations();
            await Navigation.PushAsync(new QuizDuelPage());
        }

        private async void OnImageFrame2Tapped(object sender, EventArgs e)
        {
            StopAllAnimations();
            await Navigation.PushAsync(new QuizCollectifPage());
        }

        private async Task RotateImage(Frame imageFrame, uint delay, uint duration)
        {
            await Task.Delay((int)delay);

            while (true)
            {
                await imageFrame.RotateYTo(360, duration, Easing.Linear);
                imageFrame.RotationY = 0;
            }
        }

        private void StopAllAnimations()
        {
            // Centralisation de l'arrêt des animations pour éviter la répétition
            ImageFrame1.AbortAnimation("RotateTo");
            ImageFrame2.AbortAnimation("RotateTo");
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopAllAnimations(); // Arrêter les animations lors de la disparition de la page
        }
    }
}
