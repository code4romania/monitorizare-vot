using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VoteMonitor.Api.Tests")]
namespace VoteMonitor.Api;

public static class ImportHelper
{
    public static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(Answer.Controllers.AnswersController).GetTypeInfo().Assembly;
        yield return typeof(Auth.Controllers.AuthorizationV1Controller).GetTypeInfo().Assembly;
        yield return typeof(County.Controllers.CountyController).GetTypeInfo().Assembly;
        yield return typeof(DataExport.Controllers.DataExportController).GetTypeInfo().Assembly;
        yield return typeof(Form.Controllers.FormController).GetTypeInfo().Assembly;
        yield return typeof(Location.Controllers.PollingStationController).GetTypeInfo().Assembly;
        yield return typeof(Note.Controllers.NoteController).GetTypeInfo().Assembly;
        yield return typeof(Notification.Controllers.NotificationController).GetTypeInfo().Assembly;
        yield return typeof(Observer.Controllers.ObserverController).GetTypeInfo().Assembly;
        yield return typeof(Statistics.Controllers.StatisticsController).GetTypeInfo().Assembly;
        yield return typeof(PollingStation.Controllers.PollingStationController).GetTypeInfo().Assembly;
        yield return typeof(PollingStation.Controllers.PollingStationInfoController).GetTypeInfo().Assembly;
        yield return typeof(Core.Handlers.UploadFileHandler).GetTypeInfo().Assembly;
        yield return typeof(Core.Handlers.UploadFileHandler).GetTypeInfo().Assembly;
        yield return typeof(Ngo.Controllers.NgoAdminController).GetTypeInfo().Assembly;
    }
}
