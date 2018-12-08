namespace VotingIrregularities.Domain.Models
{
    public partial class NgoAdmin
    {
        public int NgoAdminId { get; set; }
        public int NgoId { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public virtual Ngo NgoNavigationId { get; set; }
    }
}
