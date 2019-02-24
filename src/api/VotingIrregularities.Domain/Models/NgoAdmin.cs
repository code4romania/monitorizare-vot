namespace VotingIrregularities.Domain.Models
{
    public partial class NgoAdmin
    {
        public int Id { get; set; }
        public int IdNgo { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public virtual Ngo Ngo { get; set; }
    }
}
