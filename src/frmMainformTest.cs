using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwiUtils;

namespace KeizerForClubs
{
    public partial class frmMainform
    {
        void ExecTestScript(Li<string> lines)
        {
            lines = lines.Select(li => li.Trim().ToLowerInvariant())
                            .Where(li => !string.IsNullOrEmpty(li) &&
                                !li.StartsWith("#")).ToLi();
            foreach (var line in lines)
                ExecTestLine(line);
        }

        void ExecTestLine(string line)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Debug.WriteLine($"ExTeLi Start:{line}");
            var parts = line.SplitToWords().ToLi();
            int n = Helper.ToInt(parts[0]);
            int nPlayers = n;
            if (n == 0)
                n = 1;
            else
            {
                parts.RemoveAt(0);
                if (parts[0] == "create-players")
                    n = 1;
            }
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < parts.Count; ++j)
                {
                    if (parts[j] == "create-players")
                        ExecTestCreatePlayers(nPlayers);
                    else if (parts[j] == "create-results")
                        ExecTestCreateResults();
                    else if (parts[j] == "create-round")
                        MnuPairingNextRoundClick(this, null);
                    else if (parts[j] == "delete-all-players")
                        ExecTestDeleteAllPlayers();
                    else if (parts[j] == "delete-all-rounds")
                        ExecTestDeleteAllRounds();
                    else if (parts[j] == "delete-round")
                        ExecTestDeleteRound();
                }

