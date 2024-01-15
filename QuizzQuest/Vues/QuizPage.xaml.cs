using QuizzQuest.Modeles;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace QuizzQuest.Vues
{
    public partial class QuizPage : ContentPage, INotifyPropertyChanged
    {
        private double _tempsRestant;
        private int _currentIndex = 0;
        private Question _currentQuestion;
        private ObservableCollection<Choix> _lesChoix;
        private System.Timers.Timer _questionTimer;
        private Stopwatch _responseTimer = new Stopwatch();
        private Questionnaire _questionnaire;
        private bool _isQuestionActive;

        public event PropertyChangedEventHandler PropertyChanged;

        public Question CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                if (_currentQuestion != value)
                {
                    _currentQuestion = value;
                    LesChoix = new ObservableCollection<Choix>(_currentQuestion.LesChoix);
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Choix> LesChoix
        {
            get { return _lesChoix; }
            set
            {
                if (_lesChoix != value)
                {
                    _lesChoix = value;
                    OnPropertyChanged();
                }
            }
        }

        public QuizPage(Questionnaire questionnaire)
        {

                InitializeComponent();
                _questionnaire = questionnaire;
                this.BindingContext = this;

                InitializeResponseTracker();
                DisplayCurrentQuestion();
            
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void NavigateToScorePage()
        {
            await Navigation.PushAsync(new ScorePage(_questionnaire));
        }

private void DisplayCurrentQuestion()
{
    // Vérifiez si toutes les questions ont été traitées ou si le questionnaire est terminé.
    if (_currentIndex >= _questionnaire.LesQuestions.Count)
    {
        // Si toutes les questions ont été traitées, naviguez vers la page des scores.
        NavigateToScorePage();
        return;
    }

    // Récupérez la question actuelle à partir du questionnaire en utilisant _currentIndex.
    CurrentQuestion = _questionnaire.LesQuestions[_currentIndex];

    // Mettez à jour les choix disponibles pour la question actuelle.
    LesChoix = new ObservableCollection<Choix>(CurrentQuestion.LesChoix);

    // Réinitialisez et démarrez le chronomètre de réponse de l'utilisateur.
    _responseTimer.Restart();

    // Configurez et démarrez le timer pour le décompte de la question actuelle.
    SetupQuestionTimer();
}


        private void SetupQuestionTimer()
        {
            _questionTimer?.Stop();
            _questionTimer?.Dispose();

            TempsRestant = CurrentQuestion.TempsAlloue / 1000.0; // Assurez-vous que cette valeur est correcte

            _questionTimer = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };

            _questionTimer.Elapsed += (sender, e) =>
            {
                // Décrémentez le temps restant
                TempsRestant -= 1;

                // Vérifiez si le temps est écoulé
                if (TempsRestant <= 0)
                {
                    _questionTimer.Stop();
                    _questionTimer.Dispose();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // Passer à la question suivante
                        _currentIndex++;
                        if (_currentIndex < _questionnaire.LesQuestions.Count)
                        {
                            // Il y a encore des questions
                            DisplayCurrentQuestion();
                        }
                        else
                        {
                            // Plus de questions, naviguez vers la page des scores
                            NavigateToScorePage();
                        }
                    });
                }
            };

            _questionTimer.Start();
        }


        private void OnChoiceTapped(object sender, EventArgs e)
        {
            if (sender is Image image && image.BindingContext is Choix choix)
            {
                _questionTimer.Stop();
                _responseTimer.Stop();
                TimeSpan timeTaken = _responseTimer.Elapsed;
                _isQuestionActive = false;
                SetReponse(choix, timeTaken);
            }
        }

        private async void SetReponse(Choix selectedChoice, TimeSpan timeelapsed)
        {
            Reponse response = new Reponse(timeelapsed, Constantes.CurrentUser, selectedChoice);
            int? result = await response.SaveReponse();
            UpdateResponseIndicator(selectedChoice.Bool, _currentIndex);

            _currentIndex++;
            DisplayCurrentQuestion();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CleanUpTimers();
        }

        private void CleanUpTimers()
        {
            _questionTimer?.Stop();
            _questionTimer?.Dispose();
            _questionTimer = null;
            _responseTimer.Stop();
        }

        public double TempsRestant
        {
            get => _tempsRestant;
            set
            {
                if (_tempsRestant != value)
                {
                    _tempsRestant = value;
                    OnPropertyChanged(nameof(TempsRestant));
                    UpdateProgressBar();
                }
            }
        }

        private void UpdateProgressBar()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                double proportion = TempsRestant / (CurrentQuestion.TempsAlloue / 1000.0);
                circleTimer.Scale = proportion; // Cette ligne peut être incorrecte si circleTimer est une BoxView, car BoxView n'a pas de propriété Scale.

                // Changer la couleur en fonction du temps restant
                if (proportion <= 1 / 3.0) // Moins de 33% du temps restant
                {
                    circleTimer.Color = Colors.Red;
                }
                else if (proportion <= 2 / 3.0) // Moins de 66% du temps restant
                {
                    circleTimer.Color = Colors.Orange;
                }
                else
                {
                    circleTimer.Color = Colors.Green;
                }
            });
        }
        private void InitializeResponseTracker()
        {
            responseTracker.ColumnDefinitions.Clear();
            responseTracker.Children.Clear();

            for (int i = 0; i < _questionnaire.LesQuestions.Count; i++)
            {
                responseTracker.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var indicator = new BoxView
                {
                    WidthRequest = 20,
                    HeightRequest = 20,
                    CornerRadius = 10,
                    Color = Colors.Orange, // Couleur initiale du disque
                    Margin = new Thickness(2) // Espace entre les disques
                };

                responseTracker.Children.Add(indicator);
                Grid.SetColumn(indicator, i);
            }
        }

        private void UpdateResponseIndicator(bool isCorrect, int questionIndex)
        {
            if (questionIndex < responseTracker.Children.Count)
            {
                var indicator = (BoxView)responseTracker.Children[questionIndex];
                indicator.Color = isCorrect ? Colors.Green : Colors.Red; // Vert pour une bonne réponse, Rouge pour une mauvaise réponse
            }
        }
    }
}
