using System.Data;
using System.Data.SQLite;
using System.Drawing.Drawing2D;
using AwiUtils;

namespace KeizerForClubs
{
    public class SqliteInterface
    {
        public string cLangCode = "";
        private SQLiteConnection SQLiteMyDB;
        private SQLiteTransaction SQLiteMyTrans;
        private SQLiteCommand sqlCommand;


        public enum ePlayerState
        {
            eUnknown = 0,
            eAvailable = 1,
            ePaired = 2,
            eHindered = 5,
            eExcused = 6,
            eUnexcused = 7,
            eFreilos = 8,
            eRetired = 9,
            eDeleted = 10,
            eNotYetStarted = 11,
            eErrUndefined = -1
        };

        public enum eResults
        {
            eWin_White = 1,
            eDraw = 2,
            eWin_Black = 3,

            eFreeWin = 4,

            eHindered = 5,
            eExcused = 6,
            eUnexcused = 7,

            eErrUndefined = -1
        };

        public enum eTableType
        {
            None = 0, Stand, Kreuz, Spieler
        }

        public struct stPlayer
        {
            public int id;
            public string name;
            public int rating;
            public int RatingWDelta;  // Original Rating plus delta, if FirstRoundRandom.
            public int rank;
            public ePlayerState state;
            public float Keizer_StartPts;
            public float Keizer_SumPts;
            public int FreeCnt;	// Anzahl Freilose; nur für Auslosung aufgenommen      
            public override string ToString()
            {
                return $"{id} {name} rank:{rank} state:{state} frei:{FreeCnt} kStart {Keizer_StartPts} kSum {Keizer_SumPts}";
            }
        };

        public struct stPairing
        {
            public int round;
            public int board;
            public int id_w;
            public int id_b;
            public eResults result;
            public float pts_w;
            public float pts_b;
            public override string ToString()
            {
                return $"{round} {board} W:{id_w} B:{id_b} res:{result} pts_w {pts_w} pts_b {pts_b}";
            }
        };

        public SqliteInterface()
        {
            SQLiteMyDB = new SQLiteConnection("");
            sqlCommand = new SQLiteCommand(SQLiteMyDB);
        }

        public void BeginnTransaktion() => SQLiteMyTrans = SQLiteMyDB.BeginTransaction();

        public void EndeTransaktion(bool bCommit = true)
        {
            if (bCommit)
                SQLiteMyTrans.Commit();
            else
                SQLiteMyTrans.Rollback();
        }

        public bool fOpenTournament(string sDatei)
        {
            SQLiteMyDB.Close();
            SQLiteMyDB.ConnectionString = "Data Source=" + sDatei;
            SQLiteMyDB.Open();

            sqlCommand.CommandText =
                            " CREATE TABLE IF NOT EXISTS Player ( " +
                            " id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                            " name VARCHAR(30) NOT NULL, " +
                            " rating INTEGER(4) NOT NULL, " +
                            " rank INTEGER(3) NOT NULL, " +
                            " state INTEGER(2), " +
                            " Keizer_StartPts FLOAT, " +
                            " Keizer_SumPts FLOAT " +
                            ");";
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText =
                    " CREATE TABLE IF NOT EXISTS Pairing ( " +
                    " Rnd INTEGER NOT NULL, " +
                    " board INTEGER NOT NULL, " +
                    " PID_W INTEGER NOT NULL, " +
                    " PID_B INTEGER NOT NULL, " +
                    " Result INTEGER(2), " +
                    " Pts_W FLOAT, " +
                    " Pts_B FLOAT " +
                    ");";
            sqlCommand.ExecuteNonQuery();


            sqlCommand.CommandText = " ATTACH 'cfg\\KeizerForClubs.config.s3db' AS config_db ";
            sqlCommand.ExecuteNonQuery();
            fCreateConfigTab();
            cLangCode = fGetConfigText("LANGCODE");
            this.AddRatingWDeltaColIfNeeded();
            return true;
        }

        #region Config
        public int fSetConfigText(string key, string text)
        {
            sqlCommand.CommandText = $" UPDATE {getTable(key)} SET CfgText= @pCfgTxt  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgTxt", text);
            sqlCommand.Prepare();
            return sqlCommand.ExecuteNonQuery();
        }

