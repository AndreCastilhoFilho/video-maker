using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using videoMaker.Domain.Models;
using videoMaker.Domain.Robots.Services;

namespace videoMaker.Domain.Robots
{
    public class Image
    {
        string GoogleApi { get; set; }
        string SearchEngineKey { get; set; }

        public Image(string googleApi, string searchEngineKey)
        {
            GoogleApi = googleApi;
            SearchEngineKey = searchEngineKey;
        }

        public void Robot()
        {
            var content = State.Load();

            FetchAllGoogleImages(content);
            DownloadAllImages(content);
            ConvertAllImages(content);

            State.Save(content);
        }

        private void ConvertAllImages(Content content)
        {
            var index = 0;
            foreach (var s in content?.Sentences.Where(s => s.Keywords.Any()))
            {
                index++;
                ConvertImage(index);
            }
        }

        private void ConvertImage(int index)
        {
            var inputFile = $"{AppDomain.CurrentDomain.BaseDirectory }\\Content\\{index}-original.jpg";
            var outputFile = $"{AppDomain.CurrentDomain.BaseDirectory }\\Content\\{index}-converted.jpg";
            var width = 1920;
            var height = 1080;

            using (MagickImage image = new MagickImage(inputFile))
            {
                using (IMagickImage backgroundImg = image.Clone())
                {
                    backgroundImg.Blur(0, 9);
                    backgroundImg.Crop(width, height, Gravity.Center);
                    backgroundImg.RePage();

                    image.Resize(0, height);

                    IMagickImage _shadow = new MagickImage(MagickColor.FromRgb(0, 0, 0), image.Width + 20, height);
                    _shadow.Shadow(backgroundImg.Width, 400, 10, (Percentage)90);

                    backgroundImg.Composite(_shadow, Gravity.Center, CompositeOperator.Atop);
                    backgroundImg.Composite(image, Gravity.Center, CompositeOperator.SrcAtop);
                    backgroundImg.Write(outputFile);
                }
            }


        }

        private void FetchAllGoogleImages(Content content)
        {
            foreach (var s in content?.Sentences.Where(s => s.Keywords.Any()))
            {
                string query = GetBestKeywordOption(content, s);
                s.Images = FetchGoogleAndReturnImagesUrl(query).ToArray();
                s.GoogleSearchQuery = query;
            }
        }

        public void DownloadAllImages(Content content)
        {
            var DownloadedImages = new List<string>();

            var index = 0;
            foreach (var s in content?.Sentences.Where(s => s.Keywords.Any()))
            {
                index++;
                var url = s.Images.FirstOrDefault();
                try
                {
                    TryDownloadImage(DownloadedImages, index, url);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao baixar Imagem {url}");

                    if (s.Images.Length > 1 && url != s.Images[1])
                    {
                        Console.WriteLine($"Tentando baixar segunda opção de imagem");
                        TryDownloadImage(DownloadedImages, index, s.Images[1]);
                    }
                    //throw;
                }

            }
        }

        private void TryDownloadImage(List<string> DownloadedImages, int index, string url)
        {
            if (DownloadedImages.Contains(url))
                throw new Exception("Imagem já foi baixada");

            DownloadImage(url, index.ToString());
            DownloadedImages.Add(url);

            Console.WriteLine($"Baixou imagem com sucesso {url}");
        }

        private void DownloadImage(string url, string name)
        {
            var baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory }\\Content\\";

            System.IO.Directory.CreateDirectory(baseDirectory);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(url), $"{baseDirectory}{name}-original" + ".jpg");
            }
        }

        private static string GetBestKeywordOption(Models.Content content, Models.Content.Sentence s)
        {
            string keyword = GetLeastSimilarKeyword(content.SearchTerm, s.Keywords);

            return $"{content.SearchTerm} {keyword}";
        }

        private static string GetLeastSimilarKeyword(string searchTerm, string[] keywords)
        {
            return keywords.Select(k => new { similarityScore = LevenshteinDistance.Compute(searchTerm, k), keyword = k })
                  .ToList()
                  .OrderByDescending(o => o.similarityScore)
                  .First().keyword;

        }

        private List<string> FetchGoogleAndReturnImagesUrl(string query)
        {
            var _clientService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApplicationName = "Video Maker",
                ApiKey = GoogleApi,

            });

            var listRequest = _clientService.Cse.List(query);
            listRequest.Cx = SearchEngineKey;
            listRequest.Num = 2;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.ImgSize = CseResource.ListRequest.ImgSizeEnum.Huge;
            var search = listRequest.Execute();

            var imagesUrl = search.Items.Select(x => x.Link).ToList();

            return imagesUrl;
        }


    }
}
