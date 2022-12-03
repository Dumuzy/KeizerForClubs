using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace KeizerForClubs
{
    public class cReportingUnit
    {
        private string sTurnier = "";
        private StreamWriter swExportDatei;
        public StreamWriter swExportDump;

        public cReportingUnit(string sTurniername) => this.sTurnier = sTurniername;

        private string GetPaarungenBasename(int runde, cSqliteInterface sqlintf) => "export\\" + this.sTurnier + "_" +
            sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + "-" + runde;

        public bool fReport_Paarungen(int runde, cSqliteInterface sqlintf)
        {
            try
            {
                string text = sqlintf.fLocl_GetText("GUI_LABEL", "Runde");
                string xmlFile = GetPaarungenBasename(runde, sqlintf) + ".xml";
                this.swExportDatei = new StreamWriter(xmlFile);
                this.swExportDatei.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                this.swExportDatei.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_pairing.xsl' ?>");
                this.swExportDatei.WriteLine("<keizer_pairing>");
                this.swExportDatei.WriteLine("<export>");
                this.swExportDatei.WriteLine("<tournament>");
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine("</tournament>");
                this.swExportDatei.WriteLine("<title>");
                this.swExportDatei.WriteLine(text + " " + runde.ToString());
                this.swExportDatei.WriteLine("</title>");
                cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
                int pairingList = sqlintf.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
                for (int index = 0; index < pairingList; ++index)
                {
                    this.swExportDatei.WriteLine("<board>");
                    this.swExportDatei.WriteLine("<nr>" + (index + 1).ToString() + ".</nr>");
                    this.swExportDatei.WriteLine("<w>" + sqlintf.fGetPlayerName(pList[index].id_w) + "</w>");
                    this.swExportDatei.WriteLine("<b>" + sqlintf.fGetPlayerName(pList[index].id_b) + "</b>");
                    this.swExportDatei.WriteLine("<res>" + sqlintf.fLocl_GetGameResultText(pList[index].result) + "</res>");
                    this.swExportDatei.WriteLine("</board>");
                }
                this.swExportDatei.WriteLine("<footer>");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.WriteLine("</footer>");
                this.swExportDatei.WriteLine("</export>");
                this.swExportDatei.WriteLine("</keizer_pairing>");
                this.swExportDatei.Close();
                frmMainform.OpenWithDefaultApp(xmlFile);
                return this.fReport_Paarungen_TXT(runde, sqlintf);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        public bool fReport_Paarungen_TXT(int runde, cSqliteInterface sqlintf)
        {
            try
            {
                string text = sqlintf.fLocl_GetText("GUI_LABEL", "Runde");
                this.swExportDatei = new StreamWriter(GetPaarungenBasename(runde, sqlintf) + ".txt");
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine(text + " " + runde.ToString());
                this.swExportDatei.WriteLine(" ");
                cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
                int pairingList = sqlintf.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
                for (int index = 0; index < pairingList; ++index)
                {
                    string str = ((index + 1).ToString("00") + ".").PadLeft(4) + "  " + sqlintf.fGetPlayerName(pList[index].id_w).PadRight(20) + " - " + sqlintf.fGetPlayerName(pList[index].id_b).PadRight(20) + sqlintf.fLocl_GetGameResultText(pList[index].result);
                    this.swExportDatei.WriteLine(str);
                    if (this.swExportDump != null)
                        this.swExportDump.WriteLine(str);
                }
                this.swExportDatei.WriteLine(" ");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.Close();
                return true;
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        private string GetFileTabellenstandBasename(cSqliteInterface sqlintf) =>
            "export\\" + this.sTurnier + "_" + sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + "-" + sqlintf.fGetMaxRound();

        public bool fReport_Tabellenstand(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            try
            {
                string xmlFile = GetFileTabellenstandBasename(sqlintf) + ".xml";
                string str2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
                Directory.CreateDirectory(Path.GetDirectoryName(xmlFile));
                this.swExportDatei = new StreamWriter(xmlFile);
                this.swExportDatei.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                this.swExportDatei.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_simpletable.xsl' ?>");
                this.swExportDatei.WriteLine("<keizer_table>");
                this.swExportDatei.WriteLine("<export>");
                this.swExportDatei.WriteLine("<tournament>");
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine("</tournament>");
                this.swExportDatei.WriteLine("<title>");
                this.swExportDatei.WriteLine(sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2);
                this.swExportDatei.WriteLine("</title>");
                cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
                int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
                for (int index = 0; index < playerList; ++index)
                {
                    if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                    {
                        this.swExportDatei.WriteLine("<player>");
                        this.swExportDatei.WriteLine("<nr>" + num1.ToString() + ".</nr>");
                        this.swExportDatei.WriteLine("<name>" + pList[index].name + "</name>");
                        this.swExportDatei.WriteLine("<keizer_sum>" + pList[index].Keizer_SumPts.ToString() + "</keizer_sum>");
                        this.swExportDatei.WriteLine("<game_pts>" + sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString() + "</game_pts>");
                        this.swExportDatei.WriteLine("</player>");
                        ++num1;
                    }
                }
                this.swExportDatei.WriteLine("<footer>");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.WriteLine("</footer>");
                this.swExportDatei.WriteLine("</export>");
                this.swExportDatei.WriteLine("</keizer_table>");
                this.swExportDatei.Close();
                frmMainform.OpenWithDefaultApp(xmlFile);
                this.fReport_Tabellenstand_Voll_CSV(sqlintf);
                return this.fReport_Tabellenstand_TXT(sqlintf);
            }
            catch (Exception ex)
            {
                int num2 = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        private bool fReport_Tabellenstand_TXT(cSqliteInterface sqlintf, bool bWriteDump = false)
        {
            int num1 = 1;
            try
            {
                string text = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc");
                string path = GetFileTabellenstandBasename(sqlintf) + ".txt";
                string str = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound().ToString();
                this.swExportDatei = new StreamWriter(path);
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine(text + " " + str);
                this.swExportDatei.WriteLine(" ");
                cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
                int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
                for (int index = 0; index < playerList; ++index)
                {
                    if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                    {
                        this.swExportDatei.WriteLine((num1.ToString("00") + ".").PadLeft(4) + "  " + pList[index].name.PadRight(25) + pList[index].Keizer_SumPts.ToString().PadLeft(5) + sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString().PadLeft(6));
                        ++num1;
                    }
                }
                this.swExportDatei.WriteLine(" ");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.Close();
                return true;
            }
            catch (Exception ex)
            {
                int num2 = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        public bool fReport_Tabellenstand_Voll_CSV(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            int maxRound = sqlintf.fGetMaxRound();
            cSqliteInterface.stPlayer[] pList1 = new cSqliteInterface.stPlayer[100];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPairing[] pList4 = new cSqliteInterface.stPairing[1];
            try
            {
                this.swExportDatei = new StreamWriter(GetFileTabellenstandBasename(sqlintf) + ".csv");
                int playerList = sqlintf.fGetPlayerList(ref pList1, "", " ORDER BY Keizer_SumPts desc, Rating desc ");
                this.swExportDatei.WriteLine("Platz;Name;Rating;Keizer-RangPkte;Keizer-SummePkte;PartiePkt;Status;Runden...;");
                for (int index1 = 0; index1 < playerList; ++index1)
                {
                    string str1;
                    if (pList1[index1].state != cSqliteInterface.ePlayerState.eRetired)
                    {
                        str1 = num1.ToString("00") + ";";
                        ++num1;
                    }
                    else
                        str1 = "(ret);";
                    string name = pList1[index1].name;
                    string str2 = str1 + name + ";" + pList1[index1].rating.ToString() + ";" + pList1[index1].Keizer_StartPts.ToString() + ";" + pList1[index1].Keizer_SumPts.ToString() + ";" + sqlintf.fGetPlayer_PartiePunkte(pList1[index1].id).ToString() + ";" + sqlintf.fLocl_GetPlayerStateText(pList1[index1].state) + ";";
                    for (int index2 = 1; index2 <= maxRound; ++index2)
                    {
                        string sWhere = " WHERE (PID_W=" + pList1[index1].id.ToString() + " OR    PID_B=" + pList1[index1].id.ToString() + ")  AND rnd=" + index2.ToString();
                        string str3;
                        if (sqlintf.fGetPairingList(ref pList4, sWhere, "  ") > 0)
                        {
                            sqlintf.fGetPlayerList(ref pList2, " WHERE ID=" + (object)pList4[0].id_w, " ");
                            sqlintf.fGetPlayerList(ref pList3, " WHERE ID=" + (object)pList4[0].id_b, " ");
                            string str4 = sqlintf.fLocl_GetGameResultText(pList4[0].result) + " ";
                            if (pList1[index1].id == pList4[0].id_w)
                            {
                                string str5 = str4 + "w ";
                                str3 = (pList3[0].state != cSqliteInterface.ePlayerState.eRetired ? str5 + " " + pList3[0].rank.ToString() + " " : str5 + " (ret) ") + "pkt=" + pList4[0].pts_w.ToString() + " ";
                            }
                            else
                            {
                                string str6 = str4 + "b ";
                                str3 = (pList2[0].state != cSqliteInterface.ePlayerState.eRetired ? str6 + " " + pList2[0].rank.ToString() + " " : str6 + " (ret) ") + "pkt=" + pList4[0].pts_b.ToString() + " ";
                            }
                        }
                        else
                            str3 = "_";
                        str2 = str2 + str3 + ";";
                    }
                    if (this.swExportDump != null)
                        this.swExportDump.WriteLine(str2);
                    this.swExportDatei.WriteLine(str2);
                }
                this.swExportDatei.WriteLine(" ");
                this.swExportDatei.Close();
                return true;
            }
            catch (Exception ex)
            {
                int num2 = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        private string GetTeilnehmerBasename(cSqliteInterface sqlintf) => "export\\" + this.sTurnier + "_" +
                sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + "-" + sqlintf.fGetMaxRound();

        public bool fReport_Teilnehmer(cSqliteInterface sqlintf)
        {
            try
            {
                string text = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");
                string xmlFile = GetTeilnehmerBasename(sqlintf) + ".xml";
                this.swExportDatei = new StreamWriter(xmlFile);
                this.swExportDatei.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                this.swExportDatei.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_player.xsl' ?>");
                this.swExportDatei.WriteLine("<keizer_player>");
                this.swExportDatei.WriteLine("<export>");
                this.swExportDatei.WriteLine("<tournament>");
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine("</tournament>");
                this.swExportDatei.WriteLine("<title>");
                this.swExportDatei.WriteLine(text);
                this.swExportDatei.WriteLine("</title>");
                cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
                int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY ID ");
                for (int index = 0; index < playerList; ++index)
                {
                    this.swExportDatei.WriteLine("<player>");
                    this.swExportDatei.WriteLine("<nr>" + (index + 1).ToString() + ".</nr>");
                    this.swExportDatei.WriteLine("<name>" + pList[index].name + "</name>");
                    this.swExportDatei.WriteLine("<rating>" + pList[index].rating.ToString() + "</rating>");
                    this.swExportDatei.WriteLine("<state>" + sqlintf.fLocl_GetPlayerStateText(pList[index].state) + "</state>");
                    this.swExportDatei.WriteLine("</player>");
                }
                this.swExportDatei.WriteLine("<footer>");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.WriteLine("</footer>");
                this.swExportDatei.WriteLine("</export>");
                this.swExportDatei.WriteLine("</keizer_player>");
                this.swExportDatei.Close();
                frmMainform.OpenWithDefaultApp(xmlFile);
                return this.fReport_Teilnehmer_TXT(sqlintf);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }

        private bool fReport_Teilnehmer_TXT(cSqliteInterface sqlintf, bool bWriteDump = false)
        {
            try
            {
                string text = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");
                this.swExportDatei = new StreamWriter(GetTeilnehmerBasename(sqlintf) + ".txt");
                this.swExportDatei.WriteLine(this.sTurnier);
                this.swExportDatei.WriteLine(text);
                this.swExportDatei.WriteLine(" ");
                cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
                int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY ID ");
                for (int index = 0; index < playerList; ++index)
                    this.swExportDatei.WriteLine(((index + 1).ToString("00") + ".").PadLeft(4) + "  " + pList[index].name.PadRight(25) + pList[index].rating.ToString("0000").PadLeft(5) + sqlintf.fLocl_GetPlayerStateText(pList[index].state).PadLeft(25));
                this.swExportDatei.WriteLine(" ");
                this.swExportDatei.WriteLine("KeizerForClubs " + DateTime.Now.ToShortDateString());
                this.swExportDatei.Close();
                return true;
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
        }
    }
}
