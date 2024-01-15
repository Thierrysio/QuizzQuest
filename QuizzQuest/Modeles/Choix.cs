using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizzQuest.Modeles
{
    public class Choix
    {
        #region Attributs

        public static List<Choix> CollClasse = new List<Choix>();
        public static string[] Images = { "a1.png", "b1.png", "c1.png", "d1.png" };
        private static int _nextImageIndex = 0;

        private int _id;
        private string _texteDeChoix;
        private bool _bool;
        private string _image;
        #endregion

        #region Constructeurs

        public Choix(int id, string texteDeChoix, bool @bool)
        {
            _id = id;
            _texteDeChoix = texteDeChoix;
            _bool = @bool;
            _image = GetNextImage();
            Choix.CollClasse.Add(this);
        }

        #endregion

        #region Getters/Setters
        [JsonProperty("id")]
        public int Id { get => _id; set => _id = value; }
        [JsonProperty("texteDeChoix")]
        public string TexteDeChoix { get => _texteDeChoix; set => _texteDeChoix = value; }
        [JsonProperty("bool")]
        public bool Bool { get => _bool; set => _bool = value; }
        [JsonProperty("image")]
        public string Image { get => _image; set => _image = value; }
        public Color BackgroundColor
        {
            get
            {
                // Assuming you have a way to check if the choice is correct
                // and a property to identify the correct choice.
                if (IsCurrentUserLeGall() && _bool)
                    return Colors.Green;
                else
                    return Colors.Transparent; // Or any default color
            }
        }
        #endregion

        #region Methodes
        private bool IsCurrentUserLeGall()
        {
            return Constantes.CurrentUser.Nom.Equals("Le Gall", StringComparison.OrdinalIgnoreCase);
        }

        // Assurez-vous d'avoir une méthode pour notifier les changements de propriété
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private static string GetNextImage()
        {
            string image = Images[_nextImageIndex];
            _nextImageIndex = (_nextImageIndex + 1) % Images.Length;
            return image;
        }

        #endregion
    }
}
