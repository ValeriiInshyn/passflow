using System.ComponentModel;
using System.Windows.Input;
using WPF_app.Helpers;

namespace WPF_app.Models
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _username;
        private string _password;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        private bool CanLogin(object parameter)
        {
            // Add your login validation logic here
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private void Login(object parameter)
        {
            // Add your authentication logic here
            if (IsValidUser(Username, Password))
            {
                // Navigate to the dashboard view
                var dashboardViewModel = new DashboardViewModel();
                var dashboardView = new DashboardView { DataContext = dashboardViewModel };
                dashboardView.Show();

                // Close the login view
                (parameter as System.Windows.Window)?.Close();
            }
            else
            {
                // Display an error message or handle authentication failure
            }
        }

        private bool IsValidUser(string username, string password)
        {
            // Add your actual user validation logic here
            return username == "admin" && password == "password";
        }
    }
}
