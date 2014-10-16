using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace TaskiWindowsPhone7.Classes
{
    static class DBWork
    {

        public static void CreateDB()
        {
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                    Debug.WriteLine("Add DB file");
                }
            }
        }
        
        public static int AddProject(string Name,string Desc, DateTime StartDate,DateTime FinishDate,int ImgID)
        {
            Project p;
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                p = new Project();
                p.Name = Name!=""?Name:AppResources.NewProject;
                p.Description = Desc!=""?Desc:AppResources.ProjectDescription;
                p.StartDate=StartDate;
                p.FinishDate=FinishDate;
                p.ImageID = ImgID;
                context.Projects.InsertOnSubmit(p);
                context.SubmitChanges();
            }

            AddTask(p.ID, AppResources.NewTask, AppResources.NewTask, StartDate, FinishDate, 0);
            Debug.WriteLine("Add Project with Task");
            return p.ID;
        }

        public static int AddTask(int ProjectID, string Name, string Desc, DateTime StartDate, DateTime FinishDate, int State)
        {
            Task t;
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                t = new Task();
                t.Name = Name;
                t.Description = Desc;
                t.ProjectID = ProjectID;
                t.StartDate=StartDate;
                t.FinishDate=FinishDate;
                t.State=State;
                context.Tasks.InsertOnSubmit(t);
                context.SubmitChanges();
            }    
            return t.ID;
        }

        public static IList<Project> GetProjects()
        {
            IList<Project> List = null;
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                IQueryable<Project> query = from c in context.Projects select c;
                List = query.ToList();
                Debug.WriteLine("Get projects");
            }

            return List;
        }
 
        public static IList<Task> GetTasks()
        {
            IList<Task> taskList = null;
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                IQueryable<Task> query = from c in context.Tasks select c;
                taskList = query.ToList();
                Debug.WriteLine("Get tasks");
            }
  
            return taskList;
        }

        public static bool RemoveProject(int ID)
        {
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                context.Tasks.DeleteAllOnSubmit(from t in context.Tasks where t.ProjectID == ID select t);
                Project p = (from c in context.Projects where c.ID == ID select c).FirstOrDefault();
                if (p == null) return false;
                context.Projects.DeleteOnSubmit(p);
                context.SubmitChanges();
                return true;
            }
        }

        public static bool RemoveTask(int ID)
        {
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                Task p = (from c in context.Tasks where c.ID == ID select c).FirstOrDefault();
                if (p == null) return false;
                context.Tasks.DeleteOnSubmit(p);
                context.SubmitChanges();
                return true;
            }
        }

        public static bool UpdateProject(int ID, string Name="", string Description="", DateTime? StartDate=null, DateTime? FinishDate=null,int ImgID=0)
        {
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                Project p = (from el in context.Projects where el.ID==ID select el).FirstOrDefault();
                if (p==null) return false;
                if (Name != "") p.Name = Name;
                if (Description != "") p.Description = Description;
                if (StartDate != null) p.StartDate = StartDate;
                if (FinishDate != null) p.FinishDate = FinishDate;
                p.ImageID = ImgID;
                context.SubmitChanges();
            }
            return true;
        }

        public static bool UpdateTask(int ProjectID, int TaskID, string Name="", string Description="", DateTime? StartDate=null, DateTime? FinishDate=null, int State=0)
        {
            using (ProjectsDataContext context = new ProjectsDataContext(ProjectsDataContext.ConnectionString))
            {
                Task el = (from c in context.Tasks where c.ID == TaskID select c).FirstOrDefault();
                if (el == null) return false;
                if (Name != "") el.Name=Name;
                if (Description != "") el.Description=Description;
                if (StartDate != null) el.StartDate=StartDate.Value;
                if (FinishDate != null) el.FinishDate=FinishDate.Value;
                el.State = State;
                context.SubmitChanges();               
            }
            return true;
        }

        public static bool UpdateTask(Task task)
        {
            var p = (from el in PageProject.TaskCollection where el.TaskID == task.ID select el).FirstOrDefault();
            if (p == null) return false;
            PageProject.TaskCollection.Remove(p);

            //update secondary tile
            ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + task.ID));
            if (Tile != null)
            {
                StandardTileData data = new StandardTileData();
                data.Title = task.Name;
                Tile.Update(data);
            }
            Classes.DBWork.UpdateTask(
                task.ProjectID.Value, 
                task.ID, 
                task.Name, 
                task.Description, 
                task.StartDate, 
                task.FinishDate, 
                task.State.Value); //update DB
            return true;
        }

    }

    #region DataBase Code
    [Table(Name="Projects")]
    public class Project
    {
        public Project()
        {
            this.tasksRef = new EntitySet<Task>(this.OnTaskAdded, this.OnTaskRemoved);
        }


        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public int ImageID { get; set; }
        [Column]
        public DateTime? StartDate { get; set; }
        [Column]
        public DateTime? FinishDate { get; set; }

        private EntitySet<Task> tasksRef;

        [Association(Name = "FK_Projects_Tasks", Storage = "tasksRef", ThisKey = "ID", OtherKey = "ProjectID")]
        public EntitySet<Task> Tasks
        {
            get
            {
                return this.tasksRef;
            }
        }

        private void OnTaskAdded(Task task)
        {
            task.Project = this;
        }

        private void OnTaskRemoved(Task task)
        {
            task.Project = null;
        }


    }

    [Table(Name="Tasks")]
    public class Task
    {
        private Nullable<int> projectID { get; set; }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }
        [Column(Storage = "projectID", DbType = "Int")]
        public int? ProjectID { get { return projectID; } set { projectID = value; } }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public int? State { get; set; }
        [Column]
        public DateTime? StartDate { get; set; }
        [Column]
        public DateTime? FinishDate { get; set; }

        private EntityRef<Project> projectRef = new EntityRef<Project>();
        [Association(Name = "FK_Projects_Tasks", Storage = "projectRef", ThisKey = "ProjectID", OtherKey = "ID", IsForeignKey = true)]
        public Project Project
        {
            get
            {
                return this.projectRef.Entity;
            }
            set
            {
                Project previousValue = this.projectRef.Entity;
                if (((previousValue != value) || (this.projectRef.HasLoadedOrAssignedValue == false)))
                {
                    if ((previousValue != null))
                    {
                        this.projectRef.Entity = null;
                        previousValue.Tasks.Remove(this);
                    }
                    this.projectRef.Entity = value;
                    if ((value != null))
                    {
                        value.Tasks.Add(this);
                        this.projectID = value.ID;
                    }
                    else
                    {
                        this.projectID = default(Nullable<int>);
                    }
                }
            }
        }
    }

    public class ProjectsDataContext : DataContext
    {
        public static string ConnectionString = @"isostore:/db.sdf";

        public ProjectsDataContext(string ConnectionString): base(ConnectionString)
        { }

        public Table<Project> Projects
        {
            get
            {
                return this.GetTable<Project>();
            }
        }

        public Table<Task> Tasks
        {
            get
            {
                return this.GetTable<Task>();
            }
        }
    }

    #endregion
}
