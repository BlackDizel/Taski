using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using TaskiWindowsPhone7.Controls;
using TaskiWindowsPhone7.Classes;
using System.IO.IsolatedStorage;

namespace TaskiWindowsPhone7.Pages
{
    public partial class PageProjectCreate : PhoneApplicationPage, INotifyPropertyChanged
    {
        
        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //fields
        private DateTime _startDate, _finishDate;

        public DateTime StartDate { get { return this._startDate; } set { this._startDate = value; PropChanged("StartDate"); } }
        public DateTime FinishDate { get { return this._finishDate; } set { this._finishDate = value; PropChanged("FinishDate"); } }
        public int ProjectID;

        //methods
        public PageProjectCreate()
        {
            DataContext = this;
            _startDate = new DateTime(DateTime.Now.Year, 1, 1);
            _finishDate = new DateTime(DateTime.Now.Year, 12, 31);
            
            InitializeComponent();
            
            Loaded += PageProjectCreate_Loaded;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (wpImages.Children.Count == 0) //таким образом избавляемся от бага, когда при возвращении на страницу с выбора даты сбрасывались данные
            {
                foreach (var el in ImageSelector.images)
                {
                    var img = new SelectableImage() { Source = el, Width = 100, Height = 100 };
                    img.Margin = new Thickness(4);
                    wpImages.Children.Add(img);
                    img.Tap += img_Tap;
                }

                if (NavigationContext.QueryString.ContainsKey("ID"))
                {
                    int ID = Int32.Parse(NavigationContext.QueryString["ID"]);

                    Classes.Project pr = (from p in Classes.DBWork.GetProjects() where p.ID == ID select p).FirstOrDefault();

                    ProjectName.Text = pr.Name;
                    ProjectDesc.Text = pr.Description;
                    StartDate = pr.StartDate.HasValue ? pr.StartDate.Value : new DateTime(DateTime.Now.Year, 1, 1);
                    FinishDate = pr.FinishDate.HasValue ? pr.FinishDate.Value : new DateTime(DateTime.Now.Year, 12, 31);
                    ProjectID = ID;
                    (wpImages.Children[pr.ImageID] as SelectableImage).IsSelected = true;
                }
                else
                {
                    (wpImages.Children[0] as SelectableImage).IsSelected = true;
                }


                
            }
        }

        void img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            foreach (var el in wpImages.Children)
                (el as SelectableImage).IsSelected = false;
            var s = (sender as SelectableImage).IsSelected = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= PageProjectCreate_Loaded;
            base.OnNavigatedFrom(e);
        }

        private void PageProjectCreate_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton button1 = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (button1 != null)
            {
                button1.Text = AppResources.CreateButton;
            }
            ApplicationBarIconButton button2 = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            if (button2 != null)
            {
                button2.Text = AppResources.CancelButton;
            }
        }


        private void CreateButtonClick(object sender, EventArgs e)
        {

            int n = 0;
            for (int i = 0; i < wpImages.Children.Count; ++i)
                if ((wpImages.Children[i] as SelectableImage).IsSelected)
                {
                    n = i;
                    break;
                }

            if (ProjectID == 0)//создание проекта
            {
                ProjectID = Classes.DBWork.AddProject(ProjectName.Text, ProjectDesc.Text, StartDate, FinishDate, n); //ADD to DB
            }
            else //изменение проекта
            {
                var p = (from el in MainPage.ProjectCollection where el.ID == ProjectID select el).FirstOrDefault();
                MainPage.ProjectCollection.Remove(p);

                //update secondary tile
                ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + ProjectID));
                if (Tile != null)
                {
                    StandardTileData data = new StandardTileData();
                    data.Title = ProjectName.Text;
                    data.BackgroundImage = ImageSelector.images[n].UriSource;
                  
                    Tile.Update(data);

                }

                Classes.DBWork.UpdateProject(ProjectID, ProjectName.Text, ProjectDesc.Text, StartDate, FinishDate,n); //update DB

            }

            MainPage.ProjectCollection.Add(new ProjectsListElement() //update collection for projects list
            {
                ID = ProjectID,
                Title = ProjectName.Text,
                StartDate = StartDate,
                FinishDate = FinishDate,
                TaskNum = (from el in Classes.DBWork.GetTasks() where el.ProjectID == ProjectID select el).Count(),
                ImgID=n
            });

            NavigationService.GoBack();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// default templated handler
        /// </summary>
        /// <param name="propName"></param>
        private void PropChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }
}