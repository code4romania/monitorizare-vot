namespace VotingIrregularities.Domain.Models
{
    public partial class ObserverDevice
    {
        public int ObserverDeviceId { get; set; }
        public int ObserverId { get; set; }
        public string UniqueIdentifier { get; set; }

        public virtual Observer ObserverNavigationId { get; set; }
    }
}
