using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Machina.model;

namespace Machina.service
{
    public class CognitiveService
    {




        const string ENDPOINT = "https://facecontrol.cognitiveservices.azure.com/face/v1.0/";
        const string KEY_1 = "690a9a7986ba43b6988e71435b1f6de9";



        public CognitiveService()
        {
        }

        public  static async Task<FaceDetectResult> FaceDetect(Stream imageInput)
        {

            if (imageInput == null)
            {
                return null;
            }
            var url = ENDPOINT + "detect"+"?"+ "returnFaceAttributes=age,gender";

            //header
            //body

            using (var webClient = new WebClient())
            {

                string s = null;
                try
                {

           
                byte[] data = ReadStream(imageInput);

                webClient.Headers[HttpRequestHeader.ContentType]= "application/octet-stream";

                //Ocp-Apim-Subscription-Key
                webClient.Headers.Add("Ocp-Apim-Subscription-Key", KEY_1);

                var result =  await Task.Run(() =>  webClient.UploadData(url, data) );


                if (result == null)
                {
                    return null ;

                }
                  s = Encoding.UTF8.GetString(result,0,result.Length);

             var facedetectResults =    Newtonsoft.Json.JsonConvert.DeserializeObject<FaceDetectResult[]>(s);

                if (facedetectResults.Length >= 1)
                {
                    return facedetectResults[0];
                }
            }catch (Exception ex)
                {
                    Console.WriteLine("Exception : "+ex.Message);
                }

                }
            return null;
        }

        private static byte[] ReadStream(Stream input)
        {
            BinaryReader b = new BinaryReader(input);
            byte[] data = b.ReadBytes((int)input.Length);
            return data;

        }
    }
}