            sw.Stop();
            Debug.WriteLine($"ExTeLi needed sec {sw.ElapsedMilliseconds / 1000} End:{line}");
        }

        void ExecTestCreateResults()
        {
            for (int j = 0; j < grdPairings.RowCount; ++j)
            {
                int pid1 = Convert.ToInt16(this.grdPairings.Rows[j].Cells[1].Value);
                int pid2 = Convert.ToInt16(this.grdPairings.Rows[j].Cells[4].Value);
                int rat1 = GetPlayerRating(pid1);
                int rat2 = GetPlayerRating(pid2);
                SqliteInterface.Results gameResult = rat1 > rat2 ? SqliteInterface.Results.WinWhite :
                        rat2 > rat1 + 100 ? SqliteInterface.Results.WinBlack : SqliteInterface.Results.Draw;
                db.UpdPairingResult((int)Convert.ToInt16(this.numRoundSelect.Value), pid1, pid2, gameResult);
            }
        }

        void ExecTestDeleteAllPlayers()
        {
            if (db.GetMaxRound() == 0)
            {
                db.DeleteAllPlayersTa();
                db.ResetPlayerBaseIdTa();
                LoadPlayerlist();
                dictPlayerRating.Clear();
            }
        }

        void ExecTestDeleteAllRounds()
        {
            for (int maxRound = db.GetMaxRound(); maxRound > 0; --maxRound)
            {
                db.DelPairings(maxRound);
                db.DelCurrentStoredTablesWHeader(maxRound);
                this.numRoundSelect.Value = (Decimal)(maxRound - 1);
                this.LoadPairingList();
            }
            ApplyPlayerStateTexte();
        }

        void ExecTestDeleteRound()
        {
            int maxRound = db.GetMaxRound();
            db.DelPairings(maxRound);
            db.DelCurrentStoredTablesWHeader(maxRound);
            this.numRoundSelect.Value = (Decimal)(maxRound - 1);
            this.LoadPairingList();
            ApplyPlayerStateTexte();
        }

        int GetPlayerRating(int playerId)
        {
            if (!dictPlayerRating.TryGetValue(playerId, out int rating))
            {
                var player = db.GetPlayer($"WHERE id = {playerId}", "", 0);
                rating = dictPlayerRating[playerId] = player.Rating;
            }
            return rating;
        }
        Dictionary<int, int> dictPlayerRating = new Dictionary<int, int>();

        void ExecTestCreatePlayers(int count)
        {
            var playerCsv = @"Name;Rating
Pancevski, Filip; 2446
Colovic, Aleksandar; 2416
Blodig, Vincent; 2021
Giannino, Domenico; 2018
Jermann, Frank Dr.; 1995
Ebeling, Jens; 1993
Struck, Christian; 1983
Wachtel, Arthur; 1977
Jussim, Maxim; 1967
Weisheit, Erik; 1944
Schindler, Alexander; 1942
Vuckovic, Aleksandar; 2342
Vuckovic, Zarko; 2339
Lavrinenkov, Vadim; 2296
Michaelis, Martin; 2276
Weller, Uli; 2259
Duong, Quang; Bach
Brückner, Thomas; 2166
Abdic, Adnan; 2166
Balic, Adnan; 2093
Blodig, Stefan; 2058
Vuckovic, Robert; 2058
Kampen, Hans; 2037
Beesk, Kevin; 2026
Petrov, Nikita; 2571
Djukic, Nikola; 2511
Draskovic, Luka; 2504
Reif, Hermann; 1866
Mayr, Guenther; 1849
Steinberger, Michael; 1846
Rempel, Alexander; 1818
Schrimpf, Michael; 1818
Winter, Lukas; 1805
Göttler, Raphael; 1783
Rempel, Alexander; 1672
Wachtel, Alexandra; 1650
Dudnik, Daniel; 1647
Fritscher, Leonhard; 1628
Klein, Theo; 1623
Herb, Maximilian; 1616
Özkul, Felix; 1571
Thoma, Gunter; 1555
Simic, Dejan; 1520
Reimann, Denis; 1517
Hamkar, Behzad; 1499
Herbst, Elias; 1499
Dao, Andy Thang; 1495
Kleuster, Anton; 1490
Blodig, Samuel; 1762
Thorwarth, Paul; 1748
Döbel, Maximilian; 1747
Brugger, Dominic; 1732
Schütze, Jonas; 1728
Wicker, Andreas; 1722
Dommnich, Alexander; 1698
Vuckovic, Katarina; 1697
Kemmerling, Thomas; 1689
Xaver, Xolotl;1687
Yotov, Vasil; 1484
Hofmann, Werner; 1417
Li, Junchi; 1402
Nikolic, Gabriel; 1380
Harms, Carsten; 1371
Dao, Le; Thien
Schrumpf, Dominik; 1321
Marsagischwili, Leo; 1321
Zirngibl, Erich; 1310
Yetim, Orhan; 1237
Gucz, Leonard; 1218
Wachtel, Barbara; 1185
Mairoser, Thomas; 1174
Slepnov, Mykyta; 1137
Brugger, Leonas; 1100
Vuckovic, Marianne; 1016
Weidenbacher, Artur; 1012
Blodig, Antonia; 948
Kempter, Vincent Magnus; 944
Schrimpf, Felix; 932
Jung, Micha; 903
Shatrava, Alexander; 826
Boureanu, Iuliana; 786
Kussauer, Philipp; 773
Karsten, Viktor; 754
Dommnich, Emil; 750
Vollmer, Alexander; Restp.
Mayer, Uwe; Restp.
Zibak, Ömer Ali; ---
Pisani, Lia Michelle; ---
Baur, Leo; Restp.
Hoffmann, Matti; Restp.
Scripnic, David; Restp.
Koutecky, Pius; Restp.
Feigl, Roman; Restp.
Bart, Jason; Restp.
Kokane, Tanay Nishikant; Restp.
Micklitz, Helmut; -----
Töpfer, Julian; -----
Hoffmann, Claudia; -----
Essick, Simon; -----
Fast, Maxim; -----
Wallenreiter, Marika; -----
D'Ambrosio, Matteo; -----
Brugger, Livia; Marie
Langner, Matteo; -----
Dudka, Stepan; -----
Diwert, Eduard; -----
Diwert, Philipp; -----
Ivanova, Zhasmin; -----
Lederman, Max; -----
Koutecky, Leopold; -----
Oberländer, Markus; -----
Schellenberg, Philipp; -----
Stojicic, Lazar; -----
Alvino, Marco; -----
Alvino, Maria; -----
Schulze, Korbinian; -----
Ahmetaj, Eduard; -----
Lefter, Emanuel; -----
Züllich, Jonas; -----
Schrumpf, Benjamin; -----
Menzel, Linus; -----
Sobol, Matei-Jon; -----
Halmaghi, Jan; ----".SplitToLines().Take(count + 1).ToLi();
            ImportPlayers(playerCsv);
        }


    }
}
