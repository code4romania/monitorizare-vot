namespace VotingIrregularities.Domain.ValueObjects
{
    public struct QuestionTypeEnum
    {
        /// <summary>
        /// (0) se pot alege optiuni multiple
        /// </summary>
        public static int MultipleOptions = 0;

        /// <summary>
        /// (1) se alege o singura optiune selectabila
        /// </summary>
        public static int SingleOption = 1;

        /// <summary>
        /// (2) se poate alege O singura optiune selectabila + text pe O singura optiune
        /// </summary>
        public static int SingleOptionWithText = 2;

        /// <summary>
        /// (3) se pot alege mai multe optiuni + text pe o singura optiune
        /// </summary>
        public static int MultipleOptionsWithText = 3;
    }
}
