using System.Collections.Generic;
using TaskAdministratorUWP.Models;
using TaskAdministratorUWP.Services;
using Windows.UI.Xaml.Controls;
using TaskAdministratorUWP.Pages;
using System.Threading.Tasks;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaskAdministratorUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            GetTasksFromAPI();
            GetUsersFromAPI();
            GetAssignmentsFromAPI();
            GetUsersForAssigmentsBox();
        }        

        public async void SetTaskDetails(object sender, ItemClickEventArgs e)
        {
            var task = (TasksClient)e.ClickedItem;

            List<UsersClient> responsablesList = new List<UsersClient>();
            var responsables = await GetResponsables(task);

            foreach (var responsable in responsables)
            {
                responsablesList.Add(responsable);
            }

            var taskDetails = new TaskClientDetails()
            {
                TaskID = task.TaskID,
                Title = task.Title,
                BeginDateTime = task.BeginDateTime,
                DeadlineDateTime = task.DeadlineDateTime,
                Requirements = task.Requirements,
                Responsables = responsablesList
            };

            this.Frame.Navigate(typeof(TaskDetails), taskDetails);
        }

        public async void GetTasksFromAPI()
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<TasksClient> tasksFromAPI = await client.GetDataFromAPI<TasksClient>("Tasks");

            foreach (var task in tasksFromAPI)
            {
                TaskList.Items.Add(new TasksClient()
                {
                    TaskID = task.TaskID,
                    Title = task.Title,
                    BeginDateTime = task.BeginDateTime,
                    DeadlineDateTime = task.DeadlineDateTime,
                    Requirements = task.Requirements,
                    Responsables = task.Responsables
                });

                TaskList.ItemClick += SetTaskDetails;
            }
        }

        public async void GetUsersFromAPI()
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<UsersClient> usersFromAPI = await client.GetDataFromAPI<UsersClient>("Users");

            foreach (var user in usersFromAPI)
            {
                new UsersClient()
                {
                    UserID = user.UserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
            }
        }

        public async void GetAssignmentsFromAPI()
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<AssignmentsClient> assignmentsFromAPI = await client.GetDataFromAPI<AssignmentsClient>("Assignments");

            foreach (var assignment in assignmentsFromAPI)
            {
                new AssignmentsClient()
                {
                    TaskID = assignment.TaskID,
                    UserID = assignment.UserID
                };
            }
        }

        private async Task<List<UsersClient>> GetResponsables(TasksClient task)
        {
            RequestHandler client = new RequestHandler();

            List<UsersClient> usersList = new List<UsersClient>();

            IEnumerable<UsersClient> users = await client.GetDataFromAPI<UsersClient>("Users");
            IEnumerable<AssignmentsClient> assignments = await client.GetDataFromAPI<AssignmentsClient>("Assignments");

            foreach (var user in users)
            {
                foreach (var assignment in assignments)
                {
                    if (user.UserID == assignment.UserID && task.TaskID == assignment.TaskID)
                    {
                        usersList.Add(user);
                    }
                }
            }

            return usersList;
        }
        private void OpenUsersPopup(object sender, RoutedEventArgs e)
        {
            if (!PopupUsers.IsOpen)
            {
                PopupUsers.IsOpen = true;
            }
        }
        private void ClosePopupWindow(object sender, RoutedEventArgs e)
        {
            if (PopupUsers.IsOpen)
            {
                PopupUsers.IsOpen = false;
            }
        }
        private async void GetUsersForAssigmentsBox()
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<UsersClient> usersFromAPI = await client.GetDataFromAPI<UsersClient>("Users");

            foreach (var user in usersFromAPI)
            {
                AssingmentsBox.Items.Add(user);
            }
        }

        private async void GetTasksForUserAsync()
        {
            var tasksForUser = new System.Text.StringBuilder();

            RequestHandler client = new RequestHandler();

            UsersClient user = (UsersClient)this.AssingmentsBox.SelectedItem;

            IEnumerable<AssignmentsClient> assignments = await client.GetDataFromAPI<AssignmentsClient>("Assignments");
            IEnumerable<TasksClient> tasks = await client.GetDataFromAPI<TasksClient>("Tasks");

            foreach (var assignment in assignments)
            {
                foreach (var task in tasks)
                {
                    if (assignment.TaskID == task.TaskID && assignment.UserID == user.UserID)
                    {
                        ListOfTasksTextBlock.Text = task.Title + "\n";
                    }
                }
            }
        }

        private async void OpenPopupTaskList(object sender, RoutedEventArgs e)
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<AssignmentsClient> assignments = await client.GetDataFromAPI<AssignmentsClient>("Assignments");
            IEnumerable<TasksClient> tasks = await client.GetDataFromAPI<TasksClient>("Tasks");

            var tasksForUser = new System.Text.StringBuilder();

            UsersClient user = (UsersClient)this.AssingmentsBox.SelectedItem;

            if (PopupUsers.IsOpen)
            {
                PopupUsers.IsOpen = false;
            }

            foreach (var assignment in assignments)
            {
                foreach (var task in tasks)
                {
                    if (assignment.TaskID == task.TaskID && assignment.UserID == user.UserID)
                    {
                        tasksForUser.Append(task.Title + "\n");
                    }
                }
            }

            UserNameTextBlock.Text = user.FirstName + "'s task(s)";
            ListOfTasksTextBlock.Text = tasksForUser.ToString();

            if (!PopupListOfTasks.IsOpen)
            {
                PopupListOfTasks.IsOpen = true;
            }
        }
        private void ClosePopupTaskList(object sender, RoutedEventArgs e)
        {
            if (PopupListOfTasks.IsOpen)
            {
                PopupListOfTasks.IsOpen = false;
            }
        }
    }
}
