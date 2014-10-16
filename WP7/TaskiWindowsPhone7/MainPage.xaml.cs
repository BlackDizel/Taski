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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Phone.Tasks;

namespace TaskiWindowsPhone7
{
    public partial class MainPage : PhoneApplicationPage
    {
        //fields
        public static ObservableCollection<Controls.ProjectsListElement> ProjectCollection; //Classes.Project -> Name->Title, from StartDate to FinishDate
                
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            Classes.DBWork.CreateDB();
            ProjectCollection = new ObservableCollection<Controls.ProjectsListElement>();
            var tasksAll = Classes.DBWork.GetTasks();
            
            foreach (var el in Classes.DBWork.GetProjects())
            {
                var tasks = from c in tasksAll where c.ProjectID == el.ID select c;

                var p = new Controls.ProjectsListElement()
                {
                    Title = el.Name,
                    StartDate = el.StartDate.HasValue ? el.StartDate.Value : new DateTime(1900, 1, 1),
                    FinishDate = el.FinishDate.HasValue ? el.FinishDate.Value : new DateTime(5999, 12, 31),
                    ID = el.ID,
                    TaskNum = tasks != null ? tasks.Count() : 0,
                    ImgID = el.ImageID
                };
                ProjectCollection.Add(p);
            }
            ItemsList.ItemsSource = ProjectCollection;

            CheckForUpdates();
            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton button1 = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (button1 != null)
            {
                button1.Text = AppResources.CreateProjectButton;
            }
            ApplicationBarMenuItem menuItem1 = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            if (menuItem1 != null)
            {
                menuItem1.Text = AppResources.MenuItemAbout;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            base.OnNavigatedFrom(e);
        }
        private void ButtonNewProjectClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PageProjectCreate.xaml",UriKind.Relative));
        }

        private async void CheckForUpdates()
        {
            try
            {
                var _informationService = new MarketplaceInformationService();
                var _applicationManifestService = new ApplicationManifestService();
                var result = await _informationService.GetAppInformationAsync();
                var appInfo = _applicationManifestService.GetApplicationManifest();
                var myVersion = new Version(appInfo.App.Version);
                var updatedVersion = new Version(result.Entry.Version);                
                if (updatedVersion > myVersion)
                    AppUpdateButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on check app update:"+ex.Message);
            }
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            new MarketplaceDetailTask().Show();                    
        }
    }
}