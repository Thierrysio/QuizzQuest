using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using QuizzQuest.Modeles;
using QuizzQuest.Apis;

namespace QuizzQuest.Vues
{
    public partial class QuizDuelPage : ContentPage
    {
        private readonly GestionApi _apiServices = new GestionApi();
        private System.Timers.Timer _timer;
        private TimeSpan _timeRemaining;
        private Questionnaire _questionnaire;
        private bool _isPollingActive;

        public QuizDuelPage()
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
                countdownLabel.Text = "Aucun duel n'est programmé actuellement";
                homeButton.IsVisible = true; // Show the button
                StartQuestionnairePollingTimer();
            }
        }

        private void InitializeRandomUsers(List<User> lesUsers)
        {
            if (lesUsers == null || lesUsers.Count < 2)
            {
                // Gestion d'erreur ou ajout d'utilisateurs de test
                return;
            }

            var random = new Random();
            var firstUser = lesUsers[random.Next(lesUsers.Count)];
            User secondUser;
            do
            {
                secondUser = lesUsers[random.Next(lesUsers.Count)];
            } while (secondUser.Id == firstUser.Id);

            user1Label.Text = $"{firstUser.Nom} {firstUser.Prenom}";
            user2Label.Text = $"{secondUser.Nom} {secondUser.Prenom}";
        }

        private void StartQuestionnairePollingTimer()
        {
            _isPollingActive = true;
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (_isPollingActive)
                {
                    CheckForQuestionnaire();
                }
                return _isPollingActive; // Le timer s'arrêtera si cette valeur est false
            });
        }

        private void SetupTimer(int interval, ElapsedEventHandler handler)
        {
            _timer?.Stop();
            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += handler;
            _timer.Start();
        }

        private void SetupTimer()
        {
            _timeRemaining = _questionnaire.DateQuestionnaire - DateTime.Now;

            if (_timeRemaining.TotalSeconds <= 0)
            {
                NavigateToQuiz();
            }
            else
            {
                UpdateCountdownLabel();
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }
        }

        private void UpdateCountdownLabel()
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                countdownLabel.Text = FormatTimeSpan(_timeRemaining);
            });
        }

private void OnTimerElapsed(object sender, ElapsedEventArgs e)
{
    _timeRemaining -= TimeSpan.FromSeconds(1);

    MainThread.InvokeOnMainThreadAsync(() =>
    {
        UpdateCountdownLabel();

        if (_timeRemaining.TotalSeconds <= 0)
        {
            _timer.Stop();
            NavigateToQuiz();
        }
    });
}

        private string FormatTimeSpan(TimeSpan span)
        {
            return $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
        }

        private async Task CheckForQuestionnaire()
        {
            await GetNextQuestionnaire();

            if (_questionnaire != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    SetupTimer();
                });
            }
        }

        private async Task GetNextQuestionnaire()
        {
            ClearAllCollections();
            var result = await _apiServices.GetAllAsyncOne<Questionnaire>("api/mobile/nextQuestionnaire");
            if (result == null || !result.LesUsers.Any(user => user.Id == Constantes.CurrentUser.Id))
            {
                return;
            }
            if(result.LaCategorie.Id == 1)
            {
                return;
            }

            bool questionnaireFait = await _apiServices.HasUserAnsweredQuestionnaireAsync("api/mobile/GetQuestionnaireFait", Constantes.CurrentUser.Id, result.Id);
            if (!questionnaireFait)
            {
                _questionnaire = result;
                InitializeRandomUsers(_questionnaire.LesUsers);
            }
        }

        private void ClearAllCollections()
        {
            User.CollClasse.Clear();
            Questionnaire.CollClasse.Clear();
            Question.CollClasse.Clear();
            Choix.CollClasse.Clear();
            Reponse.CollClasse.Clear();
            Score.CollClasse.Clear();
        }

        private async void NavigateToQuiz()
        {
            if (_timeRemaining.TotalSeconds <= 0)
            {
                try
                {
                    await Navigation.PushAsync(new QuizPage(_questionnaire));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de la navigation : {ex.Message}");
                    Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isPollingActive = false; // Cela arrêtera le Device.StartTimer
            DisposeTimer(); // Dispose le System.Timers.Timer
        }

        private void DisposeTimer()
        {
            if (_timer != null)
            {
                
                _timer.Stop();
                _timer.Elapsed -= OnTimerElapsed;
                _timer.Dispose();
                _timer = null;
            }
        }

        private async void homeButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccueilPage()); // Navigate to AccueilPage
        }
    }
}
