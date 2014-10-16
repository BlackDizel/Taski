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

namespace TaskiWindowsPhone7.Pages
{
    public partial class PageTaskCreate : PhoneApplicationPage, INotifyPropertyChanged
    {
        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //fields
        private DateTime _startDate, _finishDate;

        public DateTime StartDate { get { return this._startDate; } set { this._startDate = value; PropChanged("StartDate"); } }
        public DateTime FinishDate { get { return this._finishDate; } set { this._finishDate = value; PropChanged("FinishDate"); } }
        public int TaskID;
        public int ProjectID;
        bool created = false;

        //methods
        public PageTaskCreate()
        {
            DataContext = this;
            _startDate = new DateTime(DateTime.Now.Year, 1, 1);
            _finishDate = new DateTime(DateTime.Now.Year, 12, 31);
            
            InitializeComponent();
            Loaded += PageTaskCreate_Loaded;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!created)
            {
                if (NavigationContext.QueryString.ContainsKey("ProjectID"))
                    ProjectID = Int32.Parse(NavigationContext.QueryString["ProjectID"]);

                if (NavigationContext.QueryString.ContainsKey("ID")) //edit task
                {
                    int ID = Int32.Parse(NavigationContext.QueryString["ID"]);

                    Classes.Task pr = (from p in Classes.DBWork.GetTasks() where p.ID == ID select p).FirstOrDefault();

                    TaskName.Text = pr.Name;
                    TaskDesc.Text = pr.Description;
                    StartDate = pr.StartDate.HasValue ? pr.StartDate.Value : new DateTime(DateTime.Now.Year, 1, 1);
                    FinishDate = pr.FinishDate.HasValue ? pr.FinishDate.Value : new DateTime(DateTime.Now.Year, 12, 31);
                    cbState.Visibility = Visibility.Visible;
                    cbState.IsChecked = pr.State.HasValue ? (pr.State.Value == 1 ? true : false) : false;
                    TaskID = ID;
                }
                else cbState.Visibility = Visibility.Collapsed;    //create task
            }
            created = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= PageTaskCreate_Loaded;
            base.OnNavigatedFrom(e);
        }
        private void PageTaskCreate_Loaded(object sender, RoutedEventArgs e)
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
            if (TaskID == 0)//create task
            {
                TaskID = Classes.DBWork.AddTask(ProjectID, TaskName.Text, TaskDesc.Text, StartDate, FinishDate, 0 ); //ADD to DB
            }
            else //edit task
            {
                Classes.DBWork.UpdateTask(
                    new Classes.Task() 
                    {
                        ID = TaskID,
                        Name = TaskName.Text,
                        ProjectID=ProjectID,
                        Description= TaskDesc.Text, 
                        StartDate= StartDate, 
                        FinishDate= FinishDate, 
                        State= cbState.IsChecked.Value ? 1 : 0                        
                    }
                    );

            }
            PageProject.TaskCollection.Add(new TaskListElement() //update collection for projects list
            {
                TaskID = TaskID,
                Title = TaskName.Text,
                StartDate = StartDate,
                FinishDate = FinishDate                
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