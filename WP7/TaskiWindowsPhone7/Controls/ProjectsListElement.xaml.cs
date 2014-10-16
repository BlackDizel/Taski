using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;
using TaskiWindowsPhone7.Classes;

namespace TaskiWindowsPhone7.Controls
{
    public partial class ProjectsListElement : UserControl, INotifyPropertyChanged
    {
        //events
        public event PropertyChangedEventHandler PropertyChanged;
        
        //fields
        private string _title, _dates, _counters;
        private DateTime _startDate, _finishDate;
        private int _taskNum, _peopleNum,_imgID;
        
        public int ID;

        public string Title
        { get { return _title; } set { _title = value; PropChanged("Title"); } }

        public DateTime StartDate
        { get { return _startDate; } set { _startDate = value; _setDate(); PropChanged("Dates"); } }

        public DateTime FinishDate
        { get { return _finishDate; } set { _finishDate = value; _setDate(); PropChanged("Dates"); } }

        public int TaskNum
        { get { return _taskNum; } set { _taskNum = value; _setCounters(); PropChanged("Counters"); } }

        public int PeopleNum
        { get { return _peopleNum; } set { _peopleNum = value; _setCounters(); PropChanged("Counters"); } }

        public int ImgID
        { get { return _imgID; }  set { _imgID = value; PropChanged("image id"); } }


        public string Dates
        { get { return _dates; } private set { } }

        public string Counters
        { get { return _counters; } private set { } }

        public Uri Image
        {
            get { return ImageSelector.images[_imgID].UriSource; }
            private set { }
        }
       

        //methods

        /// <summary>
        /// constructor
        /// </summary>
        public ProjectsListElement()
        {
            _title = "SomeTitle";
            _dates = "from 01.01.1900 to 12.31.5999";
            _counters = "tasks: 0, staff: 1";
            DataContext = this;
            InitializeComponent();
           // img.Source = ImageSelector.images[_imgID];
            //    Debug.WriteLine((DataContext as Classes.Project).Name);
        }

        

        /// <summary>
        /// set project dates
        /// </summary>
        private void _setDate()
        {
            _dates = String.Format("{0} {2} {1} {3}",   
                AppResources.ProjectsListElementDateFrom, 
                AppResources.ProjectsListElementDateTo,                                                        
                _startDate.ToString("dd MMM yyyy"), _finishDate.ToString("dd MMM yyyy"));
        }

        /// <summary>
        /// set project counters
        /// </summary>
        private void _setCounters()
        {
            _counters = String.Format("{0}: {2}, {1}: {3}",
                AppResources.ProjectsListElementTasks,
                AppResources.ProjectsListElementStaff,
                _taskNum, _peopleNum);
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

        private void ItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/PageProject.xaml?ID=" + ID, UriKind.Relative));
        }

        private void ContextMenuItemClick(object sender, RoutedEventArgs e)
        {
            var s = (sender as MenuItem).Header.ToString();
            if (s == AppResources.Pin)
            {
                ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID="+ID));
                if (Tile == null)
                {
                    
                    StandardTileData data = new StandardTileData();
                    // tile foreground data
                    data.Title = Title;
                    data.BackgroundImage = ImageSelector.images[_imgID].UriSource;
                   // data.Count = 2;
                    // create a new tile for this Second Page
                    ShellTile.Create(new Uri("/Pages/PageProject.xaml?ID=" + ID, UriKind.Relative), data);
                }

            }
            else
                if (s == AppResources.ProjectListElementEdit)
                {
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/PageProjectCreate.xaml?ID=" + ID, UriKind.Relative));
                }
                else
                    if (s == AppResources.ProjectListElementDelete)
                    {
                        Classes.DBWork.RemoveProject(ID);
                        MainPage.ProjectCollection.Remove((from p in MainPage.ProjectCollection where p.ID == ID select p).FirstOrDefault());
                    }
                
        }
    }
    
}
