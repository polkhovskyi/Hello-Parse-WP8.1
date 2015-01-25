using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Parse;

namespace Hello_Parse_WP8._1SL
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PrintString(string print)
        {
            ResultsPanel.Children.Add(new TextBlock {Text = print});
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ResultsPanel.Children.Clear();
            PrintString("Starting tests!");
            var query = ParseObject.GetQuery("GameScore");
            PrintString("Cleaning data");
            var gameScores = await query.FindAsync();
            if (gameScores != null)
            {
                foreach (var score in gameScores)
                {
                    await score.DeleteAsync();
                }
            }

            PrintString("Adding new GameScore {123, Anton}");
            var gameScore = new ParseObject("GameScore");
            gameScore["score"] = 123;
            gameScore["playerName"] = "Anton";
            await gameScore.SaveAsync();
            PrintString("Added data");
            query = ParseObject.GetQuery("GameScore");
            gameScore = await query.FirstAsync();
            Debug.Assert(gameScore.Get<string>("playerName").Equals("Anton"));
            Debug.Assert(gameScore.Get<int>("score").Equals(123));
            PrintString(string.Format("Retrieved data score : {0}, name: {1}", gameScore.Get<int>("score"),
                gameScore.Get<string>("playerName")));
            PrintString("Testing push");
            ParsePush.ToastNotificationReceived += (s, args) =>
            {
                var json = ParsePush.PushJson(args);
                Debug.Assert(json["title"].Equals("My test PN"));
                Debug.Assert(json["alert"].Equals("Hello Anton!"));
                PrintString(string.Format("Got push title: {0}, content: {1}", json["title"], json["alert"]));
            };

            var push = new ParsePush
            {
                Data = new Dictionary<string, object> {{"title", "My test PN"}, {"alert", "Hello Anton!"}},
                Channels = new List<string> { "myTesthannel" }
            };
            await push.SendAsync();
            PrintString("Push sent");
        }
    }
}