        public int fSetConfigFloat(string key, float wert)
        {
            sqlCommand.CommandText = $" UPDATE {getTable(key)}  SET CfgValue= @pCfgValue  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgValue", wert);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteNonQuery();
            res = fInsertConfigNumberIfNeeded(key, res);
            return res;
        }

        public int fSetConfigInt(string key, int wert)
        {
            sqlCommand.CommandText = $" UPDATE {getTable(key)} SET CfgValue= @pCfgValue  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgValue", wert);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteNonQuery();
            res = fInsertConfigNumberIfNeeded(key, res);
            return res;
        }

        private string getTable(string key) => key.StartsWith("INTERNAL.") ? "config_db.ConfigTab" : "ConfigTab";


        private int fInsertConfigNumberIfNeeded(string key, int res)
        {
            if (res == 0)
            {
                sqlCommand.CommandText = $" INSERT INTO {getTable(key)} (CfgKey, CfgValue) VALUES (@pKey, @pCfgValue)";
                sqlCommand.Prepare();
                res = sqlCommand.ExecuteNonQuery();
            }
            return res;
        }

        public float fSetConfigBool(string key, bool wert) => fSetConfigInt(key, wert ? 1 : 0);

        public string fGetConfigText(string key)
        {
            sqlCommand.CommandText = $" SELECT CfgText FROM {getTable(key)}  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            return Convert.ToString(sqlCommand.ExecuteScalar());
        }

        public int fGetConfigInt(string key, int? defaultVal = null)
        {
            sqlCommand.CommandText = $" SELECT CfgValue FROM {getTable(key)} WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteScalar();
            int val = res == null ? defaultVal.Value : Convert.ToInt32(res);
            return val;
        }

        public bool fGetConfigBool(string key) => fGetConfigInt(key) != 0;

        public float fGetConfigFloat(string key, float defaultVal)
        {
            sqlCommand.CommandText = $" SELECT CfgValue FROM {getTable(key)} WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteScalar();
            float val = res == null ? defaultVal : Convert.ToSingle(res);
            return val;
        }

        private void fCreateConfigTab()
        {
            sqlCommand.CommandText = @"CREATE TABLE IF NOT EXISTS ConfigTab (
	                CfgKey VARCHAR(20) NOT NULL PRIMARY KEY,
   	                CfgText VARCHAR(5), CfgValue FLOAT); 
                    Select COUNT(1) from ConfigTab";
            int n = Helper.ToInt(sqlCommand.ExecuteScalar());
            if (n == 0)
            {
                // The table is new and has to be filled. Copy initial values from config_db.
                sqlCommand.CommandText = @" INSERT INTO ConfigTab SELECT * FROM config_db.ConfigTab 
                        WHERE CfgKey NOT LIKE 'INTERNAL.%'";
                sqlCommand.ExecuteNonQuery();
            }
        }
        #endregion Config

        #region Player
        public bool InsPlayerNew(string name, int rtg)
        {
            sqlCommand.CommandText = " INSERT INTO Player (Name,Rating, Rank)  VALUES (@pName, @pRtg, 99) ";
            sqlCommand.Parameters.AddWithValue("pName", name);
            sqlCommand.Parameters.AddWithValue("pRtg", rtg);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        public bool UpdPlayer(int id, string name, int rtg, int status)
        {
            sqlCommand.CommandText = " UPDATE Player  SET name=@pName,  Rating=@pRtg,  State=@pState  WHERE ID=@pID ";
            sqlCommand.Parameters.AddWithValue("pName", name);
            sqlCommand.Parameters.AddWithValue("pRtg", rtg);
            sqlCommand.Parameters.AddWithValue("pState", status);
            sqlCommand.Parameters.AddWithValue("pID", id);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        public bool UpdPlayerRatingWDelta(int id, int rtgWDelta)
        {
            sqlCommand.CommandText = " UPDATE Player  SET RatingWDelta=@pRtg  WHERE ID=@pID ";
            sqlCommand.Parameters.AddWithValue("pRtg", rtgWDelta);
            sqlCommand.Parameters.AddWithValue("pID", id);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        /// <summary> Schreibt die Summe nach Keizer_SumPts des Spielers mit der ID. </summary>
        public void UpdPlayer_KeizerSumPts(int id, float summe)
        {
            sqlCommand.CommandText = " UPDATE Player  SET Keizer_SumPts=@pPts  WHERE ID=@pID ";
            sqlCommand.Parameters.AddWithValue("pPts", summe);
            sqlCommand.Parameters.AddWithValue("pID", id);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        public bool UpdPlayer_SetRankAndStartPts(int id, int rang, float pkte)
        {
            sqlCommand.CommandText = " UPDATE Player  SET Rank = @pRang, Keizer_StartPts=@pPts  WHERE ID=@pID ";
            sqlCommand.Parameters.AddWithValue("pRang", rang);
            sqlCommand.Parameters.AddWithValue("pPts", pkte);
            sqlCommand.Parameters.AddWithValue("pID", id);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        // Partien eines Spieler abfragen(für Kreuztabelle)
        public bool GetPlayerGames(int ID, ref string[] sResults)
        {
            sqlCommand.CommandText = " Select pr.Rnd, pr.PID_W, pr.PID_B, pr.Result,  (Select Player.Rank from Player   where PID IN (pr.PID_W, pr.PID_B) and PID<>:pNr) as Gegnerrang  from Pairing pr  where (PID_W=:pID or PID_B=:pID)  order by rnd ";
            sqlCommand.Parameters.AddWithValue("pID", (object)ID);
            sqlCommand.Prepare();
            using (SQLiteDataReader reader = sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (index < sResults.Length)
                        {
                            string str = (reader.IsDBNull(0) ? "" : reader.GetString(0)) + (reader.IsDBNull(0) ? "" : reader.GetString(1)) + (reader.IsDBNull(0) ? "" : reader.GetString(2)) + (reader.IsDBNull(0) ? "" : reader.GetString(3)) + (reader.IsDBNull(0) ? "" : reader.GetString(4));
                            sResults[index] = str;
                            ++index;
                        }
                        else
                            break;
                    }
                }
            }
            return true;
        }

        // Farbverteilung eines Spieler abfragen:
        //  Gibt die Differenz aus Weiß- und Schwarzpartien des Spielers zurück. 
        public int GetPlayerWeissUeberschuss(int ID)
        {
            int playerFarbzaehlung = 0;
            string str = ID.ToString();
            string sWhere = " where ((PID_W=" + str + " and PID_B>=0) or PID_B=" + str + ") ";
            SqliteInterface.stPairing[] pList = new SqliteInterface.stPairing[50];
            int cnt = GetPairingList(ref pList, sWhere, "");
            for (int index = 0; index < cnt; ++index)
            {
                if (pList[index].id_w == ID)
                    ++playerFarbzaehlung;
                if (pList[index].id_b == ID)
                    --playerFarbzaehlung;
            }
            return playerFarbzaehlung;
        }

        public string GetPlayerName(int ID)
        {
            if (ID < 0)
                return "NN";
            sqlCommand.CommandText = " Select name from player  where ID=:pID ";
            sqlCommand.Parameters.AddWithValue("pID", (object)ID);
            return sqlCommand.ExecuteScalar().ToString();
        }

        /// <summary> Gibt die PlayerId des Spielers mit dem Namen zurück oder -1, falls keiner gefunden. </summary>
        public int GetPlayerID(string sName)
        {
            sqlCommand.CommandText = " Select ID from player  where name=:pName ";
            sqlCommand.Parameters.AddWithValue("pName", sName);
            var res = sqlCommand.ExecuteScalar();
            return Convert.ToInt16(res);
        }

        public int CntPlayerNames(string sName)
        {
            sqlCommand.CommandText = " Select Count(*) from player  where name=:pName ";
            sqlCommand.Parameters.AddWithValue("pName", (object)sName);
            return (int)Convert.ToInt16(sqlCommand.ExecuteScalar());
        }

        /// <summary> Keizer-PunkteSumme des Spielers mit der ID aus der DB berechnen. So wie sie sich 
        /// momentan aus der DB ergibt. Das ist zu Anfang der Berechnung des Tabellenstands einer Runde 
        /// ein anderer Wert als am Ende. </summary>
        /// <param name="ID">Spieler-ID</param>
        public float GetPlayer_PunktSumme(int ID)
        {
            sqlCommand.CommandText = @" select  Keizer_StartPts +   
                (Select ifnull(Sum( Pts_W),0.0) from Pairing where PID_W = p1.ID) +    
                (Select ifnull(Sum( Pts_B),0.0) from Pairing where PID_B = p1.ID)  as summe    
                from player p1  where ID=:pID ";
            sqlCommand.Parameters.AddWithValue("pID", (object)ID);
            sqlCommand.Prepare();
            return Convert.ToSingle(Convert.ToDouble(sqlCommand.ExecuteScalar()));
        }

        // Partiepunkte (Sieg=1, remis=1/2) liefern  
        public float GetPlayer_PartiePunkte(int ID)
        {
            sqlCommand.CommandText = @" select  (Select Count(*) from Pairing where PID_W= p1.ID and result=1) +
                (Select Count(*) from Pairing where PID_B= p1.ID and result=3) +
                (Select Count(*) from Pairing where (PID_W= p1.ID OR PID_B= p1.ID) 
                    and result=2)/2.0  from player p1  where ID=:pID ";
            sqlCommand.Parameters.AddWithValue("pID", (object)ID);
            sqlCommand.Prepare();
            return Convert.ToSingle(sqlCommand.ExecuteScalar());
        }

        private bool HasColumn(string tableName, string colName)
        {
            bool ok = false;
            sqlCommand.CommandText = $"pragma table_info('{tableName}')";
            using (SQLiteDataReader reader = sqlCommand.ExecuteReader())
                if (reader.HasRows)
                    while (!ok && reader.Read())
                    {
                        var col = reader.GetString(1);
                        if (col == colName)
                            ok = true;
                    }
            return ok;
        }

        private void AddRatingWDeltaColIfNeeded()
        {
            bool hasCol = HasColumn("Player", "RatingWDelta");
            if (!hasCol)
            {
                try
                {
                    sqlCommand.CommandText = "ALTER TABLE Player ADD COLUMN RatingWDelta INT NOT NULL DEFAULT '0'";
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    // Might be the column already exists. 
                }
            }
        }

        /// <summary> In der Player-Liste kann man Player auf status "deleted" setzen. Diese Fkt. 
        /// löscht alle Player mit dem Status "deleted" aus der Player-Liste. </summary>
        /// <returns>Anzahl gelöschter Spieler. </returns>
        public int DelDeletedPlayers()
        {
            int nDeleted = 0;
            if (GetMaxRound() == 0)
            {
                sqlCommand.CommandText = " SELECT COUNT(1) from player where state = " + (int)ePlayerState.eDeleted;
                nDeleted = Helper.ToInt(sqlCommand.ExecuteScalar());
                if (nDeleted > 0)
                {
                    sqlCommand.CommandText = " DELETE from player where state = " + (int)ePlayerState.eDeleted;
                    sqlCommand.ExecuteScalar();
                }
            }
            return nDeleted;
        }
        #endregion Player

        #region Pairing
        // Alle Paarungen einer Runde löschen 
        public bool DelPairings(int runde)
        {
            sqlCommand.CommandText = " DELETE FROM Pairing  WHERE Rnd=@pRunde ";
            sqlCommand.Parameters.AddWithValue("pRunde", runde);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        // Anzahl Paarungen ohne Ergebnis (=Runde nicht beendet?)
        public int GetPairings_NoResult()
        {
            sqlCommand.CommandText = " Select Count(*) from Pairing  WHERE result=-1 ";
            return (int)Convert.ToInt16(sqlCommand.ExecuteScalar());
        }

        /// <summary> Gibt die Anzahl der Runden zurück, die bisher gelost sind. Egal ob schon gespielt oder nicht. </summary>
        public int GetMaxRound()
        {
            int m = 0;
            sqlCommand.CommandText = " Select Max(rnd) from pairing ";
            try
            {
                m = Convert.ToInt16(sqlCommand.ExecuteScalar());
            }
            catch (Exception) { }
            return m;
        }

        public bool InsPairingNew(int runde, int brett, int idw, int ids)
        {
            sqlCommand.CommandText = " INSERT INTO Pairing  (Rnd, board, PID_W, PID_B, Result, Pts_W,Pts_B)  " +
                                                  "VALUES  (@pRnd, @pBoard, @pPID_W, @pPID_B, -1 , 0, 0); ";
            sqlCommand.Parameters.AddWithValue("pRnd", runde);
            sqlCommand.Parameters.AddWithValue("pBoard", brett);
            sqlCommand.Parameters.AddWithValue("pPID_W", idw);
            sqlCommand.Parameters.AddWithValue("pPID_B", ids);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        // Ergebnis einer Paarung eintragen
        public bool UpdPairingResult(int runde, int idw, int ids, SqliteInterface.eResults erg)
        {
            // Ergebnis gültig? 
            // "Normal" oder "Spezialergebnis" je nach richtiger oder Pseudo-Paarung
            if (ids > 0)
            {
                if (erg != SqliteInterface.eResults.eWin_White &&
                    erg != SqliteInterface.eResults.eWin_Black &&
                    erg != SqliteInterface.eResults.eDraw)
                    return false;
            }
            else
            {   // Pseudo-Paarung (Freilos, entschuldigt abwesend etc)
                if (erg == eResults.eWin_White ||
                    erg == eResults.eWin_Black ||
                    erg == eResults.eDraw)
                    return false;
            }

            // Ok, jetzt eintragen  
            sqlCommand.CommandText = " UPDATE Pairing  SET result=@pResult  WHERE Rnd=@pRunde  " +
                                            " AND PID_W=@pID_W  AND PID_B=@pID_B ";
            sqlCommand.Parameters.AddWithValue("pResult", (int)erg);
            sqlCommand.Parameters.AddWithValue("pRunde", runde);
            sqlCommand.Parameters.AddWithValue("pID_W", idw);
            sqlCommand.Parameters.AddWithValue("pID_B", ids);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        /// <summary> Setzt alle Bewertungen aller Paarungen und alle Keizer_SumPts auf 0. </summary>
        public void UpdPairing_AllPairingsAndAllKeizerSumsResetValues()
        {
            sqlCommand.CommandText = " UPDATE Pairing  SET PTS_W=0, PTS_B=0 ";
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            sqlCommand.CommandText = " UPDATE Player  SET Keizer_SumPts=0 ";
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary> Setzt die KeizerPts für eine Runde für ein Brett in die DB. </summary>
        /// <param name="erg_w"> KeizerPts für W. </param>
        /// <param name="erg_s"> KeizerPts für S. </param>
        /// <returns></returns>
        public bool UpdPairingValues(int runde, int board, float erg_w, float erg_s)
        {
            sqlCommand.CommandText = " UPDATE Pairing  SET Pts_W=@pPts_W, Pts_B=@pPts_B " +
                                        " WHERE Rnd=@pRunde  AND board=@pBoard ";
            sqlCommand.Parameters.AddWithValue("pRunde", (object)runde);
            sqlCommand.Parameters.AddWithValue("pBoard", (object)board);
            sqlCommand.Parameters.AddWithValue("pPts_W", (object)erg_w);
            sqlCommand.Parameters.AddWithValue("pPts_B", (object)erg_s);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        // Gibt die Anzahl der Paarungen idw - idb seit minrunde zurück. 
        // Umgedrehte Farben werden nicht berücksichtigt. 
        public int CountPairingVorhandenSinceOneWay(int minrunde, int idw, int ids)
        {
            SQLiteCommand sqlCmdCheckVorhanden = new SQLiteCommand(SQLiteMyDB);
            sqlCmdCheckVorhanden.CommandText = " SELECT Count(1) FROM Pairing " +
                    "  WHERE Rnd>=@pMinrunde AND (PID_W=@pID_W AND PID_B=@pID_B)  ;";
            sqlCmdCheckVorhanden.Parameters.Add("pMinrunde", DbType.Decimal);
            sqlCmdCheckVorhanden.Parameters.Add("pID_W", DbType.Decimal);
            sqlCmdCheckVorhanden.Parameters.Add("pID_B", DbType.Decimal);

            sqlCmdCheckVorhanden.Parameters[0].Value = (object)minrunde;
            sqlCmdCheckVorhanden.Parameters[1].Value = (object)idw;
            sqlCmdCheckVorhanden.Parameters[2].Value = (object)ids;
            sqlCmdCheckVorhanden.Prepare();

            int n = Helper.ToInt(sqlCmdCheckVorhanden.ExecuteScalar());
            return n;
        }

        // Gibt die Anzahl der Paarungen idw - idb seit minrunde zurück. 
        // Umgedrehte Farben werden mitgezählt.
        public int CountPairingVorhandenSince(int minrunde, int idw, int ids)
        {
            int n = CountPairingVorhandenSinceOneWay(minrunde, idw, ids);
            n += CountPairingVorhandenSinceOneWay(minrunde, ids, idw);
            return n;
        }

        public int GetPairingList(ref SqliteInterface.stPairing[] pList, string sWhere, string sSortorder)
        {
            int pairingList = 0;
            sqlCommand.CommandText = " SELECT Rnd, board, pid_w, pid_b, result, pts_w, pts_b  FROM Pairing " + sWhere + sSortorder;
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
            {
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                    {
                        if (pairingList < pList.Length)
                        {
                            pList[pairingList].round = sqLiteDataReader.IsDBNull(0) ? 0 : (int)sqLiteDataReader.GetInt16(0);
                            pList[pairingList].board = sqLiteDataReader.IsDBNull(1) ? 0 : (int)sqLiteDataReader.GetInt16(1);
                            pList[pairingList].id_w = sqLiteDataReader.IsDBNull(2) ? 0 : (int)sqLiteDataReader.GetInt16(2);
                            pList[pairingList].id_b = sqLiteDataReader.IsDBNull(3) ? 0 : (int)sqLiteDataReader.GetInt16(3);
                            int num = sqLiteDataReader.IsDBNull(4) ? 0 : sqLiteDataReader.GetInt32(4);
                            pList[pairingList].result = (SqliteInterface.eResults)num;
                            pList[pairingList].pts_w = sqLiteDataReader.IsDBNull(5) ? 0.0f : sqLiteDataReader.GetFloat(5);
                            pList[pairingList].pts_b = sqLiteDataReader.IsDBNull(6) ? 0.0f : sqLiteDataReader.GetFloat(6);
                            ++pairingList;
                        }
                        else
                            break;
                    }
                }
            }
            return pairingList;
        }

        public Li<SqliteInterface.stPairing> GetPairingLi(string sWhere, string sSortorder)
        {
            SqliteInterface.stPairing[] pList1 = new SqliteInterface.stPairing[100];
            var n = GetPairingList(ref pList1, sWhere, sSortorder);
            var li = new Li<SqliteInterface.stPairing>(pList1.Take(n));
            return li;
        }
        #endregion Pairing

        #region Playerlist
        /// <summary> Spielerliste aus der DB abfragen.  Auch Tabellenstand möglich über SortOrder.
        /// Aber Achtung. Spieler-State ist in RUnde N durch die Pairing-Tabelle definiert, wenn Runde n
        /// schon gelost ist. 
        /// </summary>
        /// <returns>Number of players.</returns>
        public int GetPlayerList(ref stPlayer[] pList, string sWhere, string sSortorder, int runde)
        {
            string sFrei = $@"LEFT JOIN(SELECT pid_w, COUNT(1) frei from Pairing  
                                WHERE result in (4,5,6,7) GROUP BY pid_w) f on f.PID_W = p.id ";
            int playerCount = 0;
            sqlCommand.CommandText = @" SELECT p.id, p.name, p.rating, p.state, p.Keizer_StartPts, p.Keizer_SumPts, 
                                p.rank, f.frei, p.ratingWDelta FROM Player p 
                                " + sFrei + sWhere + sSortorder;
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
            {
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                    {
                        if (playerCount < pList.Length)
                        {
                            pList[playerCount].id = sqLiteDataReader.IsDBNull(0) ? 0 : (int)sqLiteDataReader.GetInt16(0);
                            pList[playerCount].name = sqLiteDataReader.IsDBNull(1) ? "" : sqLiteDataReader.GetString(1);
                            pList[playerCount].rating = sqLiteDataReader.IsDBNull(2) ? 0 : (int)sqLiteDataReader.GetInt16(2);
                            int num = sqLiteDataReader.IsDBNull(3) ? 0 : sqLiteDataReader.GetInt32(3);
                            pList[playerCount].state = (ePlayerState)num;
                            pList[playerCount].Keizer_StartPts = sqLiteDataReader.IsDBNull(4) ? 0.0f : sqLiteDataReader.GetFloat(4);
                            pList[playerCount].Keizer_SumPts = sqLiteDataReader.IsDBNull(5) ? 0.0f : sqLiteDataReader.GetFloat(5);
                            pList[playerCount].rank = sqLiteDataReader.IsDBNull(6) ? 0 : (int)sqLiteDataReader.GetInt16(6);
                            pList[playerCount].FreeCnt = sqLiteDataReader.IsDBNull(7) ? 0 : (int)sqLiteDataReader.GetInt16(7);
                            pList[playerCount].RatingWDelta = sqLiteDataReader.IsDBNull(8) ? 0 : (int)sqLiteDataReader.GetInt16(8);
                            ++playerCount;
                        }
                        else
                            break;
                    }
                }
            }
            return playerCount;
        }

        /// <summary> Spielerliste aus der DB abfragen.  Auch Tabellenstand möglich über SortOrder.
        /// Aber Achtung. Spieler-State ist in RUnde N durch die Pairing-Tabelle definiert, wenn Runde n
        /// schon gelost ist.  </summary>
        public Li<stPlayer> GetPlayerLi(string sWhere, string sSortorder, int runde)
        {
            SqliteInterface.stPlayer[] arr = new stPlayer[100];
            int playerCount = GetPlayerList(ref arr, sWhere, sSortorder, runde);
            return new Li<stPlayer>(arr.Take(playerCount));
        }

        /// <summary> Gibt die Liste der Spieler aus früheren Runden zurück. Nur Id, Name, Rating 
        /// und Status sind verwertbar und richtig. </summary>
        public Li<stPlayer> GetPreviousPlayerLi(string sWhere, string sSortorder, int runde)
        {
            SqliteInterface.stPlayer[] players = new stPlayer[100];
            int count = GetPlayerList(ref players, sWhere, sSortorder, GetMaxRound() + 1);
            var pairings = GetPairingLi($" WHERE Rnd={runde} ", "");
            for (int i = 0; i < count; ++i)
            {
                var playerCurrState = players[i].state;
                players[i].state = ePlayerState.eRetired;
                foreach (var pa in pairings)
                {
                    if (pa.id_w == players[i].id || pa.id_b == players[i].id)
                    {
                        if (pa.result.IsContainedIn(new eResults[]{ eResults.eWin_White, eResults.eDraw,
                                        eResults.eWin_Black, eResults.eFreeWin }.ToLi()))
                        {
                            players[i].state = ePlayerState.eAvailable;
                            break;
                        }
                    }
                    if (pa.id_w == players[i].id)
                    {
                        if (pa.result.IsContainedIn(new eResults[]{ eResults.eHindered, eResults.eExcused,
                                        eResults.eUnexcused}.ToLi()))
                        {
                            players[i].state = (ePlayerState)(int)(pa.result);
                            break;
                        }
                    }
                }
                if (players[i].state == ePlayerState.eRetired)
                {
                    // Hier ist sicher, daß der Spieler nicht vorkam in dieser Runde. Also ist er entweder
                    // zurückgetreten oder ein Spätstarter. Wenn sein playerCurrState eRetired ist, dann ist er
                    // irgendwann zurückgetreten. Ich zähle den als zurückgetreten. Es ist nicht 100% korrekt, 
                    // weil es auch sein könnte, daß es ein zurückgetretener Spätstarter ist und der in der aktuell
                    // gefragten Runde noch gar nicht da war. 
                    if (playerCurrState != ePlayerState.eRetired)
                        players[i].state = ePlayerState.eNotYetStarted;
                }
            }
            return new Li<stPlayer>(players.Take(count).Where(p => p.state != ePlayerState.eNotYetStarted));
        }


        public int GetPlayerList_Available(ref stPlayer[] pList, int runde)
        {
            string sWhere = " WHERE state IN (1) ";
            string sSortorder = " ORDER BY ID ";
            return GetPlayerList(ref pList, sWhere, sSortorder, runde);
        }

        public int GetPlayerList_NotDropped(ref stPlayer[] pList, string sOrder, int runde)
        {
            string sWhere = " WHERE state NOT IN (9) ";
            return GetPlayerList(ref pList, sWhere, sOrder, runde);
        }

        public int GetPlayerCount()
        {
            sqlCommand.CommandText = @" SELECT Count(1) FROM Player p ";
            int playerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return playerCount;
        }

        /// <summary> Returns the first player that matches the query. New empty player wiht state 
        /// unknown and id -1, if none found. </summary>
        public stPlayer GetPlayer(string sWhere, string sSortorder, int runde)
        {
            var arr = new stPlayer[1];
            int n = GetPlayerList(ref arr, sWhere, sSortorder, runde);
            return n == 0 ? new stPlayer { id = -1 } : arr[0];
        }
        #endregion Playerlist

        #region Localization
        public string Locl_GetText(string topic, string key)
        {
            string s = key;
            try
            {
                sqlCommand.CommandText = " SELECT text FROM config_db.LangText  WHERE code= @pCode  AND topic=@pTopic  AND key=@pKey ";
                sqlCommand.Parameters.AddWithValue("pCode", (object)cLangCode);
                sqlCommand.Parameters.AddWithValue("pTopic", (object)topic);
                sqlCommand.Parameters.AddWithValue("pKey", (object)key);
                sqlCommand.Prepare();
                s = Convert.ToString(sqlCommand.ExecuteScalar());
            }
            catch (Exception)
            {
                if (key != "FehlerAufgetreten")
                    throw;
            }
            return s;
        }

        public string Locl_FindKey(string topic, string pvalue)
        {
            sqlCommand.CommandText = " SELECT key FROM config_db.LangText  WHERE code= @pCode  AND topic=@pTopic  AND text=@pValue ";
            sqlCommand.Parameters.AddWithValue("pCode", (object)cLangCode);
            sqlCommand.Parameters.AddWithValue("pTopic", (object)topic);
            sqlCommand.Parameters.AddWithValue("pValue", (object)pvalue);
            sqlCommand.Prepare();
            return Convert.ToString(sqlCommand.ExecuteScalar());
        }

        public int Locl_GetPlayerState(string state)
        {
            string key = Locl_FindKey("PLAYERSTATE", state);
            return !(key == "") ? Convert.ToInt32(key, 16) : -1;
        }

        public string Locl_GetPlayerStateText(SqliteInterface.ePlayerState state)
        {
            // In der fLocl_GetText("PLAYERSTATE", ..) Fkt steht der Playerstate eDeleted als "A".
            // Deshalb das ToString("X", das macht Hex. Also 10 -> A
            var sState = ((int)state).ToString("X");
            return Locl_GetText("PLAYERSTATE", sState);
        }

        public SqliteInterface.eResults Locl_GetGameResult(string result)
        {
            string key = Locl_FindKey("GAMERESULT", result);
            return !(key == "") ? (SqliteInterface.eResults)Convert.ToInt32(key) : SqliteInterface.eResults.eErrUndefined;
        }

        public string Locl_GetGameResultText(SqliteInterface.eResults result) => Locl_GetText("GAMERESULT", ((int)result).ToString());

        public string Locl_GetGameResultShort(SqliteInterface.eResults result)
        {
            var s = Locl_GetGameResultText(result);
            if (s.StartsWith("-") || s.StartsWith("+"))
            {
                if (s.Length >= 5)
                {
                    s = s.Substring(3, s.Length - 4);
                    s = s.Truncate(4, null);
                    if (s == "ents" || s == "unen")
                        s = "fehl";
                    if (s == "Frei")
                        s = "frei";
                }
            }
            return s;
        }

        public int Locl_GetTopicTexte(string topic, string sWhereAdd, ref string[] texte)
        {
            int topicTexte = 0;
            sqlCommand.CommandText = " SELECT text FROM config_db.LangText  WHERE code= @pCode  AND topic=@pTopic " + sWhereAdd + " ORDER BY key ";
            sqlCommand.Parameters.AddWithValue("pCode", (object)cLangCode);
            sqlCommand.Parameters.AddWithValue("pTopic", (object)topic);
            sqlCommand.Prepare();
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
            {
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                    {
                        texte[topicTexte] = sqLiteDataReader.GetString(0);
                        ++topicTexte;
                    }
                }
            }
            return topicTexte;
        }
        #endregion Localization

        #region Tabelle
        const string tableSplitter = "§";

        public void WriteTableWHeaders2Db(eTableType tt, int runde, TableW2Headers table)
        {
            var tn = TabWHName(tt, runde);
            var sql = new Li<string>();
            sql.Add($@" DROP TABLE IF EXISTS {tn};");
            sql.Add($"CREATE TABLE {tn} (id INTEGER PRIMARY KEY, line TEXT NOT NULL);");
            sql.Add($"INSERT into {tn} (line) VALUES('{table.Header1}');");
            sql.Add($"INSERT into {tn} (line) VALUES('{table.Header2}');");
            sql.Add($"INSERT into {tn} (line) VALUES('{string.Join(tableSplitter, table.Footer)}');");
            for (int i = 0; i < table.Count; ++i)
            {
                var line = string.Join(tableSplitter, table[i]);
                sql.Add($"INSERT into {tn} (line) VALUES('{line}');");
            }
            sqlCommand.CommandText = string.Join("\n", sql);
            sqlCommand.ExecuteNonQuery();
        }

        public TableW2Headers ReadTableWHeadersFromDb(eTableType tt, int runde)
        {
            var tn = TabWHName(tt, runde);
            var table = new TableW2Headers("");
            sqlCommand.CommandText = $"SELECT line FROM {tn} ORDER BY id;";
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
                if (sqLiteDataReader.HasRows)
                {
                    int i = 0;
                    while (sqLiteDataReader.Read())
                    {
                        var text = sqLiteDataReader.IsDBNull(0) ? "" : sqLiteDataReader.GetString(0);
                        if (i == 0)
                            table.Header1 = text;
                        else if (i == 1)
                            table.Header2 = text;
                        else if (i == 2)
                            table.Footer = text.Split(tableSplitter).ToLi();
                        else
                            table.AddRow(text.Split(tableSplitter).ToLi());
                        ++i;
                    }
                }
            return table;
        }

        string TabWHName(eTableType tt, int runde) => "t" + tt.ToString() + "_" + runde;

        public void DelCurrentStoredTablesWHeader(int runde)
        {
            var tns = new string[] { TabWHName(eTableType.Stand, runde), TabWHName(eTableType.Kreuz, runde) };
            sqlCommand.CommandText = $@"
                DROP TABLE IF EXISTS {tns[0]};
                DROP TABLE IF EXISTS {tns[1]};";
            sqlCommand.ExecuteNonQuery();
        }
        #endregion Tabelle

    }
}
