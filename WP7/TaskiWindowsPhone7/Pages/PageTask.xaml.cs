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

namespace TaskiWindowsPhone7
{
    public partial class PageTask : PhoneApplicationPage, INotifyPropertyChanged
    {
        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //fields
        private string _title;
        private string _state;
        private string _description;
        private int TaskID;
        private string _startDate;
        private string _finishDate;

        public string taskTitle
        { get { return _title; } set { _title = value; PropChanged("taskTitle"); } }

        public string Description
        { get { return _description; } set { _description = value; PropChanged("Description"); } }

        public string taskState
        { get { return _state; } set { _state = value; PropChanged("taskState"); } }

        public string StartDate
        { get { return _startDate; } set { _startDate = value; } }

        public string FinishDate
        { get { return _finishDate; } set { _finishDate = value; } }

        //methods
        public PageTask()
        {
            _title = "TaskName";
            _description = "Some text description";
            _state = AppResources.InProgress;
            _startDate = "01 Jan 2010";
            _finishDate = "31 Dec 2014";
            DataContext = this;
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Loaded+=PageTask_Loaded;
            if (NavigationContext.QueryString.ContainsKey("ID"))
                TaskID = Int32.Parse(NavigationContext.QueryString["ID"]);

            if (TaskID != 0)
            {
                Classes.Task t = (from c in Classes.DBWork.GetTasks() where c.ID == TaskID select c).FirstOrDefault();
                if (t != null)
                {
                    taskTitle = t.Name;
                    Description = t.Description;
                    taskState = t.State.HasValue ? (t.State > 0 ? AppResources.Complete : AppResources.InProgress) : AppResources.InProgress;
                    StartDate = (t.StartDate.HasValue?t.StartDate.Value:new DateTime(DateTime.Now.Year,1,1)).ToString("dd MMM yyyy");
                    FinishDate = (t.FinishDate.HasValue?t.FinishDate.Value:new DateTime(DateTime.Now.Year,1,1)).ToString("dd MMM yyyy");

                }
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Classes.DBWork.UpdateTask(new Classes.Task() 
            {
                ProjectID = (from c in Classes.DBWork.GetTasks() where c.ID==TaskID select c.ProjectID).FirstOrDefault(),
                ID=TaskID,
                Name = taskTitle,
                Description = Description,
                State = taskState==AppResources.Complete?1:0
            });
            Loaded -= PageTask_Loaded;            
            base.OnNavigatedFrom(e);
        }
        private void PageTask_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton button1 = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (button1 != null)
            {
                button1.Text = AppResources.ToComplete;
            }
            ApplicationBarMenuItem menuItem1 = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            if (menuItem1 != null)
            {
                menuItem1.Text = AppResources.ToAbort;
            }           
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

        private void RedoButtonClick(object sender, EventArgs e)
        {
            taskState = AppResources.InProgress;
        }

        private void ToComleteButtonClick(object sender, EventArgs e)
        {
            taskState = AppResources.Complete;
        }
    }
}