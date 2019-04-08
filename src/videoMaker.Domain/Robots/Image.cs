using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using videoMaker.Domain.Models;
using videoMaker.Domain.Robots.Services;
using static Google.Apis.Customsearch.v1.CseResource.SiterestrictResource.ListRequest;

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

            State.Save(content);
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
            var downloadedImages = new List<string>();

            foreach (var s in content?.Sentences.Where(s => s.Keywords.Any()))
            {
                foreach (var url in s.Images)
                {
                    try
                    {
                        if (downloadedImages.Contains(url))
                            throw new Exception("Imagem já foi baixada");

                        DownloadImage(url, Guid.NewGuid().ToString());
                        downloadedImages.Add(url);

                        Console.WriteLine($"Baixou imagem com sucesso {url}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao baixar Imagem {url}");
                        //throw;
                    }
                }
            }
        }

        private void DownloadImage(string url, string name)
        {
            var baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory }\\Content\\";

            System.IO.Directory.CreateDirectory(baseDirectory);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(url), $"{baseDirectory}{name}" + System.IO.Path.GetExtension(url));
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
