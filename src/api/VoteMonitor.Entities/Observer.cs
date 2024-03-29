namespace VoteMonitor.Entities;

public class Observer : IIdentifiableEntity
{
    public int Id { get; set; }
    public bool FromTeam { get; set; }
    public int IdNgo { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public string Pin { get; set; }
    public string MobileDeviceId { get; set; }
    public MobileDeviceIdType MobileDeviceIdType { get; set; }
    public DateTime? DeviceRegisterDate { get; set; }
    public bool IsTestObserver { get; set; }

    public ICollection<Note> Notes { get; set; } = new HashSet<Note>();
    public ICollection<NoteCorrupted> NotesCorrupted { get; set; } = new HashSet<NoteCorrupted>();
    public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
    public ICollection<AnswerCorrupted> CorruptedAnswers { get; set; } = new HashSet<AnswerCorrupted>();
    public ICollection<PollingStationInfo> PollingStationInfos { get; set; } = new HashSet<PollingStationInfo>();
    public ICollection<PollingStationInfoCorrupted> PollingStationInfosCorrupted { get; set; } = new HashSet<PollingStationInfoCorrupted>();
    public Ngo Ngo { get; set; }
    public ICollection<NotificationRecipient> Notifications { get; set; }
}
