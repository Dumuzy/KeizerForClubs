using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwiUtils;

namespace KeizerForClubs
{
    abstract public class TimeBonus
    {
        public static TimeBonus Create(string configText)
        {
            TimeBonus res = null;
            if (!string.IsNullOrWhiteSpace(configText) && !configText.StartsWith("#"))
            {
                var dict = GetOptionsDict(CleanOptionsTimeBonusText(configText));
                switch (dict["curve"])
                {
                    case "A": // fallthrough
                    case "Sigmoid": res = new TimeBonusSigmoid(dict); break;
                }
            }
            return res;
        }

        public static string CleanOptionsTimeBonusText(string configText)
        {
            if (!string.IsNullOrWhiteSpace(configText) && !configText.StartsWith("#"))
            {
                var parts = configText.Split(',').Select(s => s.Trim()).Where(z => !string.IsNullOrEmpty(z)).ToLi();
                var cats = parts.Select(p => p.Split('=', 2)).Where(q => q.Length == 2).ToLi();
                var cats2 = cats.Select(p => Regex.Replace(p[0], "[^-\\._A-Za-z0-9]", "") + "=" + p[1].Trim())
                        .Where(q => !q.StartsWith("=")).ToLi();
                configText = string.Join(", ", cats2);
            }
            return configText;
        }

        virtual public bool IsValid { get { return false; } }
        abstract public Tuple<string, string> GetPlayerTimes(double ratingW, double ratingB);

        protected TimeBonus(Dictionary<string, string> optionsDict)
        {
            try
            {
                MinimumTime = Helper.ToInt(optionsDict["Min"]);
                SumTime = Helper.ToInt(optionsDict["Sum"]);
                Faktor = Helper.ToDouble(optionsDict["Fak"]);
            }
            catch (Exception ex)
            {
                ExLogger.Instance.LogException("TimeBonus.kk", ex);
                MinimumTime = SumTime = 0; Faktor = 1;
            }
        }

        protected readonly int MinimumTime, SumTime;
        protected readonly double Faktor;

        static Dictionary<string, string> GetOptionsDict(string cleanedText)
        {
            Dictionary<string, string> d = (!string.IsNullOrWhiteSpace(cleanedText) && !cleanedText.StartsWith("#")) ?
                Helper.ToDictionary3(cleanedText, ",", "=", true, StringComparer.OrdinalIgnoreCase) : new();
            return d;
        }

        protected Dictionary<string, string> optionsDict;
    }

    public class TimeBonusSigmoid : TimeBonus
    {

        public TimeBonusSigmoid(Dictionary<string, string> optionsDict) : base(optionsDict) { }
        public override bool IsValid { get { return MinimumTime >= 0 && SumTime > 2 && Faktor > 0.0001; } }

        public override Tuple<string, string> GetPlayerTimes(double ratingW, double ratingB)
        {
            int timeW = -1, timeB = -1;
            if (IsValid)
            {
                var diff = Math.Abs(ratingW - ratingB);
                var tbSum = SumTime;
                var tbMin = MinimumTime;
                var timeStronger = (int)Math.Round(tbMin + (tbSum - 2 * tbMin) / (1 + Math.Exp(Faktor * diff)), 0);
                var timeWeaker = tbSum - timeStronger;
                timeW = ratingW >= ratingB ? timeStronger : timeWeaker;
                timeB = ratingW >= ratingB ? timeWeaker : timeStronger;
            }
            else
                ExLogger.Instance.LogError("TimeBonus.GetPlayerTimes is Invalid.");

            return new Tuple<string, string>(timeW.ToString(), timeB.ToString());
        }
    }
}
