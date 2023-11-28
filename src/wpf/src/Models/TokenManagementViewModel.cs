using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using WPF_app.Dtos;
using WPF_app.Helpers;
using WPF_app.Nswag;

namespace WPF_app.Models
{
    public class TokenManagementViewModel : INotifyPropertyChanged
    {
        private TokenDto _selectedToken;

        public TokenDto SelectedToken
        {
            get { return _selectedToken; }
            set
            {
                _selectedToken = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedToken)));
            }
        }

        private const string ApiBaseUrl = "https://localhost:7267/";

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TokenDto> _tokens;

        public ObservableCollection<TokenDto> Tokens
        {
            get { return _tokens; }
            set
            {
                _tokens = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tokens)));
            }
        }

        public ICommand AddTokenCommand { get; private set; }
        public ICommand EditTokenCommand { get; private set; }
        public ICommand DeleteTokenCommand { get; private set; }
        public TokenManagementViewModel()
        {
            // Initialize commands
            AddTokenCommand = new RelayCommand(async _ => await AddTokenAsync());
            //EditTokenCommand = new RelayCommand(EditToken, CanEditOrDeleteToken);
            DeleteTokenCommand = new RelayCommand(async parameter => await DeleteTokenAsync(parameter), CanEditOrDeleteToken);

            // Load tokens from your API
            LoadTokensFromApi();
        }

     

        private async Task LoadTokensFromApi()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(ApiBaseUrl + "tokens");
                    Tokens = JsonConvert.DeserializeObject<ObservableCollection<TokenDto>>(response);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log, display an error message)
                Console.WriteLine($"Error loading tokens: {ex.Message}");
            }
        }

        private async Task AddTokenAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var newToken = new TokenDto() { /* Initialize properties */ };
                    var json = JsonConvert.SerializeObject(newToken);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(ApiBaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Token added successfully
                        var addedToken = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());
                        Tokens.Add(addedToken);
                    }
                    else
                    {
                        // Handle unsuccessful response
                        Console.WriteLine($"Error adding token. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log, display an error message)
                Console.WriteLine($"Error adding token: {ex.Message}");
            }
        }

        private async Task DeleteTokenAsync(object parameter)
        {
            try
            {
                var selectedToken = parameter as TokenDto;
                if (selectedToken == null)
                    return;

                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{ApiBaseUrl}/{selectedToken.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Token deleted successfully
                        Tokens.Remove(selectedToken);
                    }
                    else
                    {
                        // Handle unsuccessful response
                        Console.WriteLine($"Error deleting token. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log, display an error message)
                Console.WriteLine($"Error deleting token: {ex.Message}");
            }
        }

        private bool CanEditOrDeleteToken(object parameter)
        {
            // Implement logic to determine if editing or deleting is allowed
            // Example: Allow if a token is selected
            return parameter is TokenDto;
        }
    }
}
