using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Machina.model;
using Machina.service;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Machina
{
    public partial class ScannerPage : ContentPage
    {
        private FaceDetectResult faceDetectResult=null;
        private bool processing;
        private SpeechOptions speechOptions = null;

        public ScannerPage(MediaFile file)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            var stream = file.GetStreamWithImageRotatedForExternalStorage();

            var imageStream = new MemoryStream();
            stream.CopyTo(imageStream);
            stream.Position = 0;
            imageStream.Position=0;

            faceImage.Source = ImageSource.FromStream(() =>
            {
                return stream;
              });

                startDetection(imageStream);
                 startAnimation();
           
        }
   private async Task   startDetection(Stream detection)
        {
            faceDetectResult = await CognitiveService.FaceDetect(detection);
            //  await Task.Delay(500);
            processing = false;        
        }

        private async Task DisplayResults()
        {
              

            if (faceDetectResult == null) return;

            //recuoération des infos
            genderLabel.Text = faceDetectResult.faceAttributes.gender.Substring(0, 1).ToUpper();
            ageLabel.Text = faceDetectResult.faceAttributes.age.ToString();
            infoLayout.IsVisible = true;
            continueButton.IsVisible = true;
            
        }

        private async Task startAnimation()
        {


            await laserScan();
            

            if (faceDetectResult == null )
            {
                await OnAnalysisError();
            }
            else
            {
                await DisplayResults();
                await ResultsSpeach();

            }
                
        }

        private async Task laserScan()
        {
            laserImage.Opacity = 0;
            await Task.Delay(500);
            await laserImage.FadeTo(1, 500);

            PlaySound("scan.wav");
            await laserImage.TranslateTo(0, 360, 1800);
            double y = 0;

            while (processing)
            {
                PlayCurrent();
                await laserImage.TranslateTo(0, y, 1000);
                y = (y == 0) ? 360 : 0;
            }
            laserImage.IsVisible = false;
            PlaySound("result.wav");

            statusLabel.Text = "Analyse terminée";
        }
        private async Task OnAnalysisError()
        {
            Speak("Humain non détécté");
            //  await Speak("لم يتم التعرف على انسان");

            await DisplayAlert("ERREUR ", "Pas de detection de visage", "OK");
            await Navigation.PopAsync();
        }

        void ContinueButtonClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }


        private  void PlaySound(string file)
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(GetStreamFromFile(file));
            player.Play();
        }



        private void PlayCurrent( )
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Stop();
            player.Play();
        }
       Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            /*
             *   
                var names = assembly.GetManifestResourceNames();
                Console.WriteLine(  "Liste des name spaces :"); 
                Console.WriteLine(String.Join(",",names));
             */
            var stream = assembly.GetManifestResourceStream("Machina.media." + filename);
            return stream;
        }

        private async Task InitSpeak()
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            //  locales.Language = "ar";
            //  locales.Country = "eg";

            var locale = locales.Where(o => o.Language.ToLower() == "fr").FirstOrDefault();

           speechOptions = new SpeechOptions
            {
                Volume = .75f,
                Pitch = 0.1f,
                Locale = locale
            };
        }



            private async Task Speak(string text    )
        {
            if (speechOptions == null)
            {
                await InitSpeak();
            }
            await TextToSpeech.SpeakAsync(text, speechOptions); 
        }

        private async Task ResultsSpeach()
        {

            if (faceDetectResult == null) return;

                await Speak("Humain détécté");
                      // await Speak("تم التعرف على انسان");

                        if (faceDetectResult.faceAttributes.gender.ToLower().Equals("male"))
                                {
                         
                                    await Speak("sexe masculin");
                                    // await Speak("جنس ذكر");
                                }
                                else if(faceDetectResult.faceAttributes.gender.ToLower().Equals("female"))
                                {


                                    await Speak("sexe féminin");
                                    //  await Speak("جنس انثى");
                                }
                       
                                await Speak("âge");
                                //  await Speak("السن");
                                await Speak(faceDetectResult.faceAttributes.age.ToString());
                              await Speak("an");
                                //  await Speak("سنة");

                     /*else
                                {
                                    await Speak("Humain non détécté");
                                    //  await Speak("لم يتم التعرف على انسان");


                                }*/
             
        }
    }
}