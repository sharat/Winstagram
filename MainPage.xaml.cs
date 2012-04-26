using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Winstagram
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void OnDoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
			WebClient req = new WebClient();
			req.DownloadStringCompleted +=new System.Net.DownloadStringCompletedEventHandler(req_DownloadStringCompleted);
			req.DownloadStringAsync(new Uri(@"https://api.instagram.com/v1/media/popular?client_id=5c545224bace4df68beb588b384ab80f"));
        }

        private void req_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            JObject o = JObject.Parse(e.Result);

            JArray posts = (JArray) o["data"];


            List<InstagramPost> li = new List<InstagramPost>();

            
            foreach (JObject post in posts)
            {

                string url = (string)post.SelectToken("images").SelectToken("standard_resolution").SelectToken("url");
                string caption = (string)post.SelectToken("caption").SelectToken("text");
                string username = (string)post.SelectToken("caption").SelectToken("from").SelectToken("username");
                InstagramPost p = new InstagramPost();
                p.Username = username;
                p.Description = caption;
                p.Image = url;
                li.Add(p);
            }

            listPosts.ItemsSource = li;
            
        }

        class InstagramPost
        {
            public string Username { get; set; }
            public string Image { get; set; }
        }
    }
}