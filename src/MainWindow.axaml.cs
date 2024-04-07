using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using JetBrains.Annotations;
using System;
using System.IO;

namespace src
{
    public partial class MainWindow : Window
    {
        public bool isFileToolsOpened = false, isWindowToolsOpened = false, isFileInCreation = false;
        public string filepath = "";

        public SolidColorBrush pageBackground = new SolidColorBrush();

        public MainWindow()
        {
            InitializeComponent();
            file_tools_btn.Click += (s, e) =>
            {
                if (isFileToolsOpened)
                {
                    file_tools.Height = 0;
                    docRenderer.RowDefinitions[0].Height = new GridLength(25);
                    isFileToolsOpened = false;
                }
                else
                {
                    file_tools.Height = 80;
                    docRenderer.RowDefinitions[0].Height = new GridLength(50);
                    isFileToolsOpened = true;
                }
            };
            open_file_btn.Click += (s, e) =>
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.Title = "Open MTF";
                string? path = fd.ShowAsync(this).Result[0];
                if (path is not null)
                {
                    this.filepath = path;
                    ReadFile(this.filepath);
                }
                else
                {

                }
            };
            window_tools_btn.Click += (s, e) =>
            {
                if (isWindowToolsOpened)
                {
                    window_tools.Height = 0;
                    docRenderer.RowDefinitions[0].Height = new GridLength(25);
                    isWindowToolsOpened = false;
                }
                else
                {
                    window_tools.Height = 80;
                    docRenderer.RowDefinitions[0].Height = new GridLength(85);
                    isWindowToolsOpened = true;
                }
            };
            dark_mode_on_btn.Click += (s, e) =>
            {
                String os = Environment.OSVersion.Platform.ToString().ToLower();
                String homepath = "";
                if (os.StartsWith("win"))
                {
                    homepath = "C:/Users/" + Environment.UserName;
                }
                else if (os.StartsWith("linux"))
                {
                    homepath = "/home/" + Environment.UserName;
                }
                File.WriteAllText(homepath + "/.limotext/theme.txt", "theme: dark;");
                LoadTheme();
            };
            light_mode_on_btn.Click += (s, e) =>
            {
                String os = Environment.OSVersion.Platform.ToString().ToLower();
                String homepath = "";
                if (os.StartsWith("win"))
                {
                    homepath = "C:/Users/" + Environment.UserName;
                }
                else if (os.StartsWith("linux"))
                {
                    homepath = "/home/" + Environment.UserName;
                }
                File.WriteAllText(homepath + "/.limotext/theme.txt", "theme: light;");
                LoadTheme();
            };
            LoadTheme();
        }

        public void LoadTheme()
        {
            String os = Environment.OSVersion.Platform.ToString().ToLower();
            String homepath = "";
            if (os.StartsWith("win"))
            {
                homepath = "C:/Users/" + Environment.UserName;
            }
            else if (os.StartsWith("linux"))
            {
                homepath = "/home/" + Environment.UserName;
            }
            String ltFolder = homepath + "/.limotext";
            switch (Directory.Exists(ltFolder))
            {
                case true:
                    String ltthemeFile = ltFolder + "/theme.txt";
                    switch (File.Exists(ltthemeFile))
                    {
                        case true:
                            if (File.ReadAllText(ltthemeFile) == null || File.ReadAllText(ltthemeFile) == "")
                            {
                                File.WriteAllText(ltthemeFile, "theme: light;");
                                LoadTheme();
                            }
                            else
                            {
                                foreach (string line in File.ReadAllLines(ltthemeFile))
                                { 
                                    if (line.StartsWith("theme:"))
                                    {
                                        switch (line.Replace("theme:", "").Replace(";", "").Replace(" ", ""))
                                        {
                                            case "light":
                                                SolidColorBrush appBackground = new SolidColorBrush();
                                                appBackground.Color = Colors.White;

                                                navbar.Background = Brushes.Red;
                                                file_tools.Background = Brushes.LightGray;
                                                window_tools.Background = Brushes.LightGray;
                                                contentRenderer.Background = appBackground;
                                                open_file_btn.Background = appBackground;

                                                open_file_btn.Foreground = Brushes.Black;
                                                //open_folder_btn.Foreground = Brushes.Black;
                                                dark_mode_on_btn.Foreground = Brushes.Black;
                                                light_mode_on_btn.Foreground = Brushes.Black;

                                                pageBackground.Color = Colors.LightGray;
                                                break;

                                            case "dark":
                                                SolidColorBrush scb = new SolidColorBrush();
                                                SolidColorBrush appBg = new SolidColorBrush();
                                                scb.Color = Color.FromRgb(75, 75, 75);
                                                appBg.Color = Color.FromRgb(50, 50, 50);

                                                navbar.Background = Brushes.DarkRed;
                                                file_tools.Background = scb;
                                                window_tools.Background = scb;
                                                contentRenderer.Background = appBg;
                                                open_file_btn.Background = scb;

                                                open_file_btn.Foreground = Brushes.WhiteSmoke;
                                                //open_folder_btn.Foreground = Brushes.WhiteSmoke;
                                                dark_mode_on_btn.Foreground = Brushes.WhiteSmoke;
                                                light_mode_on_btn.Foreground = Brushes.WhiteSmoke;

                                                pageBackground.Color = Color.FromRgb(75, 75, 75);
                                                break;
                                        }
                                    }
                                }
                            }
                            break;

                        case false:
                            File.Create(ltthemeFile);
                            LoadTheme();
                            break;
                    }
                    break; 

                case false:
                    Directory.CreateDirectory(ltFolder);
                    LoadTheme();
                    break; 
            }
        }

