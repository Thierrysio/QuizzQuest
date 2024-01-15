using Newtonsoft.Json;
using QuizzQuest.Apis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizzQuest.Modeles
{
    public class Questionnaire
    {
        #region Attributs

        public static List<Questionnaire> CollClasse = new List<Questionnaire>();
        private int _id;
        private string _titre;
        private string _description;
        private int _totalQuestions;
        private DateTime _dateQuestionnaire;
        private List<Question> _lesQuestions;
        private List<User> _lesUsers;
        private Categorie _laCategorie;
        private bool _valider_Mise;

        #endregion

        #region Constructeurs



        public Questionnaire(int id, string titre, string description, int totalQuestions, DateTime dateQuestionnaire, Categorie laCategorie)
        {
            _id = id;
            _titre = titre;
            _description = description;
            _totalQuestions = totalQuestions;
            _lesQuestions = new List<Question>();
            _lesUsers = new List<User>();
            _dateQuestionnaire = dateQuestionnaire;
            Questionnaire.CollClasse.Add(this);
            _laCategorie = laCategorie;
            _valider_Mise = false;
        }


        #endregion

        #region Getters/Setters
        [JsonProperty("id")]
        public int Id { get => _id; set => _id = value; }
        [JsonProperty("titre")]
        public string Titre { get => _titre; set => _titre = value; }
        [JsonProperty("description")]
        public string Description { get => _description; set => _description = value; }
        [JsonProperty("totalQuestions")]
        public int TotalQuestions { get => _totalQuestions; set => _totalQuestions = value; }
        [JsonProperty("dateQuestionnaire")]
        public DateTime DateQuestionnaire { get => _dateQuestionnaire; set => _dateQuestionnaire = value; }
        [JsonProperty("lesUsers")]
        public List<User> LesUsers { get => _lesUsers; set => _lesUsers = value; }
        [JsonProperty("valider_mise")]
        public bool Valider_Mise { get => _valider_Mise; set => _valider_Mise = value; }
        [JsonProperty("lesQuestions")]
        internal List<Question> LesQuestions { get => _lesQuestions; set => _lesQuestions = value; }
        [JsonProperty("laCategorie")]
        internal Categorie LaCategorie { get => _laCategorie; set => _laCategorie = value; }

        #endregion

        #region Methodes

        #endregion
    }
}
