using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moments.ViewModels;
using Moments.RepositoryDAL;
using Moments.Model.EF;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Moments.Models;
using PdfSharp.Drawing;
using System.Diagnostics;
using PdfSharp.Pdf;
using System.Text.RegularExpressions;

namespace Moments.Controllers
{
    public class MomentController : Controller
    {
        // GET: Moment
        public ActionResult Index()
        {
            MomentViewModel ViewModel = new MomentViewModel();
            MomentDAL DAL = new MomentDAL();
            ViewModel.GetAllStoriesList = DAL.GetStories(null,null,null,null);
            if (ViewModel.GetAllStoriesList!=null && ViewModel.GetAllStoriesList.Any())
            {
                ViewModel.GetAllStoriesList = ViewModel.GetAllStoriesList.Where(st => st.Id > 55).ToList();
            }
            return View(ViewModel);
        }
        // GET: Moment
        public ActionResult CreateStory(string StoryId)
        {
            MomentViewModel ViewModel = new MomentViewModel();
            MomentDAL DAL = new MomentDAL();
            int StoryIdDynamic = 0;
            if (!string.IsNullOrEmpty(StoryId))
            {
                StoryIdDynamic = Convert.ToInt32(StoryId);
                ViewModel.GetStory = DAL.GetStoryData(Convert.ToInt32(StoryId));
                ViewModel.Branches = DAL.GetStoryBranches(StoryId);
            }
            else
            {

            }

            ViewModel.StoriesNodeList = DAL.GetStoryNodes(StoryIdDynamic);
            return View(ViewModel);
        }

        public ActionResult CreateStory1(string StoryId)
        {
            MomentViewModel ViewModel = new MomentViewModel();
            MomentDAL DAL = new MomentDAL();
            int StoryIdDynamic = 0;
            if (!string.IsNullOrEmpty(StoryId))
            {
                StoryIdDynamic = Convert.ToInt32(StoryId);
                ViewModel.GetStory = DAL.GetStoryData(Convert.ToInt32(StoryId));
                ViewModel.Branches = DAL.GetStoryBranches(StoryId);
            }
            else
            {

            }

            ViewModel.StoriesNodeList = DAL.GetStoryNodes(StoryIdDynamic);
            return View(ViewModel);
        }

        public ActionResult SaveStoryNodes(List<StoriesNode> StoryNodesList)
        {

          
           List<StoriesNode> StoriesNodeList = new List<StoriesNode>();
            MomentDAL DAL = new MomentDAL();
            for(int i=0; i< StoryNodesList.Count; i++)
            {
                StoriesNode StoriesNode = new StoriesNode();
                StoriesNode.LayerType = StoryNodesList[i].LayerType;
                StoriesNode.Path = StoryNodesList[i].Path;
                StoriesNode.CreatedBy = Convert.ToInt32(Session["UserProfileId"]);
                StoriesNode.UserProfileId = Convert.ToInt32(Session["UserProfileId"]);
                StoriesNode.StoryId = StoryNodesList[i].StoryId;

                StoriesNodeList.Add(StoriesNode);
                
            }

            int status = DAL.SaveStoryNodes(StoriesNodeList);
            return RedirectToAction("Index");
        }
        public ActionResult AddStory(Story story)
        {
            
            // string StoryName, string fileImage, bool Featured
            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\Images\\StoryImages\\", Server.MapPath(@"\")));
            bool isExists = System.IO.Directory.Exists(originalDirectory.ToString());
            if (!isExists)
            {
                System.IO.Directory.CreateDirectory(originalDirectory.ToString());
            }
            if (!string.IsNullOrEmpty(story.Path))
            {
                var extention = story.Path.Split(',');
                string convert = story.Path.Replace(extention[0] + ",", "");
                convert = convert.Replace('-', '+');
                convert = convert.Replace('_', '/');
                byte[] imageBytes = Convert.FromBase64String(convert);
                string fileName = DateTime.Now.ToString("ddMMyyy") + Guid.NewGuid().ToString() + ".png";
                var path = Path.Combine(Server.MapPath("~/Content/Images/StoryImages"), fileName);
                // Convert byte[] to Image
                using (Image image = Image.FromStream(new MemoryStream(imageBytes)))
                {
                    image.Save(path, ImageFormat.Png);  // Or Png
                }
                story.Path = "/Content/Images/StoryImages/" + fileName;

            }
            Story Story = new Story();
            MomentDAL DAL = new MomentDAL();
            Story.Name = story.Name;
            Story.Path = story.Path;
            Story.CreatedBy = Convert.ToInt32(Session["UserProfileId"]);
            Story.IsFeatured = false;
            Story.Status = 1;

            int id = DAL.AddStory(Story);
            return (RedirectToAction("CreateStory", new { StoryId = id }));
        }


