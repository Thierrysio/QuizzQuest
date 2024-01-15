using QuizzQuest.Apis;
using QuizzQuest.Modeles;
using System;
using System.Timers;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace QuizzQuest.Vues
{
    public partial class QuizCollectifPage : ContentPage
    {
        private readonly GestionApi _apiServices = new GestionApi();
        private System.Timers.Timer _timer;
        private TimeSpan _timeRemaining;
        private Questionnaire _questionnaire;

        public QuizCollectifPage()
        {
            InitializeComponent();
            InitializePageAsync();
        }

        private async Task InitializePageAsync()
        {
            await GetNextQuestionnaire();

            if (_questionnaire != null)
            {
                SetupTimer();
            }
            else
            {
                StartQuestionnairePollingTimer();
            }
        }

        private void StartQuestionnairePollingTimer()
        {
            _timer = new System.Timers.Timer(10000); // 10 secondes d'intervalle
            _timer.Elapsed += async (sender, e) => await CheckForQuestionnaire();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void SetupTimer()
        {
            if (_questionnaire == null)
            {
                StartQuestionnairePollingTimer();
                return;
            }

            _timeRemaining = _questionnaire.DateQuestionnaire - DateTime.Now;

            if (_timeRemaining.TotalSeconds < -60)
            {
                await Navigation.PushAsync(new AccueilPage());
            }
            else if (_timeRemaining.TotalSeconds <= 0)
            {
                NavigateToQuiz();
            }
            else
            {
                // Utilisez MainThread pour mettre � jour l'interface utilisateur sur le thread principal.
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CountdownLabel.Text = FormatTimeSpan(_timeRemaining);
                });

                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }
        }
        private string FormatTimeSpan(TimeSpan span)
        {
            return $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timeRemaining -= TimeSpan.FromSeconds(1);

            // Utilisez MainThread.InvokeOnMainThreadAsync pour mettre � jour l'interface utilisateur sur le thread principal.
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (_timeRemaining.TotalSeconds <= 0)
                {
                    _timer.Stop();
                    NavigateToQuiz();
                }
                else
                {
                    CountdownLabel.Text = FormatTimeSpan(_timeRemaining);
                }
            });
        }
        private async Task CheckForQuestionnaire()
        {
            await GetNextQuestionnaire();
            
            if (_questionnaire != null)
            {
                _timer.Stop();
                SetupTimer();
            }
        }
        private async Task GetNextQuestionnaire()
        {
            Questionnaire.CollClasse.Clear();
            Question.CollClasse.Clear();
            Choix.CollClasse.Clear();
            Reponse.CollClasse.Clear();
            Score.CollClasse.Clear();
            Questionnaire result = await _apiServices.GetAllAsyncOne<Questionnaire>("api/mobile/nextQuestionnaire");
            
            if (result != null )
            {
                if (result.LaCategorie.Id == 2)
                {
                    return;
                }
                bool questionnaireFait = await _apiServices.HasUserAnsweredQuestionnaireAsync("api/mobile/GetQuestionnaireFait", Constantes.CurrentUser.Id,result.Id);
                if (questionnaireFait)
                {
                    Questionnaire.CollClasse.Clear();
                    Question.CollClasse.Clear();
                    Choix.CollClasse.Clear();
                    Reponse.CollClasse.Clear();
                    Score.CollClasse.Clear();
                }
                else
                {
                    _questionnaire = Questionnaire.CollClasse.FirstOrDefault();

                }
            }
        }
        private async void NavigateToQuiz()
        {
            try
            {
                await Navigation.PushAsync(new QuizPage(_questionnaire));
            }
            catch (Exception ex)
            {
                // Journaliser ou g�rer l'exception
                Debug.WriteLine($"Erreur lors de la navigation : {ex.Message}");
                    Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Arr�ter et disposer le timer si n�cessaire
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= OnTimerElapsed; // D�tacher l'�v�nement pour �viter les fuites de m�moire
                _timer.Dispose();
                _timer = null;
            }

            // Vous pouvez ajouter ici d'autres op�rations de nettoyage si n�cessaire
        }

        private async void ReturnToHomeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccueilPage()); // Navigate to AccueilPage
        }

        private async void OnBetClicked(object sender, EventArgs e)
        {
            if (_questionnaire == null) return;
            // Supposant que Constantes.CurrentUser.Id et _questionnaire sont accessibles dans ce contexte
            int userId = Constantes.CurrentUser.Id;
            int questionnaireId = _questionnaire.Id;

            int betAmount = Convert.ToInt32(BetAmountEntry.Text); // Convertissez le texte en nombre
            int userBalance = Convert.ToInt32(Constantes.CurrentUser.Cagnotte); // Remplacez par la fa�on dont vous acc�dez � la cagnotte de l'utilisateur

            if (betAmount > userBalance)
            {
                await DisplayAlert("Erreur", "La mise ne peut exc�der votre cagnotte.", "OK");
                BetAmountEntry.Text = "";
                return; // Arr�ter l'ex�cution si la mise est trop �lev�e
            }

            // Cr�ation d'un objet JSON � envoyer � l'API
            var betData = new
            {
                userId = userId,
                questionnaireId = questionnaireId,
                montant = BetAmountEntry.Text
            };
            if (await this.CheckBetAndAdjustUI()) return;
            // Utilisation de la classe GestionApi pour l'appel API
            string apiUrl = "api/mobile/miser/create";
            var response = await _apiServices.PostAsyncHttpResponse(betData, apiUrl);  // En supposant que PostAsync est une m�thode dans GestionApi

            if (response.IsSuccessStatusCode)
            {
                // Analyse de la r�ponse
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);

                string message = result.message;
                int miserId = result.miserId;

                MiserButton.IsVisible = false;
                
            }
            else
            {
                // G�rer l'erreur ici
                // ...
            }
        }
        private async Task<bool> CheckBetAndAdjustUI()
        {
            bool hasBet = false;
            var betData = new
            {
                userId = Constantes.CurrentUser.Id,
                questionnaireId = _questionnaire.Id
            };

         

            var response = await _apiServices.PostAsyncHttpResponse(betData, "api/mobile/verifier-mise");

             hasBet = response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() == "La mise existe d�j�";

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                MiserButton.IsVisible = !hasBet;
                BetAmountEntry.Text = hasBet ? "Vous avez d�j� mis�" : "";
            });
            return hasBet;
        }



    }
}
