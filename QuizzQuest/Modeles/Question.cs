using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizzQuest.Modeles
{
    public class Question
    {
        #region Attributs
        private int _id;
        private string _texteDeQuestion;
        private int _tempsAlloue;
        private List<Choix> _lesChoix;


        public static List<Question> CollClasse = new List<Question>();


        #endregion

        #region Constructeurs

        public Question()
        {
            Question.CollClasse.Add(this);
        }

        public Question(int id, string texteDeQuestion, int tempsAlloue)
        {
            _id = id;
            _texteDeQuestion = texteDeQuestion;
            _lesChoix = new List<Choix>();
            _tempsAlloue = tempsAlloue;
        }

        #endregion

        #region Getters/Setters
        [JsonProperty("id")]
        public int Id { get => _id; set => _id = value; }
        [JsonProperty("texteDeQuestion")]
        public string TexteDeQuestion { get => _texteDeQuestion; set => _texteDeQuestion = value; }
        [JsonProperty("lesChoix")]
        internal List<Choix> LesChoix { get => _lesChoix; set => _lesChoix = value; }
        [JsonProperty("tempsAlloue")]
        public int TempsAlloue { get => _tempsAlloue; set => _tempsAlloue = value; }

        #endregion

        #region Methodes

        #endregion
    }
}
