namespace Churchee.Module.Tenancy.Areas.Configuration.Churches.Edit
{
    public class EditChurchInputModel
    {
        public EditChurchInputModel()
        {

        }
        public EditChurchInputModel(int? charityNumber, IEnumerable<string> domains)
        {
            CharityNumber = charityNumber;
            Domains = domains.ToList();
        }

        public int? CharityNumber { get; set; }

        public List<string> Domains { get; set; }
    }
}
