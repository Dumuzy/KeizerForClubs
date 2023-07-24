namespace KeizerForClubs
{
    internal class RankingCalculator
    {
        public RankingCalculator(SqliteInterface db, frmMainform mainform)
        {
            this.db = db;
            this.form = mainform;
        }

        /// <summary> Sets all KeizerPts of all games of all rounds to zero and recalculates all again. </summary>
        internal void AllPlayersAllRoundsCalculate()
        {
            ReportingUnit cReportingUnit = null; //  new cReportingUnit(sTurniername, db);
            cReportingUnit?.DeleteDump();
            int maxRound = db.fGetMaxRound();

            db.fUpdPairing_AllPairingsAndAllKeizerSumsResetValues();
            this.AllPlayersSetInitialStartPts(maxRound); // Keizer_StartPts in die DB setzen.
            this.AllPlayersSetKeizerSumPts();    // Keizer_SumPts in die DB setzen, hier noch Keizer_SumPts = Keizer_StartPts.
            cReportingUnit?.DebugPairingsAndStandings(0);
            // If nExtraRecursions is > 0, at the end of the calculation, that many
            // extra rounds of calculation are appended. Only for testing stuff, currently. 
            int nExtraRecursions = 0;
            for (int runde1 = 1; runde1 <= maxRound; ++runde1)
            {
                db.fUpdPairing_AllPairingsAndAllKeizerSumsResetValues();
                for (int runde2 = 1; runde2 <= runde1; ++runde2)
                    this.OneRoundAllPairingsSetKeizerPts(runde2);
                this.AllPlayersSetKeizerSumPts();
                this.AllPlayersSetRankAndStartPts();
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
            var ratioFirst2Last = db.fGetConfigFloat("OPTION.RatioFirst2Last", 3);
            double firstStartPtsFaktor = ratioFirst2Last / (ratioFirst2Last - 1);
            return Convert.ToInt32((playerCount - 1) * firstStartPtsFaktor);
        }

        /// <summary> Das ist die Funktion, die die anfänglichen Keizer-Punkte verteilt. </summary>
        /// <param name="currRunde"> Momentan auszulosende Runde, 1-basiert. </param>
        private void AllPlayersSetInitialStartPts(int currRunde)
        {
            var firstRoundRandom = currRunde != 1 ? 0 : db.fGetConfigInt("OPTION.FirstRoundRandom", 0);
            SqliteInterface.stPlayer[] pList = new SqliteInterface.stPlayer[100];
            var order = firstRoundRandom == 0 ? "rating" : "RatingWDelta";
            int playerCount = db.fGetPlayerList(ref pList, " ", $" ORDER BY {order} desc ", currRunde);
            int firstStartPts = FirstStartPts(playerCount);
            for (int index = 0; index < playerCount; ++index)
            {
                if (pList[index].state != SqliteInterface.ePlayerState.eRetired)
                {
                    db.fUpdPlayer_SetRankAndStartPts(pList[index].id, index + 1, firstStartPts);
                    --firstStartPts;
                }
                else
                    db.fUpdPlayer_SetRankAndStartPts(pList[index].id, index + 1, 0);
            }
        }

        /// <summary> Berechnet die Keizer-Punkt-Summe aller Spieler anhand der in der DB stehende Punkte 
        /// für die Spiele und schreibt die Summen in die DB. </summary>
        private void AllPlayersSetKeizerSumPts()
        {
            SqliteInterface.stPlayer[] players = new SqliteInterface.stPlayer[100];
            int playerCount = db.fGetPlayerList(ref players, "", " ", db.fGetMaxRound());
            for (int index = 0; index < playerCount; ++index)
                players[index].Keizer_SumPts = db.fGetPlayer_PunktSumme(players[index].id);
            for (int index = 0; index < playerCount; ++index)
                db.fUpdPlayer_KeizerSumPts(players[index].id, players[index].Keizer_SumPts);
        }

        /// <summary> For all players: calc his current rank and set the rank and the resulting 
        /// Keizer_StartPts into the DB. </summary>
        private void AllPlayersSetRankAndStartPts()
        {
            var players = db.fGetPlayerLi(" ", " ORDER BY Keizer_SumPts desc, rating desc ", db.fGetMaxRound());
            int firstStartPts = FirstStartPts(players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].state == SqliteInterface.ePlayerState.eRetired)
                    continue;
                db.fUpdPlayer_SetRankAndStartPts(players[i].id, i + 1, firstStartPts);
                --firstStartPts;
            }
        }

        /// <summary> Setzt für eine Runde für alle Bretter die KeizerPts in die DB. </summary>
        /// <returns>false, wenn keine Runden da sind, true sonst. </returns>
        /// <remarks> Die Punkte sind bei Sieg gleich der momentanen Keizer_StartPts der Gegner. </remarks>
        private bool OneRoundAllPairingsSetKeizerPts(int runde)
        {
            var pairings = db.GetPairingLi(" WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            if (pairings.Count == 0)
                return false;
            for (int index = 0; index < pairings.Count; ++index)
            {
                var pair = pairings[index];
                float erg_w = 0.0f, erg_s = 0.0f;
                var pWhite = db.fGetPlayer(" WHERE ID=" + (object)pair.id_w, " ", runde);
                var pBlack = db.fGetPlayer(" WHERE ID=" + (object)pair.id_b, " ", runde);

                if (pair.result == SqliteInterface.eResults.eWin_White)
                    erg_w = pBlack.Keizer_StartPts;
                else if (pair.result == SqliteInterface.eResults.eDraw)
                {
                    erg_w = pBlack.Keizer_StartPts / 2f;
                    erg_s = pWhite.Keizer_StartPts / 2f;
                }
                else if (pair.result == SqliteInterface.eResults.eWin_Black)
                    erg_s = pWhite.Keizer_StartPts;
                else if (pair.result == SqliteInterface.eResults.eExcused)
                    erg_w = pWhite.Keizer_StartPts * form.tbBonusExcused.Value / 100.0f;
                else if (pair.result == SqliteInterface.eResults.eUnexcused)
                    erg_w = pWhite.Keizer_StartPts * form.tbBonusUnexcused.Value / 100.0f;
                else if (pair.result == SqliteInterface.eResults.eHindered)
                    erg_w = pWhite.Keizer_StartPts * form.tbBonusClub.Value / 100.0f;
                else if (pair.result == SqliteInterface.eResults.eFreeWin)
                    erg_w = pWhite.Keizer_StartPts * form.tbBonusFreilos.Value / 100.0f;
                if (pWhite.state == SqliteInterface.ePlayerState.eRetired)
                {
                    if (pair.result == SqliteInterface.eResults.eWin_Black)
                        erg_s = pBlack.Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (pair.result == SqliteInterface.eResults.eDraw)
                        erg_s = 0.5f * pBlack.Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_s = 0;
                    erg_w = 0;
                }
                if (pBlack.state == SqliteInterface.ePlayerState.eRetired)
                {
                    if (pair.result == SqliteInterface.eResults.eWin_White)
                        erg_w = pWhite.Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    else if (pair.result == SqliteInterface.eResults.eDraw)
                        erg_w = 0.5f * pWhite.Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    else
                        erg_w = 0;
                    erg_s = 0;
                }
                db.fUpdPairingValues(runde, pair.board, erg_w, erg_s);
            }
            return true;
        }

        readonly SqliteInterface db;
        readonly frmMainform form;
    }
}
