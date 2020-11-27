using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Formatting;
using System.Web;

namespace server
{
    public class ProductsController : ApiController
    {
        #region CONSTANTS
        enum Params : int
        {
            WIDTH = 0,
            ROT_X = 1,
            ROT_Y = 2,
            HEIGHT = 3,
            POS_X = 4,
            POS_Y = 5
        }
        const string PATH = @"C:\Users\urban\OneDrive - Univerza v Mariboru\3_letnik\2_semester\GIS\vaja01_geoserver\geoserver\server\Layers\";
        const string URL = "https://ion.gemma.feri.um.si/ion/services/geoserver/demo1/wms";
        #endregion

        struct TfwParams
        {
            public double width;
            public double height;
            public double rot_x;
            public double rot_y;
            public double pos_x;
            public double pos_y;
        }

        [Route("api/geoserver")]
        [HttpGet]
        public IHttpActionResult GetRequest(HttpRequestMessage req, [FromUri] Request r)
        {
            MemoryStream stream = null;
            string[] bboxParams = r.bbox.Split(',');
            BBox bbox = new BBox();
            bbox.minx = double.Parse(bboxParams[0]);
            bbox.miny = double.Parse(bboxParams[1]);
            bbox.maxx = double.Parse(bboxParams[2]);
            bbox.maxy = double.Parse(bboxParams[3]);

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (r.request == "GetMap")
            {
                Console.WriteLine("GetMap request.");
                if(Directory.Exists(PATH + r.layers.ToUpper())) //layer exists locally
                {
                    stream = GetMap(r);
                }
                else //layer not found; request needed
                {
                    stream = new MemoryStream();
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["service"] = "WMS";
                    query["bbox"] = r.bbox;
                    query["version"] = "1.1.0";
                    query["request"] = r.request;
                    query["layers"] = r.layers;
                    query["width"] = r.width.ToString();
                    query["height"] = r.height.ToString();
                    query["srs"] = "EPSG:3794";
                    query["format"] = r.format;
                    string queryString = query.ToString();
                    HttpResponseMessage receivedData = client.GetAsync("?" + queryString).Result;
                    Console.WriteLine(receivedData.RequestMessage);
                    Task<byte[]> buffer = receivedData.Content.ReadAsByteArrayAsync();
                    stream = new MemoryStream(buffer.Result);
                    //Bitmap img = Image.FromStream(stream) as Bitmap;
                    //img.Save("geoserverica", ImageFormat.Png);
                }
                response.Content = new StreamContent(stream);
                switch (r.format)
                {
                    case "image/png":
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        break;
                    case "image/jpeg":
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        break;
                    case "image/jpg":
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        break;
                    case "image/gif":
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                        break;
                    default:
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        break;
                }
                response.Content.Headers.ContentLength = stream.Length;
            } 
            else if (r.request == "GetCapabilities")
            {
                Console.WriteLine("GetCapabilities request.");
                TfwParams parameters = parseParams(PATH + r.layers.ToUpper());
                response.Content = new ObjectContent<TfwParams>(parameters, new JsonMediaTypeFormatter());
            }

            return ResponseMessage(response);
        }

        private MemoryStream GetMap(Request req)
        {
            Console.WriteLine("Building map response ...");
            TfwParams parameters = parseParams(PATH + req.layers.ToUpper());

            string[] bboxParams = req.bbox.Split(new[] { "," }, StringSplitOptions.None);
            BBox bbox = new BBox();
            bbox.minx = double.Parse(bboxParams[0]);
            bbox.miny = double.Parse(bboxParams[1]);
            bbox.maxx = double.Parse(bboxParams[2]);
            bbox.maxy = double.Parse(bboxParams[3]);

            //image processing and returning
            string[] layers = Directory.GetFiles(PATH + req.layers.ToUpper(), "*.TIF");
            Bitmap src = Image.FromFile(layers[0]) as Bitmap;
            Bitmap cropped = cropPicture(parameters, bbox, src, req);
            MemoryStream fstream = new MemoryStream();
            switch (req.format)
            {
                case "image/png":
                    cropped.Save(fstream, ImageFormat.Png);
                    break;
                case "image/jpeg":
                    cropped.Save(fstream, ImageFormat.Jpeg);
                    break;
                case "image/jpg":
                    cropped.Save(fstream, ImageFormat.Jpeg);
                    break;
                case "image/gif":
                    cropped.Save(fstream, ImageFormat.Gif);
                    break;
                default:
                    cropped.Save(fstream, ImageFormat.Png);
                    break;
            }
            fstream.Position = 0;

            Console.WriteLine("Image size: " + fstream.Length);
            return fstream;
        }

        private TfwParams parseParams(string path)
        {
            string[] files = Directory.GetFiles(path, "*.tfw");
            IEnumerable<string> lines = File.ReadLines(files[0]);
            TfwParams param = new TfwParams
            {
                width = double.Parse(lines.ElementAt((int)Params.WIDTH)),
                rot_x = double.Parse(lines.ElementAt((int)Params.ROT_X)),
                rot_y = double.Parse(lines.ElementAt((int)Params.ROT_Y)),
                height = double.Parse(lines.ElementAt((int)Params.HEIGHT)),
                pos_x = double.Parse(lines.ElementAt((int)Params.POS_X)),
                pos_y = double.Parse(lines.ElementAt((int)Params.POS_Y))
            };

            return param;
        }

        private Bitmap cropPicture(TfwParams parameters, BBox bbox, Bitmap src, Request req)
        {
            //image cropping
            double startingX = (bbox.minx - parameters.pos_x) / parameters.width;
            double endingX = (bbox.maxx - parameters.pos_x) / parameters.width;
            double startingY = (bbox.miny - parameters.pos_y) / parameters.height;
            double endingY = (bbox.maxy - parameters.pos_y) / parameters.height;
            Rectangle cropRect = new Rectangle((int)startingX, -(int)startingY, (int)(endingX - startingX), -(int)(endingY - startingY));
            Bitmap target = new Bitmap(req.width, req.height);

            using(Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                    cropRect,
                    GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}
