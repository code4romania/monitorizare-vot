namespace VotingIrregularities.Domain.ValueObjects
{
    public enum QuestionType
    {
        MultipleOption = 0,
        SingleOption = 1,
        SingleOptionWithText = 2,
        MultipleOptionWithText = 3
    }
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
