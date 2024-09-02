using System.Diagnostics;
using AwiUtils;

namespace KeizerForClubs
{
    internal class RankingCalculator
    {
        public RankingCalculator(SqliteInterface db, frmMainform mainform, bool shallUseReporting)
        {
            this.db = db;
            this.form = mainform;
            cReportingUnit = shallUseReporting ? new ReportingUnit("qwert", "qwert", db) : null;
        }

        /// <summary> Sets all KeizerPts of all games of all rounds to zero and recalculates all again. </summary>
        internal void AllPlayersAllRoundsCalculateTa()
        {
            int maxRound = db.GetMaxRound();
            Stopwatches.Start($"AllPlayersAllRoundsCalculateTa-All MaxRound={maxRound}");
            cReportingUnit?.DeleteDump();

            //Setzt alle Bewertungen aller Paarungen und alle Keizer_SumPts auf 0.
            db.UpdPairing_AllPairingsAndAllKeizerSumsResetValuesTa();
            this.AllPlayersSetInitialStartPtsTa(maxRound + 1); // Keizer_StartPts in die DB setzen.
            this.AllPlayersSetKeizerSumPtsTa();    // Keizer_SumPts in die DB setzen, hier noch Keizer_SumPts = Keizer_StartPts.
            cReportingUnit?.DebugPairingsAndStandings(0);
            // If nExtraRecursions is > 0, at the end of the calculation, that many
            // extra rounds of calculation are appended. Only for testing stuff, currently. 
            int nExtraRecursions = 0;
            int runde1Start = 1;

            var table = GetTableDbCacheForRound(maxRound - 1);
            if (table != null && table.Count > 0)
                runde1Start = maxRound - 1;


            for (int runde1 = runde1Start; runde1 <= maxRound; ++runde1)
            {
                // Debug.WriteLine($"=========  ER={runde1} ===========");
                Stopwatches.Start("AllPlayersAllRoundsCalculateTa-1");
                db.UpdPairing_AllPairingsAndAllKeizerSumsResetValuesTa();
                Stopwatches.Next("AllPlayersAllRoundsCalculateTa-2");
                bool ok = AllPlayersSetRankAndStartPtsFromDbCacheTa(runde1, table);
                table = null;
                Stopwatches.Stop("AllPlayersAllRoundsCalculateTa-2");
                if (!ok)
                {
                    Stopwatches.Next("AllPlayersAllRoundsCalculateTa-3");
                    for (int runde2 = 1; runde2 <= runde1; ++runde2)
                        this.OneRoundAllPairingsSetKeizerPtsTa(runde2, runde1);
                    Stopwatches.Next("AllPlayersAllRoundsCalculateTa-4");
                    this.AllPlayersSetKeizerSumPtsTa();
                    Stopwatches.Next("AllPlayersAllRoundsCalculateTa-5");
                    this.AllPlayersSetRankAndStartPtsTa();
                    Stopwatches.Stop("AllPlayersAllRoundsCalculateTa-5");
                }
                cReportingUnit?.DebugPairingsAndStandings(runde1);
                if (runde1 == maxRound && nExtraRecursions-- > 0)
                    --runde1;
            }
            Stopwatches.Stop($"AllPlayersAllRoundsCalculateTa-All MaxRound={maxRound}");
            Stopwatches.Debug("");
        }

        /// <summary> Gibt die Keizer-Start-Pts für den 1. Spieler zurück. </summary>
        int FirstStartPtsInner(int playerCount, float ratioFirst2Last)
        {
            // ratioFirst2Last: Ein Sieg gegen den 1. in der Tabelle zählt ratioFirst2Last mal soviel wie gegen den letzten. 
            // ratioFirst2Last = 3 ist das was üblicherweise beim Keizersystem empfohlen wird. 
            // Kleinere Zahlen scheinen mir angemessener.
            // Verwendet nur noch in KeizerPointsList.
            double firstStartPtsFaktor = ratioFirst2Last / (ratioFirst2Last - 1);
            return Convert.ToInt32((playerCount - 1) * firstStartPtsFaktor);
        }

