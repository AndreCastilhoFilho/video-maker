using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var s in content?.Sentences.Where(s => s.Keywords.Any()))
            {
                var query = $"{content.SearchTerm} {s.Keywords[0]}";
                s.Images = FetchGoogleAndReturnImagesUrl(query).ToArray();
                s.GoogleSearchQuery = query;
            }

            Console.WriteLine(JsonConvert.SerializeObject(content.Sentences.Where(s => s.Keywords.Any()), Formatting.Indented));

            State.Save(content);
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
