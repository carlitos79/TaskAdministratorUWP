using System;
using System.Collections.Generic;
using System.Linq;
using TaskAdministratorUWP.Models;
using TaskAdministratorUWP.Services;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskAdministratorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TaskDetails : Page
    {
        private TaskClientDetails task;

        public TaskDetails()
        {
            this.InitializeComponent();
            GetUsersForAssigmentsBox();
            GetUsersForAssigmentsBoxDelete();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                this.Frame.Navigate(typeof(MainPage));
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                task = (TaskClientDetails)e.Parameter;

                TaskDetailTitle.Text = task.Title;
                TaskDetailBegin.Text = task.BeginDateTime.ToString();
                TaskDetailDeadline.Text = task.DeadlineDateTime.ToString();
                TaskDetailRequirement.Text = task.Requirements;
                TaskDetailResponsable.Text = ShowResponsables(task.Responsables);
            }
            base.OnNavigatedTo(e);
        }

        private string ShowResponsables(List<UsersClient> users)
        {
            var responsable = new System.Text.StringBuilder();

            if (users.Any())
            {
                foreach (var user in users)
                {
                    responsable.Append(user.FirstName + " " + user.LastName + "\n");
                }

                return responsable.ToString();
            }
            else
            {
                return "None";
            }            
        }

        private void OpenPopupWindow(object sender, RoutedEventArgs e)
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

        private void OpenPopupWindowDelete(object sender, RoutedEventArgs e)
        {
            if (!PopupUsersDelete.IsOpen)
            {
                PopupUsersDelete.IsOpen = true;
            }
        }

        private void ClosePopupWindowDelete(object sender, RoutedEventArgs e)
        {
            if (PopupUsersDelete.IsOpen)
            {
                PopupUsersDelete.IsOpen = false;
            }
        }

        private void ClosePopupConfirmation(object sender, RoutedEventArgs e)
        {
            if (PopupConfirmation.IsOpen)
            {
                PopupConfirmation.IsOpen = false;
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

        private async void GetUsersForAssigmentsBoxDelete()
        {
            RequestHandler client = new RequestHandler();
            IEnumerable<UsersClient> usersFromAPI = await client.GetDataFromAPI<UsersClient>("Users");

            foreach (var user in usersFromAPI)
            {
                foreach (var responsable in task.Responsables)
                {
                    if (user.UserID == responsable.UserID)
                    {
                        AssingmentsBoxDelete.Items.Add(user);
                    }
                }             
            }            
        }

        private async void ConfirmAcceptTaskAndPostAsync(object sender, RoutedEventArgs e)
        {
            UsersClient user = (UsersClient)this.AssingmentsBox.SelectedItem;

            if (user == null)
            {
                return;
            }

            var assignmentToPost = new AssignmentsClient()
            {
                TaskID = task.TaskID,
                UserID = user.UserID
            };

            RequestHandler client = new RequestHandler();
            await client.PostDataToAPI(assignmentToPost);

            if (PopupUsers.IsOpen)
            {
                PopupUsers.IsOpen = false;
            }

            this.Frame.Navigate(typeof(MainPage));

            if (!PopupConfirmation.IsOpen)
            {
                PopupConfirmation.IsOpen = true;
                TaskConfirmTextBlock.Text = "The task " + task.Title + "\n" + "has been accepted by " + user.FirstName;
            }
        }

        private async void DeclineTaskAndPostAsync(object sender, RoutedEventArgs e)
        {
            UsersClient user = (UsersClient)this.AssingmentsBoxDelete.SelectedItem;

            if (user == null)
            {
                return;
            }

            RequestHandler client = new RequestHandler();
            await client.DeleteDataToAPI(task.TaskID + "/" + user.UserID);

            if (PopupUsersDelete.IsOpen)
            {
                PopupUsersDelete.IsOpen = false;
            }

            this.Frame.Navigate(typeof(MainPage));

            if (!PopupConfirmation.IsOpen)
            {
                PopupConfirmation.IsOpen = true;
                TaskConfirmTextBlock.Text = "The task " + task.Title + "\n" + "has been declined by " + user.FirstName;
            }
        }
    }
}