        public ActionResult AddNewNode()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetNetwrok(GetNetworkRequestModel request)
        {
            var service = new MomentDAL();
           var dataModel= service.GetReporting(request);
            if (dataModel.Nodes.Any())
            {
                foreach(var item in dataModel.Nodes)
                {
                    item.image = GetImagePath(item.label,"X",item.id, ".png", Request, Server);
                }
            }
            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }


        public string GetImagePath(string firstName, string lastName, string userGuid, string imageExtension, HttpRequestBase Request, HttpServerUtilityBase Server)
        {
            string imagePath = string.Empty;

            try
            {
                string serverPath;
                var defaultRootPath = Request.Url.GetLeftPart(UriPartial.Authority) + (!String.Equals(Request.ApplicationPath, "/") ? Request.ApplicationPath : "");

                serverPath = Server.MapPath("~/App_Data/Images");

                var extensionWithVersion = imageExtension;
                if (!string.IsNullOrEmpty(imageExtension) && imageExtension.Split('?').Length > 1)
                {
                    imageExtension = imageExtension.Split('?')[0];
                }

                string companyGuid  = null;
                var filePath = Path.Combine(serverPath, (string.IsNullOrEmpty(companyGuid) == true ? userGuid : companyGuid) + imageExtension);

                // Contact case
                if (!System.IO.File.Exists(filePath) && string.IsNullOrEmpty(companyGuid))
                {
                    var image = GenerateAvatarWithIntials(firstName, lastName, imageExtension);
                    if (image != null)
                    {
                        var fs = new BinaryWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write));
                        fs.Write(image.FileContents);
                        fs.Close();
                    }
                }
                // Contact Case
                if (string.IsNullOrEmpty(companyGuid))
                {
                    imagePath = defaultRootPath + "/Moment/LoadImage?userGuid=" + userGuid + "&ext=" + extensionWithVersion; // due to image cache problem we have to pass version as well
                }
               
            }
            catch (Exception ex)
            {
            }
            return imagePath;
        }

        public FileContentResult GenerateAvatarWithIntials(string firstName, string lastName, string imageExtension)
        {

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(imageExtension))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(imageExtension) && imageExtension.Split('?').Length > 1)
            {
                imageExtension = imageExtension.Split('?')[0];
            }

            var avatarString = string.Format("{0}{1}", firstName.Trim()[0], lastName.Trim()[0]).ToUpper();
            using (var bitmap = new Bitmap(270, 270)) // square size outer
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#D3D3D3")))
                    {
                        g.FillRectangle(b, 0, 0, 270, 270); // could be circle
                    }

                    float emSize = 90;
                    g.DrawString(avatarString, new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Regular),
                        new SolidBrush(Color.Black), 35, 60); // 35 is from left , 60 from uper
                }
                ImageFormat ext = ImageFormat.Png;
                imageExtension = imageExtension.ToLower();
                #region Image Extension

                if (imageExtension == ".jpeg")
                {
                    ext = ImageFormat.Jpeg;
                }
                else if (imageExtension == ".bmp")
                {
                    ext = ImageFormat.Bmp;
                }
                else if (imageExtension == ".icon")
                {
                    ext = ImageFormat.Icon;
                }
                else if (imageExtension == ".jpg")
                {
                    ext = ImageFormat.Jpeg;
                }
                else if (imageExtension == ".gif")
                {
                    ext = ImageFormat.Gif;
                }
                else
                {
                    ext = ImageFormat.Png;
                }
                #endregion

                using (var memStream = new MemoryStream())
                {
                    bitmap.Save(memStream, ext);
                    var result = File(memStream.GetBuffer(), "image/png");
                    return result;
                }
            }
        }


        public FilePathResult LoadImage(string userGuid, string ext)
        {
            ext = string.IsNullOrEmpty(ext) ? ".png" : ext.Split('?')[0];
            var appData = Server.MapPath("~/App_Data/Images");
            var image = Path.Combine(appData, userGuid + ext);
            if (!System.IO.File.Exists(image))
            {
                image = Path.Combine(appData, "default.jpg");
            }
            return File(image, "image/png");

        }

        public ActionResult UploadImage()
        {
            var parentId = Request.Form["id"];
            var storyId = Request.Form["story"];

            var service = new MomentDAL();
            var id = service.AddNewNode(new Node
            {
                CreationDate = DateTime.Now,
                Active = true,
                CreatedBy = Convert.ToInt32(Session["UserProfileId"]),
                ParentId = Convert.ToInt32(parentId),
                StoryId = Convert.ToInt32(storyId),
                Contents = "Image Contents",
                Type = "Image"
            });

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var path = Path.Combine(Server.MapPath("~/App_Data/Images"), id + ".png");
                file.SaveAs(path);
            }
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadText()
        {
            var parentId = Request.Form["id"];
            var storyId = Request.Form["story"];
            var contents = Request.Form["contents"];
            var contentType = Request.Form["type"];

            if (contentType == "Video")
            {
                contents = contents.Replace("watch?v=", "embed/");
            }
            if (contentType == "Audio")
            {
                contents = Server.UrlDecode(contents);
                contents = Regex.Match(contents, "<iframe.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
            }
            var service = new MomentDAL();
            var id = service.AddNewNode(new Node
            {
                CreationDate = DateTime.Now,
                Active = true,
                CreatedBy = Convert.ToInt32(Session["UserProfileId"]),
                ParentId = Convert.ToInt32(parentId),
                StoryId = Convert.ToInt32(storyId),
                Contents = contents,
                Type = contentType
            });
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportAsPdf(ExportNodesRequestModel model)
        {
            var list = new List<NodeModel>();
            var service = new MomentDAL();
            var story = service.GetStoryById(model.StoryId);
            model.Nodes.Reverse();
            foreach (var item in model.Nodes)
            {
                var obj = service.GetNodeById(item);
                if (obj != null)
                {
                    obj.image =Server.MapPath("~/App_Data/Images")+"/"+item+".png";
                    list.Add(obj);
                }
            }

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Moments - Data Export";

            foreach (var item in list)
            {
                if (item.type == "Text")
                {
                    int yPoint =20;

                    PdfPage pdfPage = pdf.AddPage();
                    XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

                    var textPerLine = 90;
                    var startAt = 0;
                    var text = item.Contents;
                    var textcount = text.Count();
                    var lineNumber = 1;

                    while (true)
                    {
                        var TextToPrint = "";
                        if(startAt+textPerLine < textcount)
                        {
                            TextToPrint = text.Substring(startAt, textPerLine);
                            int textToReduce = 0;
                            while (!TextToPrint.EndsWith(" "))
                            {
                                TextToPrint = TextToPrint.Substring(0, TextToPrint.Count() - 1);
                                textToReduce = textToReduce + 1;
                            }

                            startAt = startAt + textPerLine - textToReduce;                            
                        }
                        else
                        {
                            TextToPrint = text.Substring(startAt);
                            graph.DrawString(TextToPrint, font, XBrushes.Black, new XRect(30, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                            startAt = textcount;
                            break;
                        }

                        if (lineNumber % 42 == 0)
                        {
                            pdfPage = pdf.AddPage();
                            graph = XGraphics.FromPdfPage(pdfPage);
                            yPoint = 20;
                        }

                        graph.DrawString(TextToPrint, font, XBrushes.Black, new XRect(30, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                        yPoint = yPoint + 18;
                        lineNumber++;
                    }                   
                }
                else if (item.type == "Image")
                {
                    var imgPage = pdf.AddPage();
                    using (XImage img = XImage.FromFile(item.image))
                    {
                        var widht = 600;
                        var height = (int)(((double)widht / (double)img.PixelWidth) * img.PixelHeight);
                        imgPage.Width = widht;
                        imgPage.Height = height;
                        XGraphics gfx = XGraphics.FromPdfPage(imgPage);
                        gfx.DrawImage(img, 0, 0, widht, height);
                    }
                }
                else if (item.type == "Video")
                {
                    int yPoint = 60;
                    PdfPage pdfPage = pdf.AddPage();
                    XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
                    var text = item.Contents;
                    var textcount = text.Count();
                    var  contents = item.Contents.Replace("embed/", "watch?v=");
                    graph.DrawString("YOUTUBE LINK (Click to open)", new XFont("Verdana", 12, XFontStyle.Regular), XBrushes.Black, new XRect(220, 20, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                    graph.DrawString(contents, font, XBrushes.Black, new XRect(30, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                }
                else if (item.type == "Audio")
                {
                    int yPoint = 60;
                    PdfPage pdfPage = pdf.AddPage();
                    XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                    XFont font = new XFont("Verdana", 7, XFontStyle.Regular);
                    var text = item.Contents;
                    var textcount = text.Count();
                    graph.DrawString("SOUNDCLOUD LINK (Click to open)", new XFont("Verdana", 12, XFontStyle.Regular), XBrushes.Black, new XRect(220, 20, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                    graph.DrawString(item.Contents, font, XBrushes.Black, new XRect(30, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                }
            }
            var filename = story.Name + "_" + DateTime.Now.Second + ".pdf";
            string pdfFilename =Path.Combine(Server.MapPath("~/App_Data/Stories"), filename);
            pdf.Save(pdfFilename);
            return Json(new { fileName= filename }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewPdf(string fileName)
        {
            string filePath = "~/App_Data/Stories/" + fileName;
            return File(filePath, "application/pdf");
        }

        public ActionResult SearchStory(string searchStr)
        {
            MomentDAL DAL = new MomentDAL();
            var dataList = new List<Sp_GetStories_Result>();
            var allMoments= DAL.GetStories(null, null, null, null);
            if (allMoments.Any() && !string.IsNullOrEmpty(searchStr))
            {
                foreach(var item in allMoments)
                {
                    if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(searchStr.ToLower()))
                    {
                        dataList.Add(item);
                    }
                }
                return Json(dataList, JsonRequestBehavior.AllowGet);
            }
            return Json(allMoments, JsonRequestBehavior.AllowGet);
        }
    }
}