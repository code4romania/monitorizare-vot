using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/sectie")]
    public class Sectie : Controller
    {

        /// <summary>
        /// Aceasta ruta permite observatorului sa caute o lista de sectii dupa judet si numarul de sectie
        /// </summary>
        /// <param name="idJudet">Observatorul va trebui sa aleaga dintr-un selectlist un id de judet (de la 1 la 40 si 41 pentru diaspora)</param>
        /// <param name="numarSectie">Se poate trece doar partial numarul sectiei</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<ModelSectie>> Cauta(int? idJudet, int? numarSectie)
        {
            return await Task.FromResult(new List<ModelSectie>
            {
                new ModelSectie
                {
                    AdresaSectie = "Seminarul Teologic Ortodox „Sfântul Simion Ştefan“, Bld. Transilvaniei (Bld. Transilvania) , Nr. 36A",
                    IdSectieDeVotare = 23,
                    Oras = "ALBA IULIA",
                    Judet = "ALBA"
                },

                new ModelSectie
                {
                    AdresaSectie = "Grădiniţa cu program normal 'Scufiţa Roşie', Str. Pricazului , Nr. 48",
                    IdSectieDeVotare = 1043,
                    Oras = "ORĂŞTIE",
                    Judet = "HUNEDOARA"
                }
            });
        }


        /// <summary>
        /// Se apeleaza aceast metoda cand observatorul salveaza informatiile legate de ora sosirii. ora plecarii, zona urbana, info despre presedintele BESV.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="dateSectie">Informatii despre sectia de votare si observatorul alocat ei</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task Inregistreaza([FromBody] ModelDateSectie dateSectie)
        {
            // TODO[DH] se salveaza efectiv
            await Task.FromResult(0);
        }
    }
}
