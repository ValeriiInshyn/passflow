using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_app.Helpers;
using WPF_app.Nswag;
using WPF_app.Pages;
using WPFApp.Nswag;

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
            LoginCommand = new RelayCommand(LoginAsync, CanLogin);
        }

        private bool CanLogin(object parameter)
        {
            // Add your login validation logic here
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private async void LoginAsync(object parameter)
        {
            // Add your authentication logic here
            if (await IsValidUserAsync(Username, Password))
            {
                // Navigate to the dashboard view
                var tokenManagementViewModel = new TokenManagementViewModel();
                var tokenManagementView = new TokenManagementView() { DataContext = tokenManagementViewModel };
                tokenManagementView.Show();

                // Close the login view
                (parameter as System.Windows.Window)?.Close();
            }
            else
            {
                // Display an error message or handle authentication failure
            }
        }

        private async Task<bool> IsValidUserAsync(string username, string password)
        {
            // Add your actual user validation logic here
            try
            {
	            await ApiClient.Instance.LoginAsync(new LoginDto
	            {
		            UserName = username,
		            Password = password
	            });
	            return true;
            }
			catch (Exception e)
			{
				return false;
			}
        }
    }
}
