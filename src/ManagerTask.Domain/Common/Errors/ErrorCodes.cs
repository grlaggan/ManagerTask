namespace ManagerTask.Domain.Common.Errors;

public class ErrorCodes
{
    public class Task
    {
        public const string TaskNameEmpty = "task.empty.name";
        public const string TaskNull = "task.null";
        public const string TaskDescriptionEmpty = "task.empty.description";
        public const string TaskSendTimeInvalid = "task.invalid.sendtime";
        public const string TaskTableNull = "task.null.table";
        public const string TaskSendTimePast = "task.invalid.sendtime.past";
        public const string TaskNotFound = "task.not.found";
        public const string TaskAlreadyExists = "task.already.exists";
        public const string TaskFailedRetrieveTasks = "task.failed.retrieve.tasks";
    }

    public class Table
    {
        public const string TableNameEmpty = "table.empty.name";
        public const string TableDescriptionEmpty = "table.empty.description";
        public const string TableTasksNull = "table.null.tasks";
        public const string TableAlreadyExists = "table.already.exists";
        public const string TableFailedRetrieveTables = "table.failed.retrieve.tables";
        public const string TableNotFound = "table.not.found";
        public const string TableNull = "table.null";
    }

    public class Transaction
    {
        public const string BeginFailed = "transaction.begin.failed";
        public const string CommitFailed = "transaction.commit.failed";
        public const string RollbackFailed = "transaction.rollback.failed";
    }

    public class Notification
    {
        public const string NotificationNameEmpty = "notification.empty.name";
        public const string NotificationMessageEmpty = "notification.empty.message";
        public const string NotificationTimeInvalid = "notification.invalid.time";
        public const string NotificationIsNull = "notification.null";
    }

    public class User
    {
        public const string UserChatItEmpty = "user.chat.id.empty";
        public const string UserNotFound = "user.not.found";
        public const string UserAlreadyExists = "user.already.exists";
    }
}
