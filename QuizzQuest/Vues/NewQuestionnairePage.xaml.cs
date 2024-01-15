using Newtonsoft.Json;
 
using QuizzQuest.Apis;
using QuizzQuest.Modeles;
using QuizzQuest.OpensAIS;
using System.Net.Http;
using System.Text;


namespace QuizzQuest.Vues;

public partial class NewQuestionnairePage : ContentPage
{
    private readonly OpenAIService _openAIService;
    private Questionnaire selectedQuestionnaire;
    private readonly GestionApi _apiServices = new GestionApi();
    public  List<Questionnaire> _questionnaires { get; set; } // Liste pour stocker les questionnaires

    private User firstCompetitor, secondCompetitor;
    public NewQuestionnairePage()
	{
		InitializeComponent();
        _openAIService = new OpenAIService();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadQuestionnaires(); // Chargement ou rafraîchissement des données
    }
    private async Task LoadQuestionnaires()
    {
        var questionnaires = await _apiServices.GetAllAsync<Questionnaire>("api/mobile/getAllQuestionnaires");
        _questionnaires = questionnaires.ToList();
        this.BindingContext = this;


    }
    private async void OnQuestionnaireSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        Questionnaire selectedTitle = (Questionnaire) picker.SelectedItem;
        var selectedQuestionnaire = _questionnaires.FirstOrDefault(q => q.Titre.Equals(selectedTitle.Titre));
        if(selectedQuestionnaire != null)
        {
            selectedQuestionnaire = await _apiServices.GetOneAsyncByID<Questionnaire>("questionnaire/getquestionnaire",selectedQuestionnaire.Id.ToString());

            jsonEditor.Text = JsonConvert.SerializeObject(selectedQuestionnaire);
        }

    }
    private async void OnCreateQuestionnaireClicked(object sender, EventArgs e)
    {

       

        var jsonQuestionnaire = AddDateHeure(jsonEditor.Text);


        
        // Valider le JSON ou utiliser JsonConvert pour désérialiser et re-sérialiser pour s'assurer du format
        // Appeler la méthode pour envoyer le JSON à l'API
        var result = await _apiServices.SendQuestionnaireAsync("questionnaire/new", jsonQuestionnaire);
        await DisplayAlert("Résultat", result, "OK");
    }
    private async void OnImageButtonClicked(object sender, EventArgs e)
    {
        // Remplacez 'AutrePage' par la page de destination réelle que vous souhaitez pousser
        await Navigation.PushAsync(new AccueilPage());
    }
    private async Task<(User, User)> ChooseCompetitors()
    {
        Random random = new Random();

        await _apiServices.GetAllAsyncByID<User>("api/mobile/GetAllUsers", "Id",Constantes.CurrentUser.Id);

        // Assurez-vous que la liste contient au moins deux utilisateurs distincts
        if (User.CollClasse.Count >= 2)
        {
            

            // Choisir le premier compétiteur
            firstCompetitor = User.CollClasse[random.Next(User.CollClasse.Count)];

            do
            {
                // Choisir le second compétiteur, différent du premier
                secondCompetitor = User.CollClasse[random.Next(User.CollClasse.Count)];
            }
            while (secondCompetitor == firstCompetitor);

            return (firstCompetitor, secondCompetitor);
        }
        else
        {
            // Gérer le cas où il n'y a pas assez d'utilisateurs
            throw new InvalidOperationException("Pas assez d'utilisateurs pour un duel");
        }
    }
    private async void OnQuizzDuelCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            quizzCollectifCheckBox.IsChecked = false;

            try
            {
                var (competitor1, competitor2) = await ChooseCompetitors();
                jsonEditor.Text = AjoutCategory(jsonEditor.Text, 2, "Duel");
                jsonEditor.Text = AddUsers(jsonEditor.Text, competitor1, competitor2);
                // Utiliser competitor1 et competitor2 pour votre logique de quizz
            }
            catch (InvalidOperationException ex)
            {
                // Gérer le cas d'erreur
                // Par exemple, afficher un message à l'utilisateur
            }
        }
    }
    private void OnQuizzCollectifCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) // Si Quizz Collectif est coché
        {
            quizzDuelCheckBox.IsChecked = false;
            jsonEditor.Text = AjoutCategory(jsonEditor.Text, 1, "Collectif");
        }
    }
    public string AjoutCategory(string json, int newCategoryId, string newCategoryName)
    {
        Categorie.CollClasse.Clear();
        // Deserialize the JSON string to Quiz object
        var quiz = JsonConvert.DeserializeObject<Questionnaire>(json);

        // Replace the category
        quiz.LaCategorie = new Categorie( newCategoryId, newCategoryName );

        // Serialize the Quiz object back to JSON
        return JsonConvert.SerializeObject(quiz);
    }
    public string AddDateHeure(string json)
    {
        var quiz = JsonConvert.DeserializeObject<Questionnaire>(json);
        DateTime selectedDate = datePicker.Date;
        TimeSpan selectedTime = timePicker.Time;

        // Combinaison de la date et de l'heure
        quiz.DateQuestionnaire = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedTime.Hours, selectedTime.Minutes, selectedTime.Seconds);


        return JsonConvert.SerializeObject(quiz);

    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        string userPrompt = jsonEditorBesoin.Text;
        string response = await _openAIService.AskQuestionAsync(userPrompt);
        jsonEditor.Text = response;
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (selectedQuestionnaire != null)
        {
            await Navigation.PushAsync(new ScorePage(selectedQuestionnaire));
        }
    }

    public string AddUsers(string json, User newUser1, User newUser2)
    {
        // Deserialize the JSON string to Quiz object
        var quiz = JsonConvert.DeserializeObject<Questionnaire>(json);

        // Initialize the user list if it's null
        if (quiz.LesUsers == null)
        {
            quiz.LesUsers = new List<User>();
        }

        // Add the new users
        quiz.LesUsers.Add(newUser1);
        quiz.LesUsers.Add(newUser2);

        // Serialize the Quiz object back to JSON
        return JsonConvert.SerializeObject(quiz);
    }
    
}