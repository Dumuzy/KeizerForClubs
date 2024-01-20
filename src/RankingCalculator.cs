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
            cReportingUnit = shallUseReporting ? new ReportingUnit("qwert", db) : null;
        }

        /// <summary> Sets all KeizerPts of all games of all rounds to zero and recalculates all again. </summary>
        internal void AllPlayersAllRoundsCalculateTa()
        {
            cReportingUnit?.DeleteDump();
            int maxRound = db.GetMaxRound();

            //Setzt alle Bewertungen aller Paarungen und alle Keizer_SumPts auf 0.
            db.UpdPairing_AllPairingsAndAllKeizerSumsResetValuesTa(); 
            this.AllPlayersSetInitialStartPtsTa(maxRound + 1); // Keizer_StartPts in die DB setzen.
            this.AllPlayersSetKeizerSumPtsTa();    // Keizer_SumPts in die DB setzen, hier noch Keizer_SumPts = Keizer_StartPts.
            cReportingUnit?.DebugPairingsAndStandings(0);
            // If nExtraRecursions is > 0, at the end of the calculation, that many
            // extra rounds of calculation are appended. Only for testing stuff, currently. 
            int nExtraRecursions = 0;
            for (int runde1 = 1; runde1 <= maxRound; ++runde1)
            {
                Debug.WriteLine($"=========  ER={runde1} ===========");
                db.UpdPairing_AllPairingsAndAllKeizerSumsResetValuesTa();
                for (int runde2 = 1; runde2 <= runde1; ++runde2)
                    this.OneRoundAllPairingsSetKeizerPtsTa(runde2, runde1);
                this.AllPlayersSetKeizerSumPtsTa();
                this.AllPlayersSetRankAndStartPtsTa();
                cReportingUnit?.DebugPairingsAndStandings(runde1);
                if (runde1 == maxRound && nExtraRecursions-- > 0)
                    --runde1;
            }
        }

        /// <summary> Gibt die Keizer-Start-Pts für den 1. Spieler zurück. </summary>
        int FirstStartPts(int playerCount)
        {
            // ratioFirst2Last: Ein Sieg gegen den 1. in der Tabelle zählt ratioFirst2Last mal soviel wie gegen den letzten. 
            // ratioFirst2Last = 3 ist das was üblicherweise beim Keizersystem empfohlen wird. 
            // Kleinere Zahlen scheinen mir angemessener.
            // Verwendet in fRanking_Init und fRankingCalcRanks.
            var ratioFirst2Last = db.GetConfigFloat("OPTION.RatioFirst2Last", 3);
            double firstStartPtsFaktor = ratioFirst2Last / (ratioFirst2Last - 1);
            return Convert.ToInt32((playerCount - 1) * firstStartPtsFaktor);
        }

        /// <summary> Das ist die Funktion, die die anfänglichen Keizer-Punkte verteilt. </summary>
        /// <param name="currRunde"> Momentan auszulosende Runde, 1-basiert. </param>
        private void AllPlayersSetInitialStartPtsTa(int currRunde)
        {
            db.BeginTransaction();
            var firstRoundRandom = currRunde != 1 ? 0 : db.GetConfigInt("OPTION.FirstRoundRandom", 0);
            SqliteInterface.stPlayer[] pList = new SqliteInterface.stPlayer[100];
            var order = firstRoundRandom == 0 ? "rating" : "RatingWDelta";
            int playerCount = db.GetPlayerList(ref pList, " ", $" ORDER BY {order} desc ", currRunde);
            int firstStartPts = FirstStartPts(playerCount);
            for (int index = 0; index < playerCount; ++index)
            {
                if (pList[index].State != SqliteInterface.PlayerState.Retired)
                {
                    db.UpdPlayer_SetRankAndStartPts(pList[index].Id, index + 1, firstStartPts);
                    --firstStartPts;
                }
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
            int firstStartPts = FirstStartPts(players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].State == SqliteInterface.PlayerState.Retired)
                    continue;
                db.UpdPlayer_SetRankAndStartPts(players[i].Id, i + 1, firstStartPts);
                --firstStartPts;
            }
            db.EndTransaction();
        }

        /// <summary> Setzt für eine Runde für alle Bretter die KeizerPts in die DB. </summary>
        /// <returns>false, wenn keine Runden da sind, true sonst. </returns>
        /// <remarks> Die Punkte sind bei Sieg gleich der momentanen Keizer_StartPts der Gegner. </remarks>
        private bool OneRoundAllPairingsSetKeizerPtsTa(int runde, int endRundeWhichIsCalculated)
        {
            db.BeginTransaction();
            var pairings = db.GetPairingLi(" WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            if (pairings.Count == 0)
                return false;
            for (int index = 0; index < pairings.Count; ++index)
            {
                var pair = pairings[index];
                float erg_w = 0.0f, erg_s = 0.0f;
                var pWhite = db.GetPlayer(" WHERE ID=" + (object)pair.IdW, " ", runde);
                var pBlack = db.GetPlayer(" WHERE ID=" + (object)pair.IdB, " ", runde);

                if (pair.Result == SqliteInterface.Results.WinWhite)
                    erg_w = pBlack.KeizerStartPts;
                else if (pair.Result == SqliteInterface.Results.Draw)
                {
                    erg_w = pBlack.KeizerStartPts / 2f;
                    erg_s = pWhite.KeizerStartPts / 2f;
                }
                else if (pair.Result == SqliteInterface.Results.WinBlack)
                    erg_s = pWhite.KeizerStartPts;
                else if (pair.Result == SqliteInterface.Results.Excused)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusExcused.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.Unexcused)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusUnexcused.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.Hindered)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusClub.Value / 100.0f;
                else if (pair.Result == SqliteInterface.Results.FreeWin)
                    erg_w = pWhite.KeizerStartPts * form.tbBonusFreilos.Value / 100.0f;
                if (pWhite.State == SqliteInterface.PlayerState.Retired)
                {
                    if (pair.Result == SqliteInterface.Results.WinBlack)
                        erg_s = pBlack.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (pair.Result == SqliteInterface.Results.Draw)
                        erg_s = 0.5f * pBlack.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_s = 0;
                    erg_w = 0;
                }
                if (pBlack.State == SqliteInterface.PlayerState.Retired)
                {
                    if (pair.Result == SqliteInterface.Results.WinWhite)
                        erg_w = pWhite.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (pair.Result == SqliteInterface.Results.Draw)
                        erg_w = 0.5f * pWhite.KeizerStartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_w = 0;
                    erg_s = 0;
                }
                db.UpdPairingValues(runde, pair.Board, erg_w, erg_s);
            }
            db.EndTransaction();
            CheckPairingsValuesTa(runde, endRundeWhichIsCalculated);
            return true;
        }

        /// <summary> Prüft, ob ein Sieg gegen X auch in jeder Runde dasselbe zählt, wenn die
        /// endRundeWhichIsCalculated gerade berechnet wird und wir grade bei Runde maxRunde sind. </summary>
        void CheckPairingsValuesTa(int maxRunde, int endRundeWhichIsCalculated)
        {
            var er = endRundeWhichIsCalculated;
            // player -> valueOfWinAgainst
            var valueOfWinAgainstPlayerDict = new Dictionary<int, double>();
            // Ein Sieg gg Player X muß immer gleich viel wert sein, nach dem eine Runde durchgerechnet
            // wurde. Nach der nächsten Runde kann und wird der Sieg anders bewertet. 
            // Aber, wenn ich zB den Stand nach Runde 5 berechne, muß ein Sieg über X in Runde 1
            // genausoviel wert sein, wie ein Sieg über X in Runde 5. Entsprechend ein Remis halb so viel. 
            for (int runde = 1; runde <= maxRunde; ++runde)
            {
                var pairings = db.GetPairingLi(" WHERE rnd=" + runde.ToString(), " ORDER BY board ");
                if (pairings.Count == 0)
                    throw new Exception("pairings.Count == 0");
                foreach (var p in pairings)
                {
                    if (p.Result == SqliteInterface.Results.WinWhite)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdB, p.PtsW, valueOfWinAgainstPlayerDict, er);
                    }
                    else if (p.Result == SqliteInterface.Results.WinBlack)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdW, p.PtsB, valueOfWinAgainstPlayerDict, er);
                    }
                    else if (p.Result == SqliteInterface.Results.Draw)
                    {
                        CheckValueOfWinAgainst(p, runde, p.IdW, 2 * p.PtsB, valueOfWinAgainstPlayerDict, er);
                        CheckValueOfWinAgainst(p, runde, p.IdB, 2 * p.PtsW, valueOfWinAgainstPlayerDict, er);
                    }
                }
            }
        }

        void CheckValueOfWinAgainst(SqliteInterface.stPairing p, int runde, int playerId, double valueOfWin,
                Dictionary<int, double> valueOfWinAgainstPlayerDict, int endRundeWhichIsCalculated)
        {
            int er = endRundeWhichIsCalculated;
            // Debug.WriteLine($"ER={er} Runde={runde} {p.IdW}-{p.IdB} = {Ext.ToDebug(p.PtsW)}:{Ext.ToDebug(p.PtsB)}");
            if (!valueOfWinAgainstPlayerDict.TryGetValue(playerId, out double prevValue))
                valueOfWinAgainstPlayerDict[playerId] = valueOfWin;
            else
            {
                if (Math.Abs(prevValue-valueOfWin) > 0.01)
                {
                    Debug.WriteLine($"Win against {playerId} differs from before in round {runde}.");
                    throw new Exception($"Win against {playerId} differs from before in round {runde}.");
                }
            }
        }

        readonly SqliteInterface db;
        readonly frmMainform form;
        readonly ReportingUnit cReportingUnit;
    }
}
