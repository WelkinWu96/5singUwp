using System;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using System.Xml.XPath;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace _5singUwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<song> ycList;
        private List<song> fcList;

        public MainPage()
        {
            this.InitializeComponent();
            getHtml();
            
        }
        async void getHtml()
        {
            Uri uri = new Uri("http://5sing.kugou.com/index.html");
            HttpClient httpClient = new HttpClient();
            string result = await httpClient.GetStringAsync(uri);
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(result);
            //HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//*[@id='yc_list_tj']/li/a[2]/h4/text()");
            HtmlNodeCollection ycNodes = html.DocumentNode.SelectNodes("/html/body/div[4]/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/div");
            HtmlNodeCollection fcNodes = html.DocumentNode.SelectNodes("/html/body/div[4]/div[1]/div[1]/div[1]/div[2]/div[3]/div[2]/div");
            ycList = new List<song>();
            fcList = new List<song>();          
            foreach (HtmlNode node in ycNodes)
            {
                string songName = node.SelectSingleNode("div[1]/a[1]").InnerText;
                string songPost = node.SelectSingleNode("div[1]/a[2]").InnerText;
                string songImg = node.SelectSingleNode("div[2]/div[1]/img[1]").Attributes["src"].Value;
                string songHref = node.SelectSingleNode("div[1]/a[1]").Attributes["href"].Value;
           
                String songId = "";
                Regex rex = new Regex(@"http://5sing.kugou.com/yc/(\d+).html");
                Match match = rex.Match(songHref);
                songId = match.Groups[1].Value;

                string songType = "yc";
                ycList.Add(new song() { Name = songName, Post = songPost, ID = songId, Type = songType, Img = songImg });
            }

            foreach (HtmlNode node in fcNodes)
            {
                string songName = node.SelectSingleNode("div[1]/a[1]").InnerText;
                string songPost = node.SelectSingleNode("div[1]/a[2]").InnerText;
                string songImg = node.SelectSingleNode("div[2]/div[1]/img[1]").Attributes["src"].Value;
                string songHref = node.SelectSingleNode("div[1]/a[1]").Attributes["href"].Value;

                String songId = "";
                Regex rex = new Regex(@"http://5sing.kugou.com/fc/(\d+).html");
                Match match = rex.Match(songHref);
                songId = match.Groups[1].Value;

                string songType = "fc";
                fcList.Add(new song() { Name = songName, Post = songPost, ID = songId, Type = songType, Img = songImg });
            }
            ycMusicList.ItemsSource = ycList;
            fcMusicList.ItemsSource = fcList;
            //HttpResponseMessage response = await httpClient.GetAsync(uri);
            //string responseBody = await response.Content.ReadAsStringAsync();
        }

        async void getPlayUri(string id,string type)
        {
            media.Stop();
            HttpClient httpClient = new HttpClient();
            Uri json = new Uri("http://mobileapi.5sing.kugou.com/song/transcoding?songid="+id+"&songtype="+type);
            HttpResponseMessage response1 = await httpClient.GetAsync(json);
            Uri res = response1.RequestMessage.RequestUri;
            media.Source =res;
            media.Play();

        }

        private void stop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            media.Stop();
        }

        private void ycMusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx= ycMusicList.SelectedIndex;
            List<song> songList = (List<song>)ycMusicList.ItemsSource;
            string id= songList[idx].ID;
            getPlayUri(id,"yc");
        }

        async private void getPostInfo(string id)
        {
            HttpClient httpclient = new HttpClient();

            //待
            Uri postInfoUrl =new Uri("");
            string postHtml = await httpclient.GetStringAsync(postInfoUrl);

            HtmlDocument html=new HtmlDocument();
            html.LoadHtml(postHtml);

            //待
            HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("");
            foreach(var item in nodes)
            {
                
            }
        }

        private void fcMusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = fcMusicList.SelectedIndex;
            List<song> songList = (List<song>)fcMusicList.ItemsSource;
            string id = songList[idx].ID;
            getPlayUri(id, "fc");
        }
    }
}
