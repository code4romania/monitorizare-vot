using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/formular")]
    public class Formular : Controller
    {
        // GET api/formulare
        [HttpGet("versiune")]
        public async Task<dynamic> Versiune(string idformular)
        {
            //DH TODO[DH] se ia din BD versiunea
            return await Task.Run(() => new { versiune= "1"});
        }

        [HttpGet]
        public async Task<IEnumerable<ModelIntrebare>> Citeste(string idformular)
        {
            return await Task.Run(() => new List<ModelIntrebare>
            {
                new ModelIntrebare
                {
                    CodSectiune = "A",
                    IdIntrebare = 12,
                    IdTipIntrebare = 1,
                    TextIntrebare = "Iti place berea? (se alege o singura optiune)",
                    RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                    {
                        new ModelRaspunsDisponibil { IdOptiune = 24, TextOptiune = "DA"},
                        new ModelRaspunsDisponibil { IdOptiune = 25, TextOptiune = "NU"},
                        new ModelRaspunsDisponibil { IdOptiune = 26, TextOptiune = "Nu stiu"}
                    }
                },

                new ModelIntrebare
                {
                    CodSectiune = "A",
                    IdIntrebare = 1,
                    IdTipIntrebare = 2,
                    TextIntrebare = "Ce tipuri de bere iti plac? (se pot alege mai multe optiuni)",
                    RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                    {
                        new ModelRaspunsDisponibil { IdOptiune = 27, TextOptiune = "Dark Island"},
                        new ModelRaspunsDisponibil { IdOptiune = 28, TextOptiune = "London Pride"},
                        new ModelRaspunsDisponibil { IdOptiune = 29, TextOptiune = "Zaganu"},
                        new ModelRaspunsDisponibil { IdOptiune = 30, TextOptiune = "Altele (specificaţi)", SeIntroduceText = true},
                    }
                }
            });
        }

    }
}
