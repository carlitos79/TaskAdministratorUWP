using System;
using System.Collections.Generic;

namespace TaskAdministratorUWP.Models
{
    class TasksClient
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime DeadlineDateTime { get; set; }
        public string Requirements { get; set; }
        public List<UsersClient> Responsables { get; set; }
    }
}
