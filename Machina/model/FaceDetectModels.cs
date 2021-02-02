using System;
using System.Threading.Tasks;

namespace Machina.model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class FaceAttributes
    {
        public string gender { get; set; }
        public double age { get; set; }
    }

    public class FaceDetectResult
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }

        public static explicit operator FaceDetectResult(Task<FaceDetectResult> v)
        {
            throw new NotImplementedException();
        }
    }

}
