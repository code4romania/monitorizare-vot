using System.Collections.Generic;
using System.Linq;

namespace VoteMonitor.Api.Location.Models
{
    internal class PollingStationExcelHeader
    {
        private readonly Dictionary<string, ExcelHeaderColumn> columns = new Dictionary<string, ExcelHeaderColumn>();

        public PollingStationExcelHeader()
        {
            columns.Add(nameof(PollingStationDTO.CodJudet), new ExcelHeaderColumn(1, "Judet"));
            columns.Add(nameof(PollingStationDTO.CodSiruta), new ExcelHeaderColumn(3, "COD SIRUTA"));
            columns.Add(nameof(PollingStationDTO.NrSV), new ExcelHeaderColumn(5, "Nr SV"));
            columns.Add(nameof(PollingStationDTO.Adresa), new ExcelHeaderColumn(7, "Adresa SV"));
        }

        public ExcelHeaderColumn this[string key] { get { return columns[key]; } }
        public IEnumerable<ExcelHeaderColumn> Columns { get { return columns.Select(x => x.Value); } }
    }

    internal class ExcelHeaderColumn
    {
        public ExcelHeaderColumn(int index, string text)
        {
            Index = index;
            Name = text;
        }

        public int Index { get; private set; }
        public string Name { get; private set; }
    }
}
