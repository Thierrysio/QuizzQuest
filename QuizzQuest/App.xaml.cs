using QuizzQuest.Vues;

namespace QuizzQuest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage( new Registration());
        }
    }
}