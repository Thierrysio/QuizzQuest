using Newtonsoft.Json;
using System;

namespace QuizzQuest.Modeles
{
    public class Score
    {
        #region Attributs
        private int _userId;
        private string _nom;
        private int _totalReponses;
        private int _totalTemps;
        private int _index;
 public static List<Score> CollClasse = new List<Score>();
        #endregion

        #region Constructeurs
        public Score()
        {
            // Initialisation ou logique de construction ici si nécessaire
        }

        public Score(int userId, string nom, int totalReponses, int totalTemps)
        {
            _userId = userId;
            _nom = nom;
            _totalReponses = totalReponses;
            _totalTemps = totalTemps;
            Score.CollClasse.Add(this); 
        }
        #endregion

        #region Getters/Setters
        [JsonProperty("userId")]
        public int UserId { get => _userId; set => _userId = value; }

        [JsonProperty("nom")]
        public string Nom { get => _nom; set => _nom = value; }

        [JsonProperty("totalReponses")]
        public int TotalReponses { get => _totalReponses; set => _totalReponses = value; }

        [JsonProperty("totalTemps")]
        public int TotalTemps { get => _totalTemps; set => _totalTemps = value; }
        public int Index { get => _index; set => _index = value; }
        #endregion

        #region Methodes
        // Méthodes supplémentaires ici si nécessaire
        #endregion
    }
}

