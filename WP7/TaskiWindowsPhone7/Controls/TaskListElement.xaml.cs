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

namespace TaskiWindowsPhone7.Controls
{
    public partial class TaskListElement : UserControl, INotifyPropertyChanged
    {
        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //fields
        private string _title;
        private string _state;
        private DateTime _startDate, _finishDate;

        public string Title
        { get { return _title; } set { _title = value; PropChanged("Title"); } }

        public string State
        { get { return _state; } set { _state = value; PropChanged("State"); } }

        public string Description;
        public DateTime StartDate { get { return this._startDate; } set { this._startDate = value; PropChanged("StartDate"); } }
        public DateTime FinishDate { get { return this._finishDate; } set { this._finishDate = value; PropChanged("FinishDate"); } }
        
        public int ProjectID;
        public int TaskID;


        //methods
        public TaskListElement()
        {
            _title = "taskName";
            _state = "finished";
            DataContext = this;
            InitializeComponent();
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
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/PageTask.xaml?ID=" + TaskID, UriKind.Relative));
        }


        private void ContextMenuItemClick(object sender, RoutedEventArgs e)
        {
            var s = (sender as MenuItem).Header.ToString();
            if (s == AppResources.Pin) //create secondary tile
            {
                ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + TaskID));
                if (Tile == null)
                {

                    StandardTileData data = new StandardTileData();
                    data.Title = Title;
                    ShellTile.Create(new Uri("/Pages/PageTask.xaml?ID=" + TaskID, UriKind.Relative), data);
                }

            }
            else
                if (s == AppResources.ProjectListElementEdit) //edit
                {
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/PageTaskCreate.xaml?ID=" + TaskID, UriKind.Relative));
                }
                else
                    if (s == AppResources.ProjectListElementDelete) //delete
                    {
                        Classes.DBWork.RemoveTask(TaskID);
                        PageProject.TaskCollection.Remove((from p in PageProject.TaskCollection where p.TaskID == TaskID select p).FirstOrDefault());
                    }

        }

        
    }
}
