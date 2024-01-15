using QuizzQuest.Apis;
using QuizzQuest.Modeles;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using QuizzQuest.Utilitaires;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR.Client;

namespace QuizzQuest.Vues;

public partial class ScorePage : ContentPage, INotifyPropertyChanged
{
    private HubConnection _hubConnection;
    public event PropertyChangedEventHandler PropertyChanged;
    
    private readonly GestionApi _apiServices = new GestionApi();
    private Score _premierScore;
    private ObservableCollection<Score> _scores;
    public int NbQuestions { get; set; }

    public ScorePage(Questionnaire _questionnaire)
    {
        InitializeComponent();
        InitializeSignalR(_questionnaire).ConfigureAwait(false);
        Scores = new ObservableCollection<Score>();
        NbQuestions = _questionnaire.TotalQuestions;
        this.BindingContext = this;
        // Lancer l'appel à l'API
    }

    private async Task InitializeSignalR(Questionnaire _questionnaire)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://172.17.0.63:5000/randomHub") // Remplacer par l'URL de votre serveur
            .Build();

        _hubConnection.On<ObservableCollection<Score>>("ReceiveClassement", result =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
               await  GetClassement(_questionnaire.Id);
            });
        });

        // Gérer la connexion ici
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("StartSendingApiScore", _questionnaire.Id);
            }
            catch (Exception ex)
            {
                // Gérer l'exception
                // Par exemple, mettre à jour un label sur l'interface utilisateur
               
            }
        }
    }
    public Score PremierScore
    {
        get { return _premierScore; }
        set
        {
            if (_premierScore != value)
            {
                _premierScore = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Score> Scores
    {
        get { return _scores; }
        set
        {
            if (_scores != value)
            {
                _scores = value;
                OnPropertyChanged();
            }
        }
    }

    private async Task GetClassement(int id)
    {
        var result = await _apiServices.GetAllAsyncByID<Score>("api/mobile/GetScore", "Id", id);
        if (result == null || !result.Any())
        {
            PremierScore = null;
            OnPropertyChanged(nameof(PremierScore));
            return;
        }

        UpdateScores(result);
    }

    private void UpdateScores(IEnumerable<Score> newScores)
    {
        // Mise à jour du premier score
        PremierScore = newScores.First();
        OnPropertyChanged(nameof(PremierScore));

        // Mise à jour des scores suivants
        int index = 1;
        foreach (var newScore in newScores.Skip(1))
        {
            UpdateOrAddScore(newScore, index++);
        }

        // Supprimer les scores excédentaires
        while (Scores.Count > newScores.Count() - 1)
        {
            Scores.RemoveAt(Scores.Count - 1);
        }
    }

    private void UpdateOrAddScore(Score newScore, int index)
    {
        if (Scores.Count >= index)
        {
            var existingScore = Scores[index - 1];
            if (!ScoreEquals(existingScore, newScore))
            {
                Scores[index - 1] = newScore;
            }
        }
        else
        {
            Scores.Add(newScore);
        }
    }

    private bool ScoreEquals(Score score1, Score score2)
    {
        // Implementez une logique pour comparer deux objets Score
        // Par exemple, comparer les ID ou tout autre critère pertinent
        return score1.TotalReponses == score2.TotalReponses && score1.TotalTemps == score2.TotalTemps;
    }




    private async void OnReturnToHomePageClicked(object sender, EventArgs e)
    {
        // Assurez-vous d'utiliser le bon mécanisme de navigation selon votre contexte
        await Navigation.PushAsync(new AccueilPage());
    }
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Placez ici le code de nettoyage ou de gestion des ressources à libérer
    }
}