        public void ReadFile(string path)
        {
            pageViewer.Content = null;
            Grid page = new Grid();
            page.Margin = new Avalonia.Thickness(25, 0, 25, 0);
            page.Background = pageBackground;
            foreach (string line in File.ReadAllLines(path))
            {
                switch (line.Substring(0, line.IndexOf("(")).Replace(" ", ""))
                {
                    case "Button":
                        Button btn = new Button();
                        btn.FontSize = 25;
                        string properties = line.Replace(line.Substring(0, line.IndexOf("(")), "").Replace(line.Substring(line.IndexOf(")")), "");
                        ParseProps(btn, 0, 0, properties);
                        page.Children.Add(btn);
                        break;

                    case "Label":
                        Label lbl = new Label();
                        lbl.FontSize = 25;
                        string props = line.Replace(line.Substring(0, line.IndexOf("(")), "").Replace(line.Substring(line.IndexOf(")")), "");
                        ParseProps(lbl, 0, 0, props);
                        page.Children.Add(lbl);
                        break;
                }
            }
            pageViewer.Content = page;
        }

        public void ParseProps(Button btn, double xOffset, double yOffset, string properties)
        {
            foreach (string property in properties.Split(","))
            {
                string property_name = property.Substring(0, property.IndexOf("="));
                string property_content = property.Substring(property.IndexOf("=") + 1);
                switch (property_name)
                {
                    case "x":
                        xOffset = Double.Parse(property_content);
                        break;

                    case "y":
                        yOffset = Double.Parse(property_content);
                        break;

                    case "height":
                        btn.Height = Double.Parse(property_content);
                        break;

                    case "width":
                        btn.Width = Double.Parse(property_content);
                        break;

                    case "text":
                        btn.Content = property_content.Replace("\"", "");
                        break;

                    case "textalign":
                        switch (property_content)
                        {
                            case "center-h":
                                btn.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                                break;

                            case "center-v":
                                btn.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                                break;

                            case "left":
                                btn.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                                break;

                            case "right":
                                btn.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Right;
                                break;
                        }
                        break;

                    case "fontsize":
                        btn.FontSize = Double.Parse(property_content);
                        break;

                    case "highlighter":
                        SolidColorBrush bgBrush;
                        switch (property_content)
                        {
                            case "blue":
                                bgBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
                                btn.Background = bgBrush;
                                break;
                        }
                        break;
                }
            }
            btn.Margin = new Avalonia.Thickness(xOffset, yOffset, 0, 0);
        }

        public void ParseProps(Label lbl, double xOffset, double yOffset, string properties)
        {
            foreach (string property in properties.Split(","))
            {
                string property_name = property.Substring(0, property.IndexOf("="));
                string property_content = property.Substring(property.IndexOf("=") + 1);
                switch (property_name)
                {
                    case "x":
                        xOffset = Double.Parse(property_content);
                        break;

                    case "y":
                        yOffset = Double.Parse(property_content);
                        break;

                    case "height":
                        lbl.Height = Double.Parse(property_content);
                        break;

                    case "width":
                        lbl.Width = Double.Parse(property_content);
                        break;

                    case "text":
                        lbl.Content = property_content.Replace("\"", "");
                        break;

                    case "textalign":
                        switch (property_content)
                        {
                            case "center-h":
                                lbl.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                                break;

                            case "center-v":
                                lbl.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                                break;

                            case "left":
                                lbl.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                                break;

                            case "right":
                                lbl.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Right;
                                break;
                        }
                        break;

                    case "fontsize":
                        lbl.FontSize = Double.Parse(property_content);
                        break;

                    case "highlighter":
                        SolidColorBrush bgBrush;
                        switch (property_content)
                        {
                            case "blue":
                                bgBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
                                lbl.Background = bgBrush;
                                break;
                        }
                        break;
                }
            }
            lbl.Margin = new Avalonia.Thickness(xOffset, yOffset, 0, 0);
        }
    }
}