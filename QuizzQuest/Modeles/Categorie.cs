using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuizzQuest.Modeles
{
    public class Categorie
    {
        #region Attributs

        public static List<Categorie> CollClasse = new List<Categorie>();
        private int _id;
        private string _nom;
        #endregion

        #region Constructeurs

        public Categorie(int id, string nom)
        {
            Categorie.CollClasse.Add(this);
            _id = id;
            _nom = nom;
        }

        #endregion

        #region Getters/Setters

        [JsonProperty("id")]
        public int Id { get => _id; set => _id = value; }
        [JsonProperty("nom")]
        public string Nom { get => _nom; set => _nom = value; }

        #endregion

        #region Methodes

        #endregion
    }
}
