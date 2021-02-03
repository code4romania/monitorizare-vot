using System;

namespace VoteMonitor.Api.Observer.Utils
{
    public class RandomNumberGenerator
    {
        public static string Generate(int digits)
        {
            var random = new Random();
            var number = "";
            for (var i = 1; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }

        public static string GenerateWithPadding(int digits, string prefix)
        {
            var random = new Random();
            var number = prefix;
            for (var i = 1 + prefix.Length; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }
    }
}