using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Domain.ValueObjects
{
    public struct TipIntrebareEnum
    {
        /// <summary>
        /// (0) se pot alege optiuni multiple
        /// </summary>
        public static int OptiuniMultiple = 0;

        /// <summary>
        /// (1) se alege o singura optiune selectabila
        /// </summary>
        public static int OSinguraOptiune = 1;

        /// <summary>
        /// (2) se poate alege O singura optiune selectabila + text pe O singura optiune
        /// </summary>
        public static int OSinguraOptiuneCuText = 2;

        /// <summary>
        /// (3) se pot alege mai multe optiuni + text pe o singura optiune
        /// </summary>
        public static int OptiuniMultipleCuText = 3;




    }
}
