namespace KeizerForClubs
{
    internal class RankingCalculator
    {
        public RankingCalculator(cSqliteInterface db, frmMainform mainform)
        {
            this.db = db;
            this.form = mainform;
        }

        /// <summary> Sets all KeizerPts of all games of all rounds to zero and recalculates all again. </summary>
        internal void AllPlayersAllRoundsCalculate()
        {
            cReportingUnit cReportingUnit = null; //  new cReportingUnit(sTurniername, db);
            cReportingUnit?.DeleteDump();
            int maxRound = db.fGetMaxRound();

            db.fUpdPairing_AllPairingsAndAllKeizerSumsResetValues();
            this.AllPlayersSetInitialStartPts(maxRound + 1); // Keizer_StartPts in die DB setzen.
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
            var firstRoundRandom = currRunde != 1 ? 0 : db.fGetConfigInt("OPTION.FirstRoundRandom", 0) ;
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            var order = firstRoundRandom == 0 ? "rating" : "RatingWDelta";
            int playerCount = db.fGetPlayerList(ref pList, " WHERE state NOT IN (9) ", $" ORDER BY {order} desc ");
            int firstStartPts = FirstStartPts(playerCount);
            for (int index = 0; index < playerCount; ++index)
            {
                db.fUpdPlayer_SetRankAndStartPts(pList[index].id, index + 1, firstStartPts);
                --firstStartPts;
            }
        }

        /// <summary> Berechnet die Keizer-Punkt-Summe aller Spieler anhand der in der DB stehende Punkte 
        /// für die Spiele und schreibt die Summen in die DB. </summary>
        private void AllPlayersSetKeizerSumPts()
        {
            cSqliteInterface.stPlayer[] players = new cSqliteInterface.stPlayer[100];
            int playerCount = db.fGetPlayerList(ref players, "", " ");
            for (int index = 0; index < playerCount; ++index)
                players[index].Keizer_SumPts = db.fGetPlayer_PunktSumme(players[index].id);
            for (int index = 0; index < playerCount; ++index)
                db.fUpdPlayer_KeizerSumPts(players[index].id, players[index].Keizer_SumPts);
        }

        /// <summary> For all players: calc his current rank and set the rank and the resulting 
        /// Keizer_StartPts into the DB. </summary>
        private void AllPlayersSetRankAndStartPts()
        {
            cSqliteInterface.stPlayer[] players = new cSqliteInterface.stPlayer[100];
            int playerCount = db.fGetPlayerList(ref players, " WHERE state NOT IN (9) ",
                " ORDER BY Keizer_SumPts desc, rating desc ");
            int firstStartPts = FirstStartPts(playerCount);
            for (int index = 0; index < playerCount; ++index)
            {
                db.fUpdPlayer_SetRankAndStartPts(players[index].id, index + 1, firstStartPts);
                --firstStartPts;
            }
        }

        /// <summary> Setzt für eine Runde für alle Bretter die KeizerPts in die DB. </summary>
        /// <returns>false, wenn keine Runden da sind, true sonst. </returns>
        /// <remarks> Die Punkte sind bei Sieg gleich der momentanen Keizer_StartPts der Gegner. </remarks>
        private bool OneRoundAllPairingsSetKeizerPts(int runde)
        {
            cSqliteInterface.stPairing[] pList1 = new cSqliteInterface.stPairing[50];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            int pairingCnt = db.fGetPairingList(ref pList1, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            if (pairingCnt == 0)
                return false;
            for (int index = 0; index < pairingCnt; ++index)
            {
                float erg_w = 0.0f;
                float erg_s = 0.0f;
                db.fGetPlayerList(ref pList2, " WHERE ID=" + (object)pList1[index].id_w, " ");
                db.fGetPlayerList(ref pList3, " WHERE ID=" + (object)pList1[index].id_b, " ");
                if (pList1[index].result == cSqliteInterface.eResults.eWin_White)
                    erg_w = pList3[0].Keizer_StartPts;
                else if (pList1[index].result == cSqliteInterface.eResults.eDraw)
                {
                    erg_w = pList3[0].Keizer_StartPts / 2f;
                    erg_s = pList2[0].Keizer_StartPts / 2f;
                }
                else if (pList1[index].result == cSqliteInterface.eResults.eWin_Black)
                    erg_s = pList2[0].Keizer_StartPts;
                else if (pList1[index].result == cSqliteInterface.eResults.eExcused)
                    erg_w = pList2[0].Keizer_StartPts * form.tbBonusExcused.Value / 100.0f;
                else if (pList1[index].result == cSqliteInterface.eResults.eUnexcused)
                    erg_w = pList2[0].Keizer_StartPts * form.tbBonusUnexcused.Value / 100.0f;
                else if (pList1[index].result == cSqliteInterface.eResults.eHindered)
                    erg_w = pList2[0].Keizer_StartPts * form.tbBonusClub.Value / 100.0f;
                else if (pList1[index].result == cSqliteInterface.eResults.eFreeWin)
                    erg_w = pList2[0].Keizer_StartPts * form.tbBonusFreilos.Value / 100.0f;
                if (pList2[0].state == cSqliteInterface.ePlayerState.eRetired)
                {
                    erg_s = pList3[0].Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    erg_w = 0.0f;
                }
                if (pList3[0].state == cSqliteInterface.ePlayerState.eRetired)
                {
                    erg_w = pList2[0].Keizer_StartPts * form.tbBonusRetired.Value / 100.0f;
                    erg_s = 0.0f;
                }
                db.fUpdPairingValues(runde, pList1[index].board, erg_w, erg_s);
            }
            return true;
        }

        readonly cSqliteInterface db;
        readonly frmMainform form;
    }
}
