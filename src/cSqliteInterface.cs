using System.Data;
using System.Data.SQLite;
using AwiUtils;

namespace KeizerForClubs
{
    public enum TableType
    {
        None = 0, Stand, Kreuz, Spieler, Paarungen
    }


    public class SqliteInterface
    {
        public enum PlayerState
        {
            ErrUndefined = -1,
            Unknown = 0,

            Available = 1,
            Paired = 2,

            Hindered = 5,
            Excused = 6,
            Unexcused = 7,

            Freilos = 8,
            Retired = 9,
            Deleted = 10,
            NotYetStarted = 11,
        };

        public enum Results
        {
            ErrUndefined = -1,
            Unknown = 0,

            WinWhite = 1,
            Draw = 2,
            WinBlack = 3,

            FreeWin = 4,

            Hindered = 5,
            Excused = 6,
            Unexcused = 7,

            //  TODO for T51.
            //WinWhiteForfeit = 8, 
            //WinBlackForfeit = 9, 
            //ForfeitForfeit = 10, 
            //Adjourned = 11,
        };

        public struct stPlayer
        {
            public int Id;
            public string Name;
            public int Rating;
            public int RatingWDelta;  // Original Rating plus delta, if FirstRoundRandom.
            public int Rank;
            public PlayerState State;
            public float KeizerStartPts, KeizerSumPts, KeizerPrevPts;
            public int FreeCnt;	// Anzahl Freilose; nur für Auslosung aufgenommen      
            public override string ToString()
            {
                return $"{Id} {Name} rank:{Rank} state:{State} frei:{FreeCnt} kPrev {KeizerPrevPts} kSum {KeizerSumPts}";
            }
        };

        /// <summary> Player class without the DB expensive stuff. Mainly FreeCnt is very expensive. </summary>
        /// <remarks> And this here is a class, stPlayer still is a struct. </remarks>
        public class clPlayerBase
        {
            public int Id;
            public string Name;
            public PlayerState State;
            public float KeizerStartPts;
            public override string ToString() => $"{Id} {Name} state:{State} kStart {KeizerStartPts} ";
        }

        public struct stPairing
        {
            public int Round;
            public int Board;
            public int IdW;
            public int IdB;
            public Results Result;
            public float PtsW;
            public float PtsB;
            public override string ToString()
            {
                return $"{Round} {Board} W:{IdW} B:{IdB} res:{Result} pts_w {PtsW} pts_b {PtsB}";
            }
            public const int FirstNonPlayingBoard = 100;
            public bool IsNonPlayingBoard => Board >= FirstNonPlayingBoard;
        };

        public class clPairing
        {
            public clPairing(stPairing pair) { this.P = pair; }
            public stPairing P;
            public override string ToString() => P.ToString();
        }

        public SqliteInterface()
        {
            SQLiteMyDB = new SQLiteConnection("");
            sqlCommand = new SQLiteCommand(SQLiteMyDB);
        }

        public void BeginTransaction()
        {
            if (SQLiteMyTrans != null)
                throw new Exception("Transaction already started.");
            SQLiteMyTrans = SQLiteMyDB.BeginTransaction();
        }

        public void EndTransaction(bool bCommit = true)
        {
            if (bCommit)
                SQLiteMyTrans.Commit();
            else
                SQLiteMyTrans.Rollback();
            SQLiteMyTrans = null;
        }

        public bool OpenTournament(string sDatei)
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
                            " Keizer_SumPts FLOAT" +
            ");";
            sqlCommand.ExecuteNonQuery();

            // Adding these two columns in a try catch, as they might already be there. SQLite seems not to
            // have ALTER .. IF. One workaround is to just create the columns and catch the exception
            // that arise if the column already exist. 
            Li<string> addColumnSql = new Li<string>
            {
                    "ALTER TABLE Player ADD COLUMN RatingWDelta INTEGER NOT NULL DEFAULT 0",
                    "ALTER TABLE Player ADD COLUMN Keizer_PrevPts FLOAT"
            };
            foreach (var cmd in addColumnSql)
                try
                {
                    sqlCommand.CommandText = cmd;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception) { }

