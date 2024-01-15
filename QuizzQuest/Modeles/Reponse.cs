
using QuizzQuest.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace QuizzQuest.Modeles
{
    public class Reponse
    {

        #region Attributs
        private readonly GestionApi _apiServices = new GestionApi();

        private TimeSpan _tempsDeReponse;
        private User _leUser;
        private Choix _leChoix;


        public static List<Reponse> CollClasse = new List<Reponse>();

        #endregion

        #region Constructeurs

        public Reponse(TimeSpan tempsDeReponse, User leUser, Choix leChoix)
        {
            Reponse.CollClasse.Add(this);
            _tempsDeReponse = tempsDeReponse;
            _leUser = leUser;
            _leChoix = leChoix;
        }

        #endregion

        #region Getters/Setters
        public TimeSpan TempsDeReponse { get => _tempsDeReponse; set => _tempsDeReponse = value; }
        public User LeUser { get => _leUser; set => _leUser = value; }
        public Choix LeChoix { get => _leChoix; set => _leChoix = value; }

        #endregion

        #region Methodes

        public async Task<int?> SaveReponse()
        {
            try
            {
                // Assurez-vous que _apiServices est initialisé
                if (_apiServices == null)
                {
                    throw new InvalidOperationException("_apiServices not initialized");
                }

                // Vous pouvez également ajouter d'autres validations ici selon les besoins

                int? resultat = await _apiServices.PostAsync<Reponse>(this, "api/mobile/postSaveReponse");
                return resultat;
            }
            catch (HttpRequestException httpEx)
            {
                // Gestion spécifique des exceptions liées aux requêtes HTTP
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Gestion générale des erreurs
                Console.WriteLine($"An error occurred while saving the response: {ex.Message}");
                return null;
            }
        }









    #endregion
}
}
