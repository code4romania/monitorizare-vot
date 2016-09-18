using System.Collections.Generic;
using System.Threading.Tasks;
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
            //DH TODO[DH] se ia din BD versiunea
            return await Task.Run(() => new { versiune = 1 });
        }

        /// <summary>
        /// Se apeleaza aceast metoda cand observatorul salveaza informatiile legate de ora sosirii. ora plecarii, zona urbana, info despre presedintele BESV.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare si codul formularului.
        /// </summary>
        /// <param name="formular">Datele despre header-ul unui formular</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task Inregistreaza(ModelFormular formular)
        {
            // TODO[DH] se salveaza efectiv
            await Task.Delay(0);
        }

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="idformular">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns>Returneaza o structura pe baza careia se poate genera un formular pentru observatori</returns>
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