            sqlCommand.CommandText =
                @" CREATE UNIQUE INDEX IF NOT EXISTS [id-name-rating-state-etcpp] ON Player (id, name, 
                    rating, state, rank, Keizer_SumPts, Keizer_StartPts, RatingWDelta, Keizer_PrevPts);";
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
                    ");" +
                    @" CREATE UNIQUE INDEX IF NOT EXISTS [rnd-board-pid_w-pid_b-etcpp] ON Pairing (Rnd, board, 
                                    PID_W, PID_B, Result, Pts_W, Pts_B); 
                       CREATE INDEX IF NOT EXISTS [result-pid_w-rnd-board] ON Pairing (Result, PID_W, Rnd, board); 
                       CREATE INDEX IF NOT EXISTS [pid_w] ON Pairing (PID_W); 
                    ";
            sqlCommand.ExecuteNonQuery();


            sqlCommand.CommandText = " ATTACH 'cfg\\KeizerForClubs.config.s3db' AS config_db ";
            sqlCommand.ExecuteNonQuery();
            CreateConfigTab();
            LangCode = GetConfigText("LANGCODE");
            this.AddRatingWDeltaColIfNeeded();
            return true;
        }

        #region Config
        public int SetConfigText(string key, string text)
        {
            sqlCommand.CommandText = @$"INSERT or REPLACE INTO {getTable(key)} (CfgKey, CfgText) 
                    VALUES(@pKey, @pCfgTxt)";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgTxt", text);
            sqlCommand.Prepare();
            return sqlCommand.ExecuteNonQuery();
        }

        public int SetConfigFloat(string key, float wert)
        {
            sqlCommand.CommandText = $" UPDATE {getTable(key)}  SET CfgValue= @pCfgValue  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgValue", wert);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteNonQuery();
            res = InsertConfigNumberIfNeeded(key, res);
            return res;
        }

        public int SetConfigInt(string key, int wert)
        {
            sqlCommand.CommandText = $" UPDATE {getTable(key)} SET CfgValue= @pCfgValue  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Parameters.AddWithValue("pCfgValue", wert);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteNonQuery();
            res = InsertConfigNumberIfNeeded(key, res);
            return res;
        }

        private string getTable(string key) => key.StartsWith("INTERNAL.") ? "config_db.ConfigTab" : "ConfigTab";


        private int InsertConfigNumberIfNeeded(string key, int res)
        {
            if (res == 0)
            {
                sqlCommand.CommandText = $" INSERT INTO {getTable(key)} (CfgKey, CfgValue) VALUES (@pKey, @pCfgValue)";
                sqlCommand.Prepare();
                res = sqlCommand.ExecuteNonQuery();
            }
            return res;
        }

        public float SetConfigBool(string key, bool wert) => SetConfigInt(key, wert ? 1 : 0);

        public string GetConfigText(string key)
        {
            sqlCommand.CommandText = $" SELECT CfgText FROM {getTable(key)}  WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            return Convert.ToString(sqlCommand.ExecuteScalar());
        }

        public int GetConfigInt(string key, int? defaultVal = null)
        {
            sqlCommand.CommandText = $" SELECT CfgValue FROM {getTable(key)} WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteScalar();
            int val = res == null ? defaultVal.Value : Convert.ToInt32(res);
            return val;
        }

        public bool GetConfigBool(string key, bool? defaultVal = null) =>
            GetConfigInt(key, defaultVal.HasValue && defaultVal.Value ? 1 : 0) != 0;

        public float GetConfigFloat(string key, float defaultVal)
        {
            sqlCommand.CommandText = $" SELECT CfgValue FROM {getTable(key)} WHERE CfgKey= @pKey ";
            sqlCommand.Parameters.AddWithValue("pKey", key);
            sqlCommand.Prepare();
            var res = sqlCommand.ExecuteScalar();
            float val = res == null ? defaultVal : Convert.ToSingle(res);
            return val;
        }

        private void CreateConfigTab()
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
            BeginTransaction();
            sqlCommand.CommandText = " INSERT INTO Player (Name, Rating, Rank, state)  VALUES (@pName, @pRtg, 99, 1)";
            sqlCommand.Parameters.AddWithValue("pName", name);
            sqlCommand.Parameters.AddWithValue("pRtg", rtg);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            EndTransaction();
            return true;
        }

        /// <summary> Setzt die automatisch vergebene Player-Id auf die Anzahl der Spieler n. 
        /// D.h. der nächste Spieler, der erzeugt wird, erhält die Spieler-Id n+1. </summary>
        /// <remarks> Darf nur nach ResetPlayerIds oder DeleteAllPlayers durchgeführt werden. </remarks>
        public void ResetPlayerBaseIdTa()
        {
            int cnt = CntPlayers();
            BeginTransaction();
            sqlCommand.CommandText = $"UPDATE sqlite_sequence SET seq = {cnt} WHERE name = 'Player'";
            sqlCommand.ExecuteNonQuery();
            EndTransaction();
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
            sqlCommand.CommandText = " UPDATE Player  SET Rank = @pRang, Keizer_PrevPts=Keizer_StartPts, Keizer_StartPts=@pPts  WHERE ID=@pID ";
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

        /// <summary> Gibt die Anzahl der gespielten Spiele (mit Ergebnis gewonnen, verloren oder remis)
        /// des Spielers mit der id zurück. </summary>
        public int CntPlayersPlayedGames(int id)
        {
            var sWhere = $" where ((PID_W={id} or PID_B={id}) AND Result in (1,2,3)) ";
            var li = GetPairingLi(sWhere, "");
            return li.Count;
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
                if (pList[index].IdW == ID)
                    ++playerFarbzaehlung;
                if (pList[index].IdB == ID)
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
            return sqlCommand.ExecuteScalar()?.ToString() ?? null;
        }


        public PlayerState GetPlayerState(int id)
        {
            if (id < 0)
                return PlayerState.ErrUndefined;
            sqlCommand.CommandText = " Select state from player  where ID=:pID ";
            sqlCommand.Parameters.AddWithValue("pID", (object)id);
            var res = sqlCommand.ExecuteScalar();
            var ires = res == null ? (long)(-1) : (long)res;
            return (PlayerState)ires;
        }

        public Dictionary<int, SqliteInterface.PlayerState> GetPlayerStateDict()
        {
            var d = new Dictionary<int, SqliteInterface.PlayerState>();
            sqlCommand.CommandText = " Select id, state from player ";
            using (var r = sqlCommand.ExecuteReader())
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        var id = r.IsDBNull(0) ? 0 : (int)r.GetInt16(0);
                        var state = (PlayerState)(r.IsDBNull(1) ? (long)(-1) : (long)r.GetInt16(1));
                        d[id] = state;
                    }
                }
            return d;
        }

        ///// <summary> Way faster than without cache. </summary>
        //public PlayerState GetPlayerStateCached(int id)
        //{
        //}

        /// <summary> Gibt die PlayerId des Spielers mit dem Namen zurück oder -1, falls keiner gefunden. </summary>
        public int GetPlayerID(string sName)
        {
            sqlCommand.CommandText = " Select ID from player  where name=:pName ";
            sqlCommand.Parameters.AddWithValue("pName", sName);
            var res = sqlCommand.ExecuteScalar();
            return res != null ? Convert.ToInt16(res) : -1;
        }

        /// <summary> Returns the first player that matches the query. New empty player with state 
        /// unknown and id -1, if none found. </summary>
        public stPlayer GetPlayer(string sWhere, string sSortorder, int runde)
        {
            var arr = new stPlayer[1];
            Stopwatches.Start("GetPlayer-1");
            int n = GetPlayerList(ref arr, sWhere, sSortorder, runde);
            Stopwatches.Stop("GetPlayer-1");
            return n == 0 ? new stPlayer { Id = -1 } : arr[0];
        }

        /// <summary> Returns the players with the given Ids in the order of the ids. 
        /// New empty player with state unknown and id -1, if one is not found.</summary>
        public Tuple<clPlayerBase, clPlayerBase> GetPlayerBaseById(int id, int id2)
        {
            Stopwatches.Start("GetPlayerBaseById");
            sqlCommand.CommandText = " SELECT id, name, state, Keizer_StartPts FROM Player p WHERE " +
                (id2 == -1 ? $"id = {id}" : $"id in ({id}, {id2})");
            Li<clPlayerBase> lip = new();
            clPlayerBase p = null;
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                    {
                        p = new clPlayerBase();
                        p.Id = sqLiteDataReader.IsDBNull(0) ? 0 : (int)sqLiteDataReader.GetInt16(0);
                        p.Name = sqLiteDataReader.IsDBNull(1) ? "" : sqLiteDataReader.GetString(1);
                        int num = sqLiteDataReader.IsDBNull(2) ? 0 : sqLiteDataReader.GetInt32(2);
                        p.State = (PlayerState)num;
                        p.KeizerStartPts = sqLiteDataReader.IsDBNull(3) ? 0.0f : sqLiteDataReader.GetFloat(3);
                        lip.Add(p);
                    }
                }

            while (lip.Count < 2)
                lip.Add(new clPlayerBase { Id = -1, State = PlayerState.Unknown });
            clPlayerBase p1, p2;
            if (lip[0].Id == id)
            {
                p1 = lip[0];
                p2 = lip[1];
            }
            else if (lip[1].Id == id)
            {
                p1 = lip[1];
                p2 = lip[0];
            }
            else if (lip[0].Id == id2)
            {
                p1 = lip[1];
                p2 = lip[0];
            }
            else if (lip[1].Id == id2)
            {
                p1 = lip[0];
                p2 = lip[1];
            }
            else
            {
                p1 = lip[0];
                p2 = lip[1];
            }

            if (p1.Id != id && p1.Id != -1)
                throw new InvalidOperationException($"p1.Id ({p1.Id}) != id ({id}) && p1.Id != -1");
            else if (p2.Id != id2 && p2.Id != -1)
                throw new InvalidOperationException($"p2.Id ({p2.Id}) != id2 ({id2}) && p2.Id != -1");

            Stopwatches.Stop("GetPlayerBaseById");
            var res = new Tuple<clPlayerBase, clPlayerBase>(p1, p2);
            return res;
        }

        public int CntPlayerNames(string sName)
        {
            sqlCommand.CommandText = " Select Count(*) from player  where name=:pName ";
            sqlCommand.Parameters.AddWithValue("pName", (object)sName);
            return (int)Convert.ToInt16(sqlCommand.ExecuteScalar());
        }

        public int CntPlayers()
        {
            sqlCommand.CommandText = " Select Count(1) from player";
            return Helper.ToInt(sqlCommand.ExecuteScalar());
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
                sqlCommand.CommandText = " SELECT COUNT(1) from player where state = " + (int)PlayerState.Deleted;
                nDeleted = Helper.ToInt(sqlCommand.ExecuteScalar());
                if (nDeleted > 0)
                {
                    sqlCommand.CommandText = " DELETE from player where state = " + (int)PlayerState.Deleted;
                    sqlCommand.ExecuteScalar();
                }
            }
            return nDeleted;
        }

        public void DeleteAllPlayersTa()
        {
            if (GetMaxRound() == 0)
            {
                BeginTransaction();
                sqlCommand.CommandText = " DELETE from player;";
                sqlCommand.ExecuteScalar();
                EndTransaction();
            }
        }

        public void RebasePlayerIdsTa()
        {
            if (GetMaxRound() == 0)
            {
                var li = GetPlayerLi("", "order by id desc", 0);
                BeginTransaction();
                for (int i = 0; i < li.Count; ++i)
                {
                    sqlCommand.CommandText = $"UPDATE PLAYER set id={li[i].Id + 1000} WHERE id={li[i].Id}";
                    sqlCommand.ExecuteNonQuery();
                }


                li = GetPlayerLi("", "order by rating desc", 0);
                for (int i = 0; i < li.Count; ++i)
                {
                    sqlCommand.CommandText = $"UPDATE PLAYER set id={i + 1} WHERE id={li[i].Id}";
                    sqlCommand.ExecuteNonQuery();
                }

                RefreshRatingWDelta();

                EndTransaction();
            }
        }

        // This function should probably called everytime when round 1 is deleted. 
        public void RefreshRatingWDelta()
        {
            if (GetMaxRound() == 0)
            {
                var li = GetPlayerLi("", "", 0);
                var firstRoundRandom = GetConfigInt("OPTION.FirstRoundRandom", 0);
                for (int rwd, i = 0; i < li.Count; ++i)
                {
                    if (firstRoundRandom == 0)
                        rwd = 0;
                    else
                        rwd = random.Next(-firstRoundRandom, firstRoundRandom) + li[i].Rating;
                    UpdPlayerRatingWDelta(li[i].Id, rwd);
                }
            }
        }
        static readonly Random random = new();
        #endregion Player

        #region Pairing

        public void ChangeBoards(int runde, Li<SqliteInterface.clPairing> pairs)
        {
            var bb = string.Join(',', pairs.Select(p => p.P.Board.ToString()).ToArray());
            var andCondition = $" AND board in ({bb}) ";
            DelPairings(runde, andCondition);
            foreach (var p in pairs)
                InsPairingNew(runde, p.P.Board, p.P.IdW, p.P.IdB);
        }

        // Alle Paarungen einer Runde löschen 
        public bool DelPairings(int runde, string andCondition = "")
        {
            sqlCommand.CommandText = $" DELETE FROM Pairing  WHERE Rnd=@pRunde {andCondition}";
            sqlCommand.Parameters.AddWithValue("pRunde", runde);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            return true;
        }

        // Alle Paarungen einer Runde löschen, in denen player1 oder player2 vorkommt. 
        public void DelPairings(int runde, int playerId1, int playerId2 = -3678)
        {
            sqlCommand.CommandText = @" DELETE FROM Pairing  WHERE Rnd=@pRunde AND 
                            (PID_W in (@p1, @p2) OR PID_B in (@p1, @p2)) ";
            sqlCommand.Parameters.AddWithValue("pRunde", runde);
            sqlCommand.Parameters.AddWithValue("p1", playerId1);
            sqlCommand.Parameters.AddWithValue("p2", playerId2);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        // Gibt das nächste freie Brett der Runde zurück. 
        public int GetNextFreeBrettOfRound(int runde, bool isNonPlayingBoard)
        {
            var extra = isNonPlayingBoard ? $" AND board >= {stPairing.FirstNonPlayingBoard}"
                : $" AND board < {stPairing.FirstNonPlayingBoard}";
            sqlCommand.CommandText = @" SELECT board FROM Pairing WHERE Rnd=@pRunde" + extra;
            sqlCommand.Parameters.AddWithValue("pRunde", runde);
            sqlCommand.Prepare();
            Li<int> boards = new Li<int>();
            using (var reader = sqlCommand.ExecuteReader())
                if (reader.HasRows)
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            boards.Add((int)reader.GetInt16(0));
            boards.Sort();
            int free = -1;
            int minBoardNum = isNonPlayingBoard ? stPairing.FirstNonPlayingBoard : 1;
            while (free == -1)
            {
                if (boards.IsEmpty)
                    free = minBoardNum;
                else
                {
                    var min = boards.Min();
                    if (min > minBoardNum)
                        free = minBoardNum;
                    else
                        boards.Remove(minBoardNum++);
                }
            }
            return free;
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
        public bool UpdPairingResult(int runde, int idw, int ids, SqliteInterface.Results erg)
        {
            // Ergebnis gültig? 
            // "Normal" oder "Spezialergebnis" je nach richtiger oder Pseudo-Paarung
            if (ids > 0)
            {
                if (erg != SqliteInterface.Results.WinWhite &&
                    erg != SqliteInterface.Results.WinBlack &&
                    erg != SqliteInterface.Results.Draw)
                    return false;
            }
            else
            {   // Pseudo-Paarung (Freilos, entschuldigt abwesend etc)
                if (erg == Results.WinWhite ||
                    erg == Results.WinBlack ||
                    erg == Results.Draw)
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
        public void UpdPairing_AllPairingsAndAllKeizerSumsResetValuesTa()
        {
            BeginTransaction();
            sqlCommand.CommandText = @" UPDATE Pairing  SET PTS_W=0, PTS_B=0;
                                        UPDATE Player  SET Keizer_SumPts=0 ";
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
            EndTransaction();
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
                            pList[pairingList].Round = sqLiteDataReader.IsDBNull(0) ? 0 : (int)sqLiteDataReader.GetInt16(0);
                            pList[pairingList].Board = sqLiteDataReader.IsDBNull(1) ? 0 : (int)sqLiteDataReader.GetInt16(1);
                            pList[pairingList].IdW = sqLiteDataReader.IsDBNull(2) ? 0 : (int)sqLiteDataReader.GetInt16(2);
                            pList[pairingList].IdB = sqLiteDataReader.IsDBNull(3) ? 0 : (int)sqLiteDataReader.GetInt16(3);
                            int num = sqLiteDataReader.IsDBNull(4) ? 0 : sqLiteDataReader.GetInt32(4);
                            pList[pairingList].Result = (SqliteInterface.Results)num;
                            pList[pairingList].PtsW = sqLiteDataReader.IsDBNull(5) ? 0.0f : sqLiteDataReader.GetFloat(5);
                            pList[pairingList].PtsB = sqLiteDataReader.IsDBNull(6) ? 0.0f : sqLiteDataReader.GetFloat(6);
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

        public Li<SqliteInterface.clPairing> GetClPairingLi(string sWhere, string sSortorder)
        {
            SqliteInterface.stPairing[] pList1 = new SqliteInterface.stPairing[100];
            var n = GetPairingList(ref pList1, sWhere, sSortorder);
            var li = new Li<SqliteInterface.clPairing>(pList1.Take(n).Select(sp => new clPairing(sp)));
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
                                p.Keizer_PrevPts, p.rank, f.frei, p.ratingWDelta FROM Player p 
                                " + sFrei + sWhere + sSortorder;
            using (SQLiteDataReader sqLiteDataReader = sqlCommand.ExecuteReader())
            {
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                    {
                        if (playerCount < pList.Length)
                        {
                            pList[playerCount].Id = sqLiteDataReader.IsDBNull(0) ? 0 : (int)sqLiteDataReader.GetInt16(0);
                            pList[playerCount].Name = sqLiteDataReader.IsDBNull(1) ? "" : sqLiteDataReader.GetString(1);
                            pList[playerCount].Rating = sqLiteDataReader.IsDBNull(2) ? 0 : (int)sqLiteDataReader.GetInt16(2);
                            int num = sqLiteDataReader.IsDBNull(3) ? 0 : sqLiteDataReader.GetInt32(3);
                            pList[playerCount].State = (PlayerState)num;
                            pList[playerCount].KeizerStartPts = sqLiteDataReader.IsDBNull(4) ? 0.0f : sqLiteDataReader.GetFloat(4);
                            pList[playerCount].KeizerSumPts = sqLiteDataReader.IsDBNull(5) ? 0.0f : sqLiteDataReader.GetFloat(5);
                            pList[playerCount].KeizerPrevPts = sqLiteDataReader.IsDBNull(6) ? 0.0f : sqLiteDataReader.GetFloat(6);
                            pList[playerCount].Rank = sqLiteDataReader.IsDBNull(7) ? 0 : (int)sqLiteDataReader.GetInt16(7);
                            pList[playerCount].FreeCnt = sqLiteDataReader.IsDBNull(8) ? 0 : (int)sqLiteDataReader.GetInt16(8);
                            pList[playerCount].RatingWDelta = sqLiteDataReader.IsDBNull(9) ? 0 : (int)sqLiteDataReader.GetInt16(9);
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
        /// und Status sind verwertbar und richtig und sollen exakt so ausehhen wie damals, als
        /// die Runde gelaufen ist. </summary>
        public Li<stPlayer> GetPreviousPlayerLi(string sWhere, string sSortorder, int runde)
        {
            SqliteInterface.stPlayer[] players = new stPlayer[100];
            int count = GetPlayerList(ref players, sWhere, sSortorder, GetMaxRound() + 1);
            var pairings = GetPairingLi($" WHERE Rnd={runde} ", "");
            for (int i = 0; i < count; ++i)
            {
                var playerCurrState = players[i].State;
                players[i].State = PlayerState.Retired;
                foreach (var pa in pairings)
                {
                    if (pa.IdW == players[i].Id || pa.IdB == players[i].Id)
                    {
                        // Those pairings which have not yet been played have Result == ErrUndefined. 
                        // In the player list they must show up as available. 
                        if (pa.Result.IsContainedIn(new Results[]{ Results.WinWhite, Results.Draw,
                                        Results.WinBlack, Results.FreeWin, Results.ErrUndefined }.ToLi()))
                        {
                            players[i].State = PlayerState.Available;
                            break;
                        }
                    }
                    if (pa.IdW == players[i].Id)
                    {
                        if (pa.Result.IsContainedIn(new Results[]{ Results.Hindered, Results.Excused,
                                        Results.Unexcused}.ToLi()))
                        {
                            players[i].State = (PlayerState)(int)(pa.Result);
                            break;
                        }
                    }
                }
                if (players[i].State == PlayerState.Retired)
                {
                    // Hier ist sicher, daß der Spieler nicht vorkam in dieser Runde. Also ist er entweder
                    // zurückgetreten oder ein Spätstarter. Wenn sein playerCurrState eRetired ist, dann ist er
                    // irgendwann zurückgetreten. Ich zähle den als zurückgetreten. Es ist nicht 100% korrekt, 
                    // weil es auch sein könnte, daß es ein zurückgetretener Spätstarter ist und der in der aktuell
                    // gefragten Runde noch gar nicht da war. 
                    // Das stimmt aber nicht vor Runde 1. Wenn das hier Runde 0 ist, könnte der Spieler hier auch 
                    // entschuldigt oder so sein.  
                    if (runde == 0)
                    {
                        if (playerCurrState.IsContainedIn(new PlayerState[]{
                                PlayerState.Retired, PlayerState.Excused, PlayerState.Unexcused, PlayerState.Hindered }))
                            players[i].State = playerCurrState;
                        else
                            players[i].State = PlayerState.NotYetStarted;
                    }
                    else
                        if (playerCurrState != PlayerState.Retired)
                        players[i].State = PlayerState.NotYetStarted;
                }
            }
            // Es scheint sinnlos, daß Spieler, die nie gespielt haben und zurückgetreten sind, überhaupt
            // angezeigt werden in der Spielerliste. Die werden drum hier rausgenommen. 
            var idsNeverPlayedAndRetired = new Li<stPlayer>(players.Take(count).
                    Where(p => p.State == PlayerState.Retired && CntPlayersPlayedGames(p.Id) <= 0)).
                    Select(p => p.Id).ToLi();

            var pls = runde == 0 ? new Li<stPlayer>(players.Take(count)) :
                new Li<stPlayer>(players.Take(count).Where(p => p.State != PlayerState.NotYetStarted));
            if (!idsNeverPlayedAndRetired.IsEmpty)
                pls = pls.Where(p => !p.Id.IsContainedIn(idsNeverPlayedAndRetired)).ToLi();
            return pls;
        }

        public Li<stPlayer> GetPlayerLi_Available(string sSortorder, int runde)
        {
            SqliteInterface.stPlayer[] arr = new stPlayer[100];
            int playerCount = GetPlayerList_Available(ref arr, sSortorder, runde);
            return new Li<stPlayer>(arr.Take(playerCount));
        }

        public int GetPlayerList_Available(ref stPlayer[] pList, string sSortorder, int runde)
        {
            string sWhere = " WHERE state IN (1) ";
            if (string.IsNullOrEmpty(sSortorder))
                sSortorder = " ORDER BY ID ";
            return GetPlayerList(ref pList, sWhere, sSortorder, runde);
        }

        public int GetPlayerList_NotDropped(ref stPlayer[] pList, string sOrder, int runde)
        {
            string sWhere = " WHERE state NOT IN (9) ";
            return GetPlayerList(ref pList, sWhere, sOrder, runde);
        }

        public int GetPlayerCount_NotDropped(int runde)
        {
            string sWhere = " WHERE state NOT IN (9) ";
            var li = GetPlayerLi(sWhere, "", runde);
            return li.Count;
        }

        public int GetPlayerCount()
        {
            sqlCommand.CommandText = @" SELECT Count(1) FROM Player p ";
            int playerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return playerCount;
        }
        #endregion Playerlist

        #region Localization
        public string Locl_GetText(string topic, string key)
        {
            string s = key;
            try
            {
                sqlCommand.CommandText = " SELECT text FROM config_db.LangText  WHERE code= @pCode  AND topic=@pTopic  AND key=@pKey ";
                sqlCommand.Parameters.AddWithValue("pCode", (object)LangCode);
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
            sqlCommand.Parameters.AddWithValue("pCode", (object)LangCode);
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

        public string Locl_GetPlayerStateText(SqliteInterface.PlayerState state)
        {
            // In der fLocl_GetText("PLAYERSTATE", ..) Fkt steht der Playerstate eDeleted als "A".
            // Deshalb das ToString("X", das macht Hex. Also 10 -> A
            var sState = ((int)state).ToString("X");
            return Locl_GetText("PLAYERSTATE", sState);
        }

        public SqliteInterface.Results Locl_GetGameResult(string result)
        {
            string key = Locl_FindKey("GAMERESULT", result);
            return !(key == "") ? (SqliteInterface.Results)Convert.ToInt32(key) : SqliteInterface.Results.ErrUndefined;
        }

        public string Locl_GetGameResultText(SqliteInterface.Results result) => Locl_GetText("GAMERESULT", ((int)result).ToString());

        public string Locl_GetGameResultShort(SqliteInterface.Results result)
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
            sqlCommand.CommandText = @" SELECT text FROM config_db.LangText  WHERE code= @pCode  
                        AND topic=@pTopic " + sWhereAdd + " ORDER BY (0 + key)";
            sqlCommand.Parameters.AddWithValue("pCode", (object)LangCode);
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

        public void WriteTableWHeaders2Db(TableType tt, int runde, TableW2Headers table)
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

        public TableW2Headers ReadTableWHeadersFromDb(TableType tt, int runde)
        {
            var tn = TabWHName(tt, runde);
            var table = new TableW2Headers("", tt, runde);
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

        string TabWHName(TableType tt, int runde) => "t" + tt.ToString() + "_" + runde;

        public void DelCurrentStoredTablesWHeader(int runde)
        {
            var tns = new string[] { TabWHName(TableType.Stand, runde), TabWHName(TableType.Kreuz, runde) };
            sqlCommand.CommandText = $@"
                DROP TABLE IF EXISTS {tns[0]};
                DROP TABLE IF EXISTS {tns[1]};";
            sqlCommand.ExecuteNonQuery();
        }
        #endregion Tabelle

        public string LangCode = "";
        private SQLiteConnection SQLiteMyDB;
        private SQLiteTransaction SQLiteMyTrans;
        private SQLiteCommand sqlCommand;

    }
}
