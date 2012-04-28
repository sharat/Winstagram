using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;


namespace Winstagram
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<InstagramPost>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<InstagramPost> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {

            /*for (int i = 0; i < 10; i++)
            {
                Items.Add(new InstagramPost()
                {
                    Username = "User" + i.ToString(),
                    Caption = "Description" + i.ToString(),
                    Image = @"http://upload.wikimedia.org/wikipedia/en/2/28/Instagram_logo.png"
                    
                });

            }*/

            WebClient req = new WebClient();
            
            req.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(
                delegate(object sender, System.Net.DownloadStringCompletedEventArgs e)
                {
                    JObject o = JObject.Parse(e.Result);
                    JArray posts = (JArray)o["data"];
                    foreach (JObject post in posts)
                    {
                        

                        try
                        {
                            JObject captionArray = (JObject)post.SelectToken("caption");
                            InstagramPost item = new InstagramPost();

                            if (captionArray != null)
                            {

                                Items.Add(new InstagramPost()
                                {
                                    Caption = (string)captionArray.SelectToken("text"),
                                    Username = (string)captionArray.SelectToken("from").SelectToken("username"),
                                    Image = (string)post.SelectToken("images").SelectToken("standard_resolution").SelectToken("url")
                                });
                            }
                            else
                                continue;
                        }
                        catch (Exception )
                        {
                            continue;
                        }
                    }
                    this.IsDataLoaded = true;
                });
			
            // download the popular photos
            req.DownloadStringAsync(new Uri(@"https://api.instagram.com/v1/media/popular?client_id=5c545224bace4df68beb588b384ab80f"));
        }

        public class InstagramPost
        {
            public string Username { get; set; }
            public string Image { get; set; }
            public string Caption { get; set; }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}