using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace TaskiWindowsPhone7
{
    public partial class PageProject : PhoneApplicationPage
    {
        //fields
        public static ObservableCollection<Controls.TaskListElement> TaskCollection;
        private int ProjectID;

        //methods
        public PageProject()
        {
            
            InitializeComponent();
            Loaded += PageProject_Loaded;
                    
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= PageProject_Loaded;            
            base.OnNavigatedFrom(e);
        }

        private void PageProject_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton button1 = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (button1 != null)
            {
                button1.Text = AppResources.CreateTaskButton;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("ID"))
                ProjectID = Int32.Parse(NavigationContext.QueryString["ID"]);

            TaskCollection = new ObservableCollection<Controls.TaskListElement>();
            var tasks = from el in Classes.DBWork.GetTasks() where el.ProjectID == ProjectID select el;
            foreach (var el in tasks)
                TaskCollection.Add(new Controls.TaskListElement()
                {
                    ProjectID = el.ProjectID.Value,
                    TaskID = el.ID,
                    Title = el.Name,
                    StartDate = el.StartDate.Value,
                    FinishDate = el.FinishDate.Value,
                    Description = el.Description,
                    State = el.State.ToString()
                });
            ItemsList.ItemsSource = TaskCollection;
        }

        private void ButtonNewTaskClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PageTaskCreate.xaml?ProjectID="+ProjectID,UriKind.Relative));            
        }        
    }
}