        /// <summary> I call the Normalization of Keizer points "WickerNormalization". </summary>
        internal enum WickerNormalization { None = 0, OneForVictoryAgainstLast = 1 };

        /// <summary> Returns list of KeizerPoints you'd get for a victory against player with certain rank. </summary>
        Li<float> KeizerPointsList(int playerCount)
        {
            Stopwatches.Start("KeizerPointsList");
            var kpn = (WickerNormalization)db.GetConfigInt("OPTION.WickerNormalization", 0);
            var ratioFirst2Last = db.GetConfigFloat("OPTION.RatioFirst2Last", 3);

            var kpl = new Li<float>();
            if (kpn == WickerNormalization.None)
            {
                var first = FirstStartPtsInner(playerCount, ratioFirst2Last);
                var last = first - playerCount + 1;
                for (int i = 0; i < playerCount; ++i)
                    if (kpn == WickerNormalization.None)
                        kpl.Add(first--);
            }
            else
            {
                // If unnormalized, the ratios between first and last are usually not exact.
                // If normalized, we can use exact ratios. 
                float delta = 0;
                if (kpn == WickerNormalization.OneForVictoryAgainstLast)
                    delta = (ratioFirst2Last - 1) / (playerCount - 1);

                for (int i = 0; i < playerCount; ++i)
                    kpl.Add(ratioFirst2Last - i * delta);

                var roundTo = GetNachkommaStellenForWickerNormalization(kpl[0], kpl[1]);
                if (kpn != WickerNormalization.None)
                {
                    for (int i = 0; i < playerCount; ++i)
                        kpl[i] = (float)Math.Round(kpl[i], roundTo);
                }
            }
            Stopwatches.Stop("KeizerPointsList");
            return kpl;
        }

        public static int GetNachkommaStellenForWickerNormalization(float kp0, float kp1)
        {
            int nkStellen = -1;
            var dkpl0 = kp0 - kp1;
            for (int i = 0; i < 10  && nkStellen == -1; ++i)
                if (dkpl0 > Math.Pow(10, -i))
                    nkStellen = i;
            if (nkStellen == -1)
                nkStellen = 1;  //Shouldn't happen, but it has happened. 
            return nkStellen;
        }

        TableW2Headers GetTableDbCacheForRound(int runde)
        {
            TableW2Headers table = runde != db.GetMaxRound() ?
                db.ReadTableWHeadersFromDb(TableType.Stand, runde) : null;
            return table;
        }

        /// <summary> Gibt T zurück, falls das ging, F andernfalls. Setzt Rank, KeizerStartPts und KeizerSumPts
        /// in die Player-Liste der DB, falls aus DB-Cache ermittelbar. </summary>
        /// <param name="runde">Stand nach dieser Runde gewünscht, 1-basiert. </param>
        bool AllPlayersSetRankAndStartPtsFromDbCacheTa(int runde, TableW2Headers table)
        {
            if (table == null || table.Count == 0)
                table = GetTableDbCacheForRound(runde);
            bool ok = table != null && table.Count != 0;
            if (ok)
            {
                // If the header line contains "Id", the table contains the playerId.
                int playerIdIdx = table[0].IndexOf("Id");
                // The table contains one header line. 
                var kpli = KeizerPointsList(table.Count - 1);

                db.BeginTransaction();
                for (int i = 1; i < table.Count; ++i)
                {
                    var row = table[i];
                    var rank = Helper.ToInt(row[0]);
                    var keizerSum = Helper.ToSingle(row[2]);
                    var playerId = playerIdIdx != -1 ? Helper.ToInt(row[playerIdIdx]) : db.GetPlayerID(row[1]);
                    db.UpdPlayer_SetRankAndStartPtsAndKeizerSumPts(playerId, rank, kpli[i - 1], keizerSum);
                }
                db.EndTransaction();
            }
            return ok;
        }

