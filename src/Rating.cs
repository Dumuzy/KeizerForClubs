using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwiUtils;

namespace KeizerForClubs
{
    internal class Rating
    {
        /// <summary> Returns the new rating number and the expected Result from 0 to 1.</summary>
        /// <param name="resultFrom0to1"></param>
        /// <param name="oldRating"></param>
        /// <param name="opponentsRating"></param>
        /// <param name="k"></param>
        /// <returns>(newRating, expectedScoreFrom0to1, ratingChange)</returns>
        public static (double, double, double) GetNewRating(double resultFrom0to1, double oldRating, double opponentsRating, double k)
        {
            var expectedScore = (oldRating - opponentsRating) / (4 * C);
            if (expectedScore > 0.5)
                expectedScore = 0.5;
            else if (expectedScore < -0.5)
                expectedScore = -0.5;

            var realScore = resultFrom0to1 - 0.5;
            var newRating = oldRating + k * (realScore - expectedScore);
            return (newRating, expectedScore + 0.5, newRating - oldRating);
        }

        /// <summary> Berechnet näherungsweise die Performance eines Spielers in einem Turnier. </summary>
        /// <param name="gegnerRatings">Ein Array, das die DWZ der Gegner enthält.</param>
        /// <param name="ergebnisse">Ein Array, das die ergebnisse gegnen die Gegner enthält, jeweils 1, 0.5 oder 0.</param>
        /// <remarks>Von da:https://de.wikipedia.org/wiki/Elo-Zahl.  Diese näherungsweise Performance weicht z.T. 
        /// deutlich ab von der Performance, die auf der BSV-Seite angegeben ist, habe bis zu 30 Punkten gesehen. </remarks>
        public static int? CalcPerformance(IList<int> gegnerRatings, IList<double> ergebnisse)
        {
            if (gegnerRatings.Count != ergebnisse.Count)
                throw new ArgumentException("Die Länge der Arrays muss gleich sein.");
            int? leistung = null;
            int n = gegnerRatings.Count;
            if (n != 0)
            {
                double summeRatings = 0, summeErgebnisse = 0;
                for (int i = 0; i < n; i++)
                {
                    summeRatings += gegnerRatings[i];
                    summeErgebnisse += ergebnisse[i];
                }

                leistung = CalcPerformance(summeRatings, summeErgebnisse, n);
            }
            return leistung;
        }

        /// <summary> Berechnet näherungsweise die Performance eines Spielers in einem Turnier. </summary>
        /// <param name="ratingsSum">Summe  der Ratings der Gegner des Spielers.</param>
        /// <param name="resultSum">Summe der erspielten Punkte des Spielers. (Sieg: 1, Remis:0.5).</param>
        public static int? CalcPerformance(double ratingsSum, double resultSum, int countGames)
        {
            int? leistung = null;
            if (countGames > 0)
            {
                double durchschnittRating = ratingsSum / countGames, durchschnittErg = resultSum / countGames;
                leistung = (int)(durchschnittRating + 800 * (durchschnittErg - 0.5));
            }
            return leistung;
        }

        public static void TestPerf()
        {
            // Testwerte laut BSV am 25.1.25, AWa, AWi, MEr
            TestPerf1("1485 1498 1538 1596 1648", "1 1 1 0.5 0", 1712);
            TestPerf1("1461 1479 1533 1624 1708", "1 1 1 0.5 1", 1878);
            TestPerf1("1615 1766 1794 2070", "0.5 1 0.5 0", 1837);

        }

        private static void TestPerf1(string ratings, string ergs, int soll)
        {
            var dwz1 = ratings.Split().Select(d => Helper.ToInt(d)).ToLi();
            var erg1 = ergs.Split().Select(d => Helper.ToDouble(d)).ToLi();
            var p = CalcPerformance(dwz1, erg1);   // 1759
            if ((int)p != soll)
                ExLogger.Instance.LogInfo($"p={p}  soll={soll}");
        }


        const int C = 200;

    }
}
