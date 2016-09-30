using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta Formular ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>
    [Route("api/v1/formular")]
    public class Formular : Controller
    {
        private readonly IMediator _mediator;

        public Formular(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returneaza versiunea unui formular sub forma unui numar intreg. 
        /// Aceasta metoda trebuie apelata de fiecare data cand se afiseaza un formular in aplicatie. 
        /// Daca id-ul returnat difera de cel din aplicatie, atunci trebuie incarcat formularul printr-un apel la 
        /// <code>api//v1//formular</code>
        /// </summary>
        /// <param name="idformular">Id-ul formularului pentru care trebuie aflat versiunea</param>
        /// <returns>Returneaza un obiect care are proprietatea de tip int, versiune</returns>
        [HttpGet("versiune")]
        public async Task<dynamic> Versiune(string idformular)
        {
            var versiune = await _mediator.SendAsync(new ModelFormular.VersiuneQuery {CodFormular = idformular});

            return new {versiune};
        }


        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="idformular">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns>Returneaza o structura pe baza careia se poate genera un formular pentru observatori</returns>
        [HttpGet]
        public async Task<IEnumerable<ModelSectiune>> Citeste(string idformular)
        {
            return await Task.FromResult(new List<ModelSectiune>
            {
                new ModelSectiune
                {
                    CodSectiune = "A",
                    Intrebari = new List<ModelIntrebare>
                    {
                        new ModelIntrebare
                        {
                            IdIntrebare = 1,
                            IdTipIntrebare = 0,
                            TextIntrebare = "Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                            RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                            {
                                new ModelRaspunsDisponibil { IdOptiune = 27, TextOptiune = "Dark Island"},
                                new ModelRaspunsDisponibil { IdOptiune = 28, TextOptiune = "London Pride"},
                                new ModelRaspunsDisponibil { IdOptiune = 29, TextOptiune = "Zaganu"},
                            }
                        },
                        new ModelIntrebare
                        {
                            IdIntrebare = 12,
                            IdTipIntrebare = 1,
                            TextIntrebare = "Iti place berea? (se alege o singura optiune selectabila)",
                            RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                            {
                                new ModelRaspunsDisponibil { IdOptiune = 24, TextOptiune = "DA"},
                                new ModelRaspunsDisponibil { IdOptiune = 25, TextOptiune = "NU"},
                                new ModelRaspunsDisponibil { IdOptiune = 26, TextOptiune = "Nu stiu"}
                            }
                        }
                    }
                },
                 new ModelSectiune
                {
                    CodSectiune = "A",
                    Intrebari = new List<ModelIntrebare>
                    {


                        new ModelIntrebare
                        {
                            IdIntrebare = 1,
                            IdTipIntrebare = 2,
                            TextIntrebare = "Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                            RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                            {
                                new ModelRaspunsDisponibil { IdOptiune = 31, TextOptiune = "Transmisia manualã"},
                                new ModelRaspunsDisponibil { IdOptiune = 32, TextOptiune = "Transmisia automatã"},
                                new ModelRaspunsDisponibil { IdOptiune = 33, TextOptiune = "Transmisia non-sincron"},
                                new ModelRaspunsDisponibil { IdOptiune = 34, TextOptiune = "Altele (specificaţi)", SeIntroduceText = true},
                            }
                        },

                        new ModelIntrebare
                        {
                            IdIntrebare = 1,
                            IdTipIntrebare = 3,
                            TextIntrebare = "Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni)",
                            RaspunsuriDisponibile = new List<ModelRaspunsDisponibil>
                            {
                                new ModelRaspunsDisponibil { IdOptiune = 27, TextOptiune = "Metrou"},
                                new ModelRaspunsDisponibil { IdOptiune = 28, TextOptiune = "Tramvai"},
                                new ModelRaspunsDisponibil { IdOptiune = 29, TextOptiune = "Autobuz"},
                                new ModelRaspunsDisponibil { IdOptiune = 30, TextOptiune = "Altele (specificaţi)", SeIntroduceText = true},
                            }
                        }
                    }
            }
            });
        }

    }
}
