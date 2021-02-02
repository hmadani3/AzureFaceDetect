using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Machina
{
    public partial class MainPage : ContentPage
    {

        public ICommand AnimationViewClickCommand { get; set; }

        public MainPage()
        {
            InitializeComponent();

            AnimationViewClickCommand = new Command(() =>
            {
                StartButtonClickedAsync();
            });
            BindingContext = this;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        void StartButtonClicked(System.Object sender, System.EventArgs e)
        {

            StartButtonClickedAsync();

       

            NavigationPage.SetHasNavigationBar(this, false);

        }

        private async Task StartButtonClickedAsync()
        {

            var networkAccess = Connectivity.NetworkAccess;

            if(networkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Erreur", "Vous devez être connecté", "OK");
                return;
            }
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            { 
                Directory = "Sample", 
                Name = "test.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small

            });

            if (file == null)
            {
            //  await DisplayAlert("Ficher image non retrouvé", "Not Found", "OK");

                 return;
            }             

         
            await Navigation.PushAsync(new ScannerPage(file),false);
        
        }
    }
}
