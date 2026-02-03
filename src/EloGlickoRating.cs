using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AwiUtils;

/// <summary> A bit of a mixture between Elo and Glicko. </summary>
public class EloGlickoRating
{

    public EloGlickoRating(double rating, double deviation) :
        this($"{rating}~{deviation}")    { }

    public EloGlickoRating(string dbString)
    {
        var parts = dbString.Split('~').ToLi();
        if (parts.Count > 0 && parts[0] != "")
            Rating = Helper.ToDouble(parts[0]);
        else
            Rating = 1000;

        if (parts.Count > 1)
            Deviation = Helper.ToDouble(parts[1]);
        else
            Deviation = MaxDeviation;

        if (parts.Count > 2)
            LastUpdate = DateTime.ParseExact(parts[2], LastUpdateDbFormat, CultureInfo.InvariantCulture);
        else
            LastUpdate = DateTime.Now;
    }

    public double Rating { get; private set; }
    public double Deviation { get; private set; }
    public double LastChange { get; private set; }

    public void SetNewRatingAndDeviation(double resultFrom0to1, double oppoRating)
    {
        (Rating, Deviation, LastChange) = GetNewRatingAndDeviation(resultFrom0to1, Rating, oppoRating,
            Deviation, (DateTime.Now - LastUpdate).TotalDays);
        LastUpdate = DateTime.Now;
    }

    DateTime LastUpdate;

    /// <summary> Returns the new rating number and teh expected Result from 0 to 1.</summary>
    /// <param name="resultFrom0to1"></param>
    /// <param name="oldRating"></param>
    /// <param name="opponentsRating"></param>
    /// <param name="k"></param>
    /// <returns>(newRating, expectedScoreFrom0to1, ratingChange)</returns>
    static (double, double, double) GetNewRating(double resultFrom0to1, double oldRating, double opponentsRating, double k)
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

    public static void Test()
    {
        var r = new EloGlickoRating("1500~200~20220101");

        for (int j = 1; j < 26; j += 4)
        {
            ExLogger.Instance.LogInfo($"ELOGLICKO ===== weeks={j} r={r}");
            r = new EloGlickoRating("1500~200~20220101");
            for (int i = 0; i <= 260; ++i)
            {
                r.SetNewRatingAndDeviation(0.5, 1800);
                r.LastUpdate = DateTime.Now - TimeSpan.FromDays(7 * j);
                if (i % 26 == 0)
                    ExLogger.Instance.LogInfo($"ELOGLICKO i={i} weeks={j} r={r}");
            }
        }
    }

    static (double, double, double) GetNewRatingAndDeviation(double resultFrom0to1, double oldRating, double oppoRating,
            double deviation, double daysSinceLastUpdate)
    {
        (var nr, var es, var dr) = GetNewRating(resultFrom0to1, oldRating, oppoRating, deviation);
        var nd = GetNewDeviationByGameAndTime(deviation, daysSinceLastUpdate, es);
        return (nr, nd, dr);
    }

    const int C = 200, MaxDeviation = 350;
    const double q = 0.0057565;
    // I assumed that a players RD should be at a new players deviation after 2 years. 
    // Which are around 700 days. Which means that for me one period are 7 days.
    // BUT it's came out that this leads to a RD which feels quite too high after 20 games, with 
    // one game every 5 days or so. (T360)
    // Also hab ich das versucht, so anzupassen, daß man mit einem Spiel alle 7 Tage auf eine RD
    // von 20-30 kommt. Weil der k-Faktor im ELO == 20 ist für die meisten Erwachsenen.
    // Geht aber nicht, egal wie hoch man die periodInDays wählt. Ich setzt die auf 210, was heißt, 
    // daß man ohne Spiele nach ca 60 Jahren wieder Rd = 350 hat. 
    const int periodInDays = 210;

    const double fastSinkA = 104, fastSinkB = 34, fastSinkC = 7, minRd = 20;


    // This is from GLicko, I use it instead of the K-Factor. 
    static double GetNewDeviationByTime(double oldDeviation, double daysSinceLastUpdate)
    {
        var dev = Math.Min(MaxDeviation, Math.Sqrt(oldDeviation * oldDeviation + 34.6 * 34.6 *
                        daysSinceLastUpdate / periodInDays));
        return dev;
    }

    static double GetNewDeviationByGameAndTime(double oldDeviation, double daysSinceLastUpdate,
        double expectedScoreFrom0to1, double rdOpponent = 10)
    {
        var rdStar = GetNewDeviationByTime(oldDeviation, daysSinceLastUpdate);
        var g = GFaktor(rdOpponent);
        var oneByDQuadrat = q * q * g * g * expectedScoreFrom0to1 * (1 - expectedScoreFrom0to1);
        var rd = 1.0 / Math.Sqrt(1 / (rdStar * rdStar) + oneByDQuadrat);


        // Spezielle Anpassungen, damit RD schneller gegen kleinere Werte konvergiert. 
        // Hiermit und periodInDays = 210 konvergiert RD gg 101/68/37/24/21, wenn man
        // alle 25/17/13/fünf/eine Woche
        // genau ein Spiel macht und gg 20, wenn man jeden Tag ein Spiel macht.
        // Und das nach ca. 30-50 Spielen. Das ist erstmal recht vernünftig, scheint mir. 
        double dRd = 0;
        if (rd < fastSinkA && rd >= fastSinkB)
            dRd = fastSinkC * (fastSinkA - rd) / (fastSinkA - fastSinkB);
        else if (rd < fastSinkB && rd >= minRd)
            dRd = fastSinkC * (rd - minRd) / (fastSinkB - minRd);
        rd -= dRd;
        if (rd < minRd)
            rd = minRd;

        return rd;
    }

    static double GFaktor(double rdOpponent) =>
         1 / (Math.Sqrt(1 + rdOpponent * rdOpponent * q * q * 3 / (3.1415 * 3.1415)));

    const string LastUpdateDbFormat = "yyyyMMdd";
    public override string ToString() => ToDbString();
    public string ToDbString() => $"{Rating:####}~{Deviation:###}~" + LastUpdate.ToString(LastUpdateDbFormat);
}