        /// <summary> Das ist die Funktion, die die anfänglichen Keizer-Punkte verteilt. </summary>
        /// <param name="currRunde"> Momentan auszulosende Runde, 1-basiert. </param>
        private void AllPlayersSetInitialStartPtsTa(int currRunde)
        {
            db.BeginTransaction();
            var firstRoundRandom = currRunde != 1 ? 0 : db.GetConfigInt("OPTION.FirstRoundRandom", 0);
            SqliteInterface.stPlayer[] pList = new SqliteInterface.stPlayer[100];
            var order = firstRoundRandom == 0 ? "rating" : "RatingWDelta";
            var playerCountNotDropped = db.GetPlayerCount_NotDropped(currRunde);
            var kpli = KeizerPointsList(playerCountNotDropped);
            int playerCount = db.GetPlayerList(ref pList, " ", $" ORDER BY {order} desc ", currRunde);
            Debug.WriteLine($"FirstStartPts = {kpli[0]} currRunde={currRunde}");
            for (int index = 0, iNotDropped = 0; index < playerCount; ++index)
            {
                if (pList[index].State != SqliteInterface.PlayerState.Retired)
                    db.UpdPlayer_SetRankAndStartPts(pList[index].Id, index + 1, kpli[iNotDropped++]);
                else
                    db.UpdPlayer_SetRankAndStartPts(pList[index].Id, index + 1, 0);
            }
            db.EndTransaction();
        }

        /// <summary> Berechnet die Keizer-Punkt-Summe aller Spieler anhand der in der DB stehende Punkte 
        /// für die Spiele und schreibt die Summen in die DB. </summary>
        private void AllPlayersSetKeizerSumPtsTa()
        {
            db.BeginTransaction();
            SqliteInterface.stPlayer[] players = new SqliteInterface.stPlayer[100];
            int playerCount = db.GetPlayerList(ref players, "", " ", db.GetMaxRound());
            for (int index = 0; index < playerCount; ++index)
                players[index].KeizerSumPts = db.GetPlayer_PunktSumme(players[index].Id);
            for (int index = 0; index < playerCount; ++index)
                db.UpdPlayer_KeizerSumPts(players[index].Id, players[index].KeizerSumPts);
            db.EndTransaction();
        }

