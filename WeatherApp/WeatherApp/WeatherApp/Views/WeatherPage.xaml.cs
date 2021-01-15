using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Helper;
using WeatherApp.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherPage : ContentPage
    {
        public WeatherPage()
        {
            InitializeComponent();
            ScrollView scrollView = new ScrollView();
            scrollView.Scrolled += ScrollView_Scrolled;

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private string Lat;
        private string Lon;

        private string API_KEY = "e20c60c734fb3f34a6dea372c6944e88";
        CancellationTokenSource cts;

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            Console.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
        }

        async void bgImg_Clicked(object sender, System.EventArgs e)
        {
            try
            {

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                    {
                        Lat = location.Latitude.ToString();
                        Lon = location.Longitude.ToString();
                    }

                    var url = $"http://api.openweathermap.org/data/2.5/weather?lat={Lat}&lon={Lon}&appid={API_KEY}&units=metric";

                    var result = await ApiCaller.Get(url);

                    if (result.Successful)
                    {
                        try
                        {
                            var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                            descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                            iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                            cityTxt.Text = weatherInfo.name.ToUpper();
                            temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                            humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                            pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                            windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                            cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                            var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                            dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Weather Info", ex.Message, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Weather Info", "No weather information found", "OK");
                    }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }
        }
        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }
        private double width = 0;
        private double height = 0;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                //reconfigure layout
            }
        }
    }
}