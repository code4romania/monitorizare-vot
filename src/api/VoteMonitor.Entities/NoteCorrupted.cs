namespace VoteMonitor.Entities;

public class NoteCorrupted : IIdentifiableEntity
{
    public int Id { get; set; }
    public DateTime LastModified { get; set; }
    public int? IdQuestion { get; set; }
    public int IdObserver { get; set; }
    public string CountyCode { get; set; }
    public string MunicipalityCode { get; set; }
    public int PollingStationNumber { get; set; }
    public string Text { get; set; }

    public virtual Question Question { get; set; }
    public virtual Observer Observer { get; set; }
    public virtual ICollection<NotesAttachmentCorrupted> Attachments { get; set; }

}