        /// <summary> For all players: calc his current rank and set the rank and the resulting 
        /// Keizer_StartPts into the DB. </summary>
        private void AllPlayersSetRankAndStartPtsTa()
        {
            db.BeginTransaction();
            var players = db.GetPlayerLi(" Where state != 9 ", " ORDER BY Keizer_SumPts desc, rating desc ", db.GetMaxRound());
            var kpli = KeizerPointsList(players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].State == SqliteInterface.PlayerState.Retired)
                    continue;
                db.UpdPlayer_SetRankAndStartPts(players[i].Id, i + 1, kpli[i]);
            }
            db.EndTransaction();
        }

        /// <summary> Setzt für eine Runde für alle Bretter die KeizerPts in die DB. </summary>
        /// <returns>false, wenn keine Runden da sind, true sonst. </returns>
        /// <remarks> Die Punkte sind bei Sieg gleich der momentanen Keizer_StartPts der Gegner. </remarks>
        private bool OneRoundAllPairingsSetKeizerPtsTa(int runde, int endRundeWhichIsCalculated)
        {
            // Debug.WriteLine($"OneRoundAllPairingsSetKeizerPtsTa Runde={runde} EndRunde={endRundeWhichIsCalculated}");
            db.BeginTransaction();
            Stopwatches.Start($"OneRoundAllPairingsSetKeizerPtsTa-1");
            var pairings = db.GetPairingLi(" WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            Stopwatches.Stop($"OneRoundAllPairingsSetKeizerPtsTa-1");
            if (pairings.Count == 0)
                return false;
            for (int index = 0; index < pairings.Count; ++index)
            {
                var pair = pairings[index];
                float erg_w = 0.0f, erg_s = 0.0f;
                Stopwatches.Start("OneRoundAllPairingsSetKeizerPtsTa-2");
                (var pWhite, var pBlack) = db.GetPlayerBaseById(pair.IdW, pair.IdB);
                Stopwatches.Next("OneRoundAllPairingsSetKeizerPtsTa-3");

                if (SqliteInterface.IsWhiteWin(pair.Result))
                {
                    erg_w = pBlack.KeizerStartPts;
                    if(pair.Result != SqliteInterface.Results.WinWhiteForfeit)
                        erg_s = pWhite.KeizerStartPts * form.tbBonusVerlust.Value / 100.0f;
                }
                else if (SqliteInterface.IsDrawish(pair.Result))
                {
                    erg_w = pBlack.KeizerStartPts / 2f;
                    erg_s = pWhite.KeizerStartPts / 2f;
                }
                else if (SqliteInterface.IsBlackWin(pair.Result))
                {
                    erg_s = pWhite.KeizerStartPts;
                    if (pair.Result != SqliteInterface.Results.WinBlackForfeit)
                        erg_w = pBlack.KeizerStartPts * form.tbBonusVerlust.Value / 100.0f;
                }
                else if (pair.Result == SqliteInterface.Results.Excused)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusExcused.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.Unexcused)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusUnexcused.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.Hindered)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusClub.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.FreeWin)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusFreilos.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.LateStart)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusUnexcused.Value / 100.0f;
                if (pWhite.State == SqliteInterface.PlayerState.Retired)
                {
                    if (SqliteInterface.IsBlackWin(pair.Result))
                        erg_s = pBlack.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (SqliteInterface.IsDrawish(pair.Result))
                        erg_s = 0.5f * pBlack.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_s = 0;
                    erg_w = 0;
                }
                if (pBlack.State == SqliteInterface.PlayerState.Retired)
                {
                    if (SqliteInterface.IsWhiteWin(pair.Result))
                        erg_w = pWhite.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (SqliteInterface.IsDrawish(pair.Result))
                        erg_w = 0.5f * pWhite.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_w = 0;
                    erg_s = 0;
                }
                Stopwatches.Stop("OneRoundAllPairingsSetKeizerPtsTa-3");
                if (pair.PtsW != erg_w || pair.PtsB != erg_s)
                {
                    Stopwatches.Start("OneRoundAllPairingsSetKeizerPtsTa-4");
                    db.UpdPairingValues(runde, pair.Board, erg_w, erg_s);
                    Stopwatches.Stop("OneRoundAllPairingsSetKeizerPtsTa-4");
                }
            }
            Stopwatches.Start("OneRoundAllPairingsSetKeizerPtsTa-5");
            db.EndTransaction();
            Stopwatches.Next("OneRoundAllPairingsSetKeizerPtsTa-6");
            CheckPairingsValuesTa(runde, endRundeWhichIsCalculated);
            Stopwatches.Stop("OneRoundAllPairingsSetKeizerPtsTa-6");
            return true;
        }

        /// <summary> Prüft, ob ein Sieg gegen X auch in jeder Runde dasselbe zählt, wenn die
        /// endRundeWhichIsCalculated gerade berechnet wird und wir grade bei Runde maxRunde sind. </summary>
        void CheckPairingsValuesTa(int maxRunde, int endRundeWhichIsCalculated)
        {
            var er = endRundeWhichIsCalculated;
            // Diese Funktion braucht einen Haufen Zeit - was speziell bei sehr hohen Rundenzahlen nervt. 
            // Darum führ ich die Funktion dann nicht immer durch. 
            if (endRundeWhichIsCalculated > 15 && (maxRunde % 11 != 0 && maxRunde != 1 && maxRunde != er))
                return;

            // player -> valueOfWinAgainst
            var valueOfWinAgainstPlayerDict = new Dictionary<int, double>();
            Stopwatches.Start("CheckPairingsValuesTa-0");
            var playerStateDict = db.GetPlayerStateDict();
            Stopwatches.Stop("CheckPairingsValuesTa-0");
            // Ein Sieg gg Player X muß immer gleich viel wert sein, nach dem eine Runde durchgerechnet
            // wurde. Nach der nächsten Runde kann und wird der Sieg anders bewertet. 
            // Aber, wenn ich zB den Stand nach Runde 5 berechne, muß ein Sieg über X in Runde 1
            // genausoviel wert sein, wie ein Sieg über X in Runde 5. Entsprechend ein Remis halb so viel. 
            for (int runde = 1; runde <= maxRunde; ++runde)
            {
                Stopwatches.Start("CheckPairingsValuesTa-1");
                var pairings = db.GetPairingLi(" WHERE rnd=" + runde.ToString(), " ORDER BY board ");
                Stopwatches.Stop("CheckPairingsValuesTa-1");
                if (pairings.Count == 0)
                    throw new Exception("pairings.Count == 0");
                foreach (var p in pairings)
                {
                    if (p.Result == SqliteInterface.Results.WinWhite)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdB, p.PtsW, valueOfWinAgainstPlayerDict, er, playerStateDict);
                    }
                    else if (p.Result == SqliteInterface.Results.WinBlack)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdW, p.PtsB, valueOfWinAgainstPlayerDict, er, playerStateDict);
                    }
                    else if (p.Result == SqliteInterface.Results.Draw)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdW, 2 * p.PtsB, valueOfWinAgainstPlayerDict, er, playerStateDict);
                        CheckValueOfWinAgainst(p, runde, p.IdB, 2 * p.PtsW, valueOfWinAgainstPlayerDict, er, playerStateDict);
                    }
                }
            }
        }

        void CheckValueOfWinAgainst(SqliteInterface.stPairing p, int runde, int playerId, double valueOfWin,
                Dictionary<int, double> valueOfWinAgainstPlayerDict, int endRundeWhichIsCalculated,
                Dictionary<int, SqliteInterface.PlayerState> playerStateDict)
        {
            if (Math.Abs(valueOfWin) < 0.01)
                return;  // this is the case for retired players and not a real problem.

            Stopwatches.Start("CheckValueOfWinAgainst-1");
            if (!playerStateDict.TryGetValue(playerId, out SqliteInterface.PlayerState plst))
            {
                Stopwatches.Start("CheckValueOfWinAgainst-2");
                plst = db.GetPlayerState(playerId);
                playerStateDict[playerId] = plst;
                Stopwatches.Stop("CheckValueOfWinAgainst-2");
            }
            Stopwatches.Stop("CheckValueOfWinAgainst-1");
            if (plst == SqliteInterface.PlayerState.Retired)
                return;
            int er = endRundeWhichIsCalculated;
            // Debug.WriteLine($"ER={er} Runde={runde} {p.IdW}-{p.IdB} = {Ext.ToDebug(p.PtsW)}:{Ext.ToDebug(p.PtsB)}");
            if (!valueOfWinAgainstPlayerDict.TryGetValue(playerId, out double prevValue))
                valueOfWinAgainstPlayerDict[playerId] = valueOfWin;
            else
            {
                if (Math.Abs(prevValue - valueOfWin) > 0.01)
                {
                    var name = db.GetPlayerName(playerId);
                    var s = $"Win against Id={playerId} Name={name} differs from before in round {runde}. Previous={prevValue} Now={valueOfWin}";
                    Debug.WriteLine(s);
                    throw new Exception(s);
                }
            }
        }

        readonly SqliteInterface db;
        readonly frmMainform form;
        readonly ReportingUnit cReportingUnit;
    }
}
