// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Kbg.NppPluginNET.PluginInfrastructure;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;
using System.Data.SQLite;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.InteropServices.ComTypes;

namespace Kbg.NppPluginNET
{
    /// <summary>
    /// Integration layer as the demo app uses the pluginfiles as soft-links files.
    /// This is different to normal plugins that would use the project template and get the files directly.
    /// </summary>
    class Main
    {
        static internal void CommandMenuInit()
        {
            Kbg.Demo.Namespace.Main.CommandMenuInit();
        }

        static internal void PluginCleanUp()
        {
            Kbg.Demo.Namespace.Main.PluginCleanUp();
        }

        static internal void SetToolBarIcon()
        {
            Kbg.Demo.Namespace.Main.SetToolBarIcon();
            Kbg.Demo.Namespace.Main.SetToolBarIconFormat();
            Kbg.Demo.Namespace.Main.SetToolBarIconUnFormat();
            Kbg.Demo.Namespace.Main.SetToolBarIconX12Format();
            Kbg.Demo.Namespace.Main.SetToolBarIconX12UnFormat();

        }

        public static void OnNotification(ScNotification notification)
        {
            if (notification.Header.Code == (uint)SciMsg.SCN_CHARADDED)
            {
                Kbg.Demo.Namespace.Main.doInsertHtmlCloseTag((char)notification.Character);
            }
            //MI 20210102 intecept dblClick on the window text
            /*if (notification.Header.Code == (uint)SciMsg.SCN_DOUBLECLICK)
            {
                Kbg.Demo.Namespace.Main.EDIAnalizeSelectedLine();
                Kbg.Demo.Namespace.Main.frmGoToLine.displaySegmentOnListview();
               //Demo.Namespace.frmGoToLine.
            }
            */

            Boolean isFormVisible = false;

            if (notification.Header.Code == (uint)SciMsg.SCN_DOUBLECLICK)
            {
                try
                {
                    isFormVisible = Kbg.Demo.Namespace.Main.frmGoToLine.Visible;
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

                if (isFormVisible == true)
                    { 
                        Kbg.Demo.Namespace.Main.EDIAnalizeSelectedLine();
                        Kbg.Demo.Namespace.Main.frmGoToLine.displaySegmentOnListview();
                    }

            }

            // if (!frmGoToLine.Visible)
        }

        internal static string PluginName { get { return Kbg.Demo.Namespace.Main.PluginName; } }
    }
}

namespace Kbg.Demo.Namespace
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "Npp EDI Analyzer";
        static string iniFilePath = null;
        static string sectionName = "Insert Extension";
        static string keyName = "doCloseTag";
        static bool doCloseTag = false;
        static string sessionFilePath = @"C:\text.session";
        static public frmGoToLine frmGoToLine = null;
        static public frmDatabaseEditor frmDatabaseEditor = null;
        static internal int idFrmGotToLine = -1;
        static internal int idFrmDatabaseEditor = -1;
        static internal int idMnuFormat = -1;
        static internal int idMnuUnformat = -1;
        static internal int idMnuX12Format = -1;
        static internal int idMnuX12Unformat = -1;
        static internal int idMnuLexerLanguage = -1;
        static internal int idMnuDatabaseEditor = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmpFormat = Properties.Resources.format;
        static Bitmap tbBmpUnFormat = Properties.Resources.unformat;
        static Bitmap tbBmpFormatX12 = Properties.Resources.formatX12;
        static Bitmap tbBmpUnFormatX12 = Properties.Resources.unformatX12;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIcon = null;
        static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
        static INotepadPPGateway notepad = new NotepadPPGateway();
        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // Initialization of your plugin commands
            // You should fill your plugins commands here

            //
            // Firstly we get the parameters from your plugin config file (if any)
            //

            // get path of plugin configuration
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            // if config path doesn't exist, we create it
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }

            // make your plugin config file full file path name
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");

            // get the parameter value from plugin config
            doCloseTag = (Win32.GetPrivateProfileInt(sectionName, keyName, 0, iniFilePath) != 0);

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );

            //PluginBase.SetCommand(0, "Hello Notepad++", hello);

            //PluginBase.SetCommand(1, "Hello (with FX)", helloFX);
            //PluginBase.SetCommand(2, "What is Notepad++?", WhatIsNpp);

            // Here you insert a separator
            //PluginBase.SetCommand(3, "---", null);

            // Shortcut :
            // Following makes the command bind to the shortcut Alt-F
            /*
            PluginBase.SetCommand(4, "Current Full Path", insertCurrentFullPath, new ShortcutKey(false, true, false, Keys.F));
            PluginBase.SetCommand(5, "Current File Name", insertCurrentFileName);
            PluginBase.SetCommand(6, "Current Directory", insertCurrentDirectory);
            PluginBase.SetCommand(7, "Date && Time - short format", insertShortDateTime);
            PluginBase.SetCommand(8, "Date && Time - long format", insertLongDateTime);

            PluginBase.SetCommand(9, "Close HTML/XML tag automatically", checkInsertHtmlCloseTag, new ShortcutKey(false, true, false, Keys.Q), doCloseTag);

            PluginBase.SetCommand(10, "---", null);

            PluginBase.SetCommand(11, "Get File Names Demo", getFileNamesDemo);
            PluginBase.SetCommand(12, "Get Session File Names Demo", getSessionFileNamesDemo);
            PluginBase.SetCommand(13, "Save Current Session Demo", saveCurrentSessionDemo);

            PluginBase.SetCommand(14, "---", null);
            
            PluginBase.SetCommand(15, "Dockable Dialog Demo", DockableDlgDemo); idFrmGotToLine = 15;

            PluginBase.SetCommand(16, "---", null);

            PluginBase.SetCommand(17, "Print Scroll and Row Information", PrintScrollInformation);
            */

            PluginBase.SetCommand(0, "Structure View", StructureView); idFrmGotToLine = 0;
            PluginBase.SetCommand(1, "Format EDIFACT (Alt+Down)", formatEdifact, new ShortcutKey(false, true, false, Keys.Down)); idMnuFormat = 1;
            PluginBase.SetCommand(2, "Un-Format EDIFACT (Alt+Up)", unFormatEdifact, new ShortcutKey(false, true, false, Keys.Up)); idMnuUnformat = 2;
            PluginBase.SetCommand(3, "Format X12 (Alt+Left)", formatX12, new ShortcutKey(false, true, false, Keys.Left)); idMnuX12Format = 3;
            PluginBase.SetCommand(4, "Un-Format X12 (Alt+Right)", unformatX12, new ShortcutKey(false, true, false, Keys.Right)); idMnuX12Unformat = 4;
            PluginBase.SetCommand(5, "Add X12 Language (Must Restart)", addX12Language); idMnuLexerLanguage = 5;
            PluginBase.SetCommand(6, "EDI Database Editor", DatabaseEditor); idMnuDatabaseEditor = 6;
        }

        static void formatEdifact()
        {
            Util.ReplaceAll("'", "'\r\n", editor);
        }

        static void unFormatEdifact()
        {
            Util.ReplaceAll("'\r\n", "'", editor);
            Util.ReplaceAll("'\n", "'", editor);
        }

        static void formatX12()
        {
            Util.ReplaceAll("~", "~\r\n", editor);
        }

        static void unformatX12()
        {
            Util.ReplaceAll("~\r\n", "~", editor);
            Util.ReplaceAll("~\n", "~", editor);
        }

        static void addX12Language()
        {
            string userDefLangFolder = @"%Appdata%\Notepad++\userDefineLangs\X12.xml";
            userDefLangFolder = Environment.ExpandEnvironmentVariables(userDefLangFolder);

            string X12LanguageString = @"<NotepadPlus>
    <UserLang name=""X12"" ext="""">
        <Settings>
            <Global caseIgnored=""no"" />
            <TreatAsSymbol comment=""no"" commentLine=""no"" />
            <Prefix words1=""no"" words2=""no"" words3=""no"" words4=""no"" />
        </Settings>
        <KeywordLists>
            <Keywords name=""Delimiters"">000000</Keywords>
            <Keywords name=""Folder+""></Keywords>
            <Keywords name=""Folder-""></Keywords>
            <Keywords name=""Operators"">* : ^ ~ +</Keywords>
            <Keywords name=""Comment""></Keywords>
            <Keywords name=""Words1"">ISA IEA GS GE ST SE</Keywords>
            <Keywords name=""Words2"">AAA ACT ADX AK1 AK2 AK3 AK4 AK5 AK6 AK7 AK8 AK9 AMT AT1 AT2 AT3 AT4 AT5 AT6 AT7 AT8 AT9 AT8 AT9 B2 B2A BEG BGN BHT BPR CAS CL1 CLM CLP CN1 COB CR1 CR2 CL3 CL4 CR5 CR6 CRC CTX CUR DMG DN1 DN2 DSB DTM DTP EB EC ENT EQ FRM G61 G62 HCP HCR HD HI HL HLH HSD ICM IDC III IK3 IK4 IK5 INS IT1 K1 K2 K3 L3 L11 LIN LQ LUI LX MEA MIA MOA MPI MSG N1 N2 N3 N4 NM1 NTE NX OI PAT PER PLA PLB PO1 PRV PS1 PWK QTY RDM REF RMR S5 SAC SBR SLN STC SV1 SV2 SV3 SV4 SV5 SVC SVD TA1 TOO TRN TS2 TS3 UM</Keywords>
            <Keywords name=""Words3"">LS LE</Keywords>
            <Keywords name=""Words4"">00 000 0007 001 0010 0019 002 0022 003 004 00401 004010 004010X061 004010X061A1 004010X091 004010X091A1 004010X092 004010X092A1 004010X093 004010X093A1 004010X094 004010X094A1 004010X095 004010X095A1 004010X096 004010X096A1 004010X098 004010X098A1 005 00501 005010 005010X212 005010X217 005010X218 005010X220 005010X220A1 005010X221 005010X221A1 005010X222 005010X222A1 005010X223 005010X223A1 005010X223A2 005010X224 005010X224A1 005010X224A2 005010X230 005010X231 005010X279 005010X279A1 006 007 0078 008 009 01 010 011 012 013 014 015 016 017 018 019 02 020 021 022 023 024 025 026 027 028 029 03 030 031 032 035 036 04 05 050 06 07 08 09 090 091 096 097 0B 0F 0K 102 119 139 150 151 152 18 193 194 196 198 1A 1B 1C 1D 1E 1G 1H 1I 1J 1K 1L 1O 1P 1Q 1R 1S 1T 1U 1V 1W 1X 1Y 1Z 200 232 233 270 271 276 277 278 286 290 291 292 295 296 297 2A 2B 2C 2D 2E 2F 2I 2J 2K 2L 2P 2Q 2S 2U 2Z 30 300 301 303 304 307 31 314 318 330 336 337 338 339 340 341 342 343 344 345 346 347 348 349 350 351 356 357 36 360 361 374 382 383 385 386 388 393 394 3A 3C 3D 3E 3F 3G 3H 3I 3J 3K 3L 3M 3N 3O 3P 3Q 3R 3S 3T 3U 3V 3W 3X 3Y 3Z 40 405 41 417 431 434 435 438 439 441 442 444 446 45 452 453 454 455 456 458 46 461 463 471 472 473 474 480 481 484 492 4A 4B 4C 4D 4E 4F 4G 4H 4I 4J 4K 4L 4M 4N 4O 4P 4Q 4R 4S 4U 4V 4W 4X 4Y 4Z 539 540 543 573 580 581 582 598 5A 5B 5C 5D 5E 5F 5G 5H 5I 5J 5K 5L 5M 5N 5O 5P 5Q 5R 5S 5T 5U 5V 5W 5X 5Y 5Z 607 636 695 6A 6B 6C 6D 6E 6F 6G 6H 6I 6J 6K 6L 6M 6N 6O 6P 6Q 6R 6S 6U 6V 6W 6X 6Y 70 71 72 738 739 74 77 771 7C 82 820 831 834 835 837 85 850 866 87 8H 8U 8W 938 997 999 9A 9B 9C 9D 9E 9F 9H 9J 9K 9V 9X A0 A1 A172 A2 A3 A4 A5 A6 A7 A8 A9 AA AAE AAG AAH AAJ AB ABB ABC ABF ABJ ABK ABN AC ACH AD ADD ADM AE AF AG AH AI AJ AK AL ALC ALG ALS AM AN AO AP APC APR AQ AR AS AT AU AV AX AY AZ B1 B2 B3 B4 B6 B680 B7 B9 BA BB BBQ BBR BC BD BE BF BG BH BI BJ BK BL BLT BM BN BO BOP BP BPD BQ BR BS BT BTD BU BV BW BX BY BZ C1 C2 C3 C4 C5 C6 C7 C8 C9 CA CAD CB CC CCP CD CE CER CF CG CH CHD CHK CI CJ CK CL CLI CLM01 CM CN CNJ CO CON CP CQ CR CS CT CV CW CX CY CZ D2 D3 D8 D9 D940 DA DB DCP DD DEN DEP DG DGN DH DI DJ DK DM DME DN DO DP DQ DR DS DT DX DY E1 E1D E2 E2D E3 E3D E5D E6D E7 E7D E8 E8D E9 E9D EA EAF EBA ECH ED EI EJ EL EM EMP EN ENT01 EO EP EPO ER ES ESP ET EV EW EX EXS EY F1 F2 F3 F4 F5 F6 F8 FA FAC FAM FB FC FD FE FF FH FI FJ FK FL FM FO FS FT FWT FX FY G0 G1 G2 G3 G4 G5 G740 G8 G9 GB GD GF GH GI GJ GK GM GN GO GP GR GW GT GY H1 H6 HC HE HF HH HJ HLT HM HMO HN HO HP HPI HR HS HT I10 I11 I12 I13 I3 I4 I5 I6 I7 I8 I9 IA IAT IC ID IE IF IG IH II IJ IK IL IN IND IP IR IS IV J1 J3 J6 JD JP KG KH KW L1 L2 L3 L4 L5 L6 LA LB LC LD LI LM LOI LR LT LU LTC LTD M1 M2 M3 M4 M5 M6 M7 M8 MA MB MC MD ME MED MH MI MJ ML MM MN MO MOD MP MR MRC MS MSC MT N5 N6 N7 N8 NA ND NE NF NH NI NM109 NL NN NON NQ NR NS NT NTR NU OA OB OC OD ODT OE OF OG OL ON OP OR OT OU OX OZ P0 P1 P2 P3 P4 P5 P6 P7 PA PB PC PD PDG PE PI PID PL PN PO POS PP PPO PQ PR PRA PRP PS PT PU PV PW PXC PY PZ Q4 QA QB QC QD QE QH QK QL QM QN QO QQ QR QS QV QY R1 R2 R3 R4 RA RB RC RD8 RE REC RET RF RGA RHB RLH RM RN RNH RP RR RT RU RW RX S1 S2 S3 S4 S5 S6 S7 S8 S9 SA SB SC SD SEP SET SFM SG SJ SK SL SP SPC SPO SPT SS STD SU SV SWT SX SY SZ T1 T10 T11 T12 T2 T3 T4 T5 T6 T7 T8 T9 TC TD TE TF TJ TL TM TN TNJ TO TPO TQ TR TRN02 TS TT TTP TU TV TWO TZ UC UH UI UK UN UP UPI UR UT V1 V5 VA VER VIS VN VO VS VV VY W1 WA WC WK WO WP WR WU WW X12 X3 X4 X5 X9 XM XN XP XT XV XX XX1 XX2 XZ Y2 Y4 YR YT YY Z6 ZB ZH ZK ZL ZM ZN ZO ZV ZX ZZ</Keywords>
        </KeywordLists>
        <Styles>
            <WordsStyle name=""DEFAULT"" styleID=""11"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""FOLDEROPEN"" styleID=""12"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""FOLDERCLOSE"" styleID=""13"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""KEYWORD1"" styleID=""5"" fgColor=""0000FF"" bgColor=""FFFFFF"" fontName="""" fontStyle=""1"" />
            <WordsStyle name=""KEYWORD2"" styleID=""6"" fgColor=""800000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""1"" />
            <WordsStyle name=""KEYWORD3"" styleID=""7"" fgColor=""00FF00"" bgColor=""FFFFFF"" fontName="""" fontStyle=""1"" />
            <WordsStyle name=""KEYWORD4"" styleID=""8"" fgColor=""8000FF"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""COMMENT"" styleID=""1"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""COMMENT LINE"" styleID=""2"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""NUMBER"" styleID=""4"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""OPERATOR"" styleID=""10"" fgColor=""FF00FF"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""DELIMINER1"" styleID=""14"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""DELIMINER2"" styleID=""15"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
            <WordsStyle name=""DELIMINER3"" styleID=""16"" fgColor=""000000"" bgColor=""FFFFFF"" fontName="""" fontStyle=""0"" />
        </Styles>
    </UserLang>
</NotepadPlus>";

            File.WriteAllText(userDefLangFolder, X12LanguageString);

            // To-Do: For language in lexer language, check for X12, if it does not exist, save the X12 string as a file in the user defined languages folder
            // Find out if there is a way to either automatically restart NPP
        }
       
        static void PrintScrollInformation()
        {
            ScrollInfo scrollInfo = editor.GetScrollInfo(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_TRACKPOS | ScrollInfoMask.SIF_PAGE, ScrollInfoBar.SB_VERT);
            var scrollRatio = (double)scrollInfo.nTrackPos / (scrollInfo.nMax - scrollInfo.nPage);
            var scrollPercentage = Math.Min(scrollRatio, 1) * 100;
            editor.ReplaceSel($@"The maximum row in the current document was {scrollInfo.nMax + 1}.
A maximum of {scrollInfo.nPage} rows is visible at a time.
The current scroll ratio is {Math.Round(scrollPercentage, 2)}%.
");
        }

        static internal void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idFrmGotToLine]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void SetToolBarIconFormat()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmpFormat.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMnuFormat]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void SetToolBarIconUnFormat()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmpUnFormat.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMnuUnformat]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void SetToolBarIconX12Format()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmpFormatX12.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMnuX12Format]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void SetToolBarIconX12UnFormat()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmpUnFormatX12.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMnuX12Unformat]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }


        static internal void PluginCleanUp()
        {
            Win32.WritePrivateProfileString(sectionName, keyName, doCloseTag ? "1" : "0", iniFilePath);
        }
        #endregion

        #region " Menu functions "
        static void hello()
        {
            notepad.FileNew();
            editor.SetText("Hello, Notepad++...from.NET!");
            var rest = editor.GetLine(0);
            editor.SetText(rest + rest + rest);
        }

        static void helloFX()
        {
            hello();
            new Thread(callbackHelloFX).Start();
        }

        static void callbackHelloFX()
        {
            int currentZoomLevel = editor.GetZoom();
            int i = currentZoomLevel;
            for (int j = 0; j < 4; j++)
            {
                for (; i >= -10; i--)
                {
                    editor.SetZoom(i);
                    Thread.Sleep(30);
                }
                Thread.Sleep(100);
                for (; i <= 20; i++)
                {
                    Thread.Sleep(30);
                    editor.SetZoom(i);
                }
                Thread.Sleep(100);
            }
            for (; i >= currentZoomLevel; i--)
            {
                Thread.Sleep(30);
                editor.SetZoom(i);
            }
        }

        static void WhatIsNpp()
        {
            string text2display = "Notepad++ is a free (as in \"free speech\" and also as in \"free beer\") " +
                "source code editor and Notepad replacement that supports several languages.\n" +
                "Running in the MS Windows environment, its use is governed by GPL License.\n\n" +
                "Based on a powerful editing component Scintilla, Notepad++ is written in C++ and " +
                "uses pure Win32 API and STL which ensures a higher execution speed and smaller program size.\n" +
                "By optimizing as many routines as possible without losing user friendliness, Notepad++ is trying " +
                "to reduce the world carbon dioxide emissions. When using less CPU power, the PC can throttle down " +
                "and reduce power consumption, resulting in a greener environment.";
            new Thread(new ParameterizedThreadStart(callbackWhatIsNpp)).Start(text2display);
        }

        static void callbackWhatIsNpp(object data)
        {
            string text2display = (string)data;
            notepad.FileNew();

            Random srand = new Random(DateTime.Now.Millisecond);
            int rangeMin = 0;
            int rangeMax = 250;
            for (int i = 0; i < text2display.Length; i++)
            {
                Thread.Sleep(srand.Next(rangeMin, rangeMax) + 30);
                editor.AppendTextAndMoveCursor(text2display[i].ToString());
            }
        }

        static void insertCurrentFullPath()
        {
            insertCurrentPath(NppMsg.FULL_CURRENT_PATH);
        }
        static void insertCurrentFileName()
        {
            insertCurrentPath(NppMsg.FILE_NAME);
        }
        static void insertCurrentDirectory()
        {
            insertCurrentPath(NppMsg.CURRENT_DIRECTORY);
        }
        static void insertCurrentPath(NppMsg which)
        {
            NppMsg msg = NppMsg.NPPM_GETFULLCURRENTPATH;
            if (which == NppMsg.FILE_NAME)
                msg = NppMsg.NPPM_GETFILENAME;
            else if (which == NppMsg.CURRENT_DIRECTORY)
                msg = NppMsg.NPPM_GETCURRENTDIRECTORY;

            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)msg, 0, path);

            editor.ReplaceSel(path.ToString());
        }

        static void insertShortDateTime()
        {
            insertDateTime(false);
        }
        static void insertLongDateTime()
        {
            insertDateTime(true);
        }
        static void insertDateTime(bool longFormat)
        {
            string dateTime = string.Format("{0} {1}", DateTime.Now.ToShortTimeString(), longFormat ? DateTime.Now.ToLongDateString() : DateTime.Now.ToShortDateString());
            editor.ReplaceSel(dateTime);
        }

        static void checkInsertHtmlCloseTag()
        {
            doCloseTag = !doCloseTag;

            int i = Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[9]._cmdID,
                Win32.MF_BYCOMMAND | (doCloseTag ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
        }

        static Regex regex = new Regex(@"[\._\-:\w]", RegexOptions.Compiled);

        static internal void doInsertHtmlCloseTag(char newChar)
        {
            LangType docType = LangType.L_TEXT;
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTLANGTYPE, 0, ref docType);
            bool isDocTypeHTML = (docType == LangType.L_HTML || docType == LangType.L_XML || docType == LangType.L_PHP);

            if (!doCloseTag || !isDocTypeHTML)
                return;

            if (newChar != '>')
                return;

            int bufCapacity = 512;
            var pos = editor.GetCurrentPos();
            int currentPos = pos;
            int beginPos = currentPos - (bufCapacity - 1);
            int startPos = (beginPos > 0) ? beginPos : 0;
            int size = currentPos - startPos;

            if (size < 3)
                return;

            using (TextRange tr = new TextRange(startPos, currentPos, bufCapacity))
            {
                editor.GetTextRange(tr);
                string buf = tr.lpstrText;

                if (buf[size - 2] == '/')
                    return;

                int pCur = size - 2;
                while ((pCur > 0) && (buf[pCur] != '<') && (buf[pCur] != '>'))
                    pCur--;

                if (buf[pCur] == '<')
                {
                    pCur++;

                    var insertString = new StringBuilder("</");

                    while (regex.IsMatch(buf[pCur].ToString()))
                    {
                        insertString.Append(buf[pCur]);
                        pCur++;
                    }
                    insertString.Append('>');

                    if (insertString.Length > 3)
                    {
                        editor.BeginUndoAction();
                        editor.ReplaceSel(insertString.ToString());
                        editor.SetSel(pos, pos);
                        editor.EndUndoAction();
                    }
                }
            }
        }

        static void getFileNamesDemo()
        {
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 0);
            MessageBox.Show(nbFile.ToString(), "Number of opened files:");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, cStrArray.NativePointer, nbFile) != IntPtr.Zero)
                    foreach (string file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void getSessionFileNamesDemo()
        {
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBSESSIONFILES, 0, sessionFilePath);

            if (nbFile < 1)
            {
                MessageBox.Show("Please modify \"sessionFilePath\" in \"Demo.cs\" in order to point to a valid session file", "Error");
                return;
            }
            MessageBox.Show(nbFile.ToString(), "Number of session files:");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETSESSIONFILES, cStrArray.NativePointer, sessionFilePath) != IntPtr.Zero)
                    foreach (string file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void saveCurrentSessionDemo()
        {
            string sessionPath = Marshal.PtrToStringUni(Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SAVECURRENTSESSION, 0, sessionFilePath));
            if (!string.IsNullOrEmpty(sessionPath))
                MessageBox.Show(sessionPath, "Saved Session File :", MessageBoxButtons.OK);
        }

        static void StructureView()
        {
            // Dockable Dialog Demo
            // 
            // This demonstration shows you how to do a dockable dialog.
            // You can create your own non dockable dialog - in this case you don't nedd this demonstration.
            if (frmGoToLine == null)
            {
                frmGoToLine = new frmGoToLine(editor);

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmGoToLine.Handle;
                _nppTbData.pszName = "Npp EDI Analyzer";
                // the dlgDlg should be the index of funcItem where the current function pointer is in
                // this case is 15.. so the initial value of funcItem[15]._cmdID - not the updated internal one !
                _nppTbData.dlgID = idFrmGotToLine;
                // define the default docking behaviour
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                // Following message will toogle both menu item state and toolbar button
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmGotToLine]._cmdID, 1);
            }
            else
            {
                if (!frmGoToLine.Visible)
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmGoToLine.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmGotToLine]._cmdID, 1);
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, frmGoToLine.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmGotToLine]._cmdID, 0);
                }
            }
            //frmGoToLine.textBox1.Focus();
        }

        static void DatabaseEditor()
        {
            // Dockable Dialog Demo
            // 
            // This demonstration shows you how to do a dockable dialog.
            // You can create your own non dockable dialog - in this case you don't nedd this demonstration.
            if (frmDatabaseEditor == null)
            {
                frmDatabaseEditor = new frmDatabaseEditor(editor);

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmDatabaseEditor.Handle;
                _nppTbData.pszName = "NPP EDI Database Editor";
                // the dlgDlg should be the index of funcItem where the current function pointer is in
                // this case is 15.. so the initial value of funcItem[15]._cmdID - not the updated internal one !
                _nppTbData.dlgID = idFrmDatabaseEditor;
                // define the default docking behaviour
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                // Following message will toogle both menu item state and toolbar button
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmDatabaseEditor]._cmdID, 1);
            }
            else
            {
                if (!frmDatabaseEditor.Visible)
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmGoToLine.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmDatabaseEditor]._cmdID, 1);
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, frmGoToLine.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idFrmDatabaseEditor]._cmdID, 0);
                }
            }
            //frmGoToLine.textBox1.Focus();
        }
        #endregion


        public static List<SegmentStructure> SegmentList = new List<SegmentStructure>();


        static internal void EDIAnalizeSelectedLine()
        {

            //clear
            SegmentList.Clear();

            /*
            editor.SetSearchFlags(FindOption.NONE);
            editor.SetTargetStart(Math.Max(editor.GetCurrentPos(), editor.GetAnchor()));
            editor.SetTargetEnd(editor.GetTextLength());

            int pos = editor.SearchInTarget(searchText.Length, searchText);
            if (pos >= 0)
            {
                editor.SetSel(editor.GetTargetStart(), editor.GetTargetEnd());
            }
            */

            string myString, mySegment;
            int myInt = 0;

            myString = editor.GetCurLine(myInt);

            if (myString != "")
            {

                mySegment = myString.Substring(0, 3);

                //Remove line terminator
                myString = myString.Replace("\r", "").Replace("\n", "");
                if (myString.Substring(myString.Length - 1, 1) == "'")
                {
                    myString = myString.Substring(0, myString.Length - 1);
                }

                var myStringArray = myString.Split('+').Select(x => x.Split(':')).ToArray();

                //List<SegmentStructure> SegmentList = new List<SegmentStructure>();

                //SegmentStructure[] Segments = new SegmentStructure[1]

                /*
                SegmentList.Add(new SegmentStructure("file",1,"G0",0, mySegment,"","val"));
                ElementStructure myElement = new ElementStructure("TAG", "Descri");
                SegmentList[0].ElementsList.Add(myElement);
                */

                //MessageBox.Show(mySegment);



                //Add Segment
                SegmentList.Add(new SegmentStructure("file", 1, "G0", 0, mySegment, ""));



                //try
                //{
                string curAssemblyFolder = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                curAssemblyFolder = curAssemblyFolder.Replace(".dll", ".db");

                //SQLiteConnection db = new SQLiteConnection(@"Data Source=C:\code\NotepadPlusPlusPluginPack\EDIFACT.db;Pooling=true;FailIfMissing=false;Version=3");

                SQLiteConnection db = new SQLiteConnection(@"Data Source=" + curAssemblyFolder + ";Pooling=true;FailIfMissing=false;Version=3");
                db.Open();

                var selectElementsCommand = new SQLiteCommand("select * from VW_Segment_Elements where Segment_TAG = @SegmentTag order by Element_Position", db);
                selectElementsCommand.Parameters.AddWithValue("@SegmentTag", mySegment);

                SQLiteDataReader elementsQuery = selectElementsCommand.ExecuteReader();

                int iNumElement = 0;
                int iNumComponent = 0;
                string elementVal;
                string elementRef;
                string elementTag;

                string componentTag;
                string componentDescri;
                string componentValue;
                string componentDataType;
                string componentRefTable;
                int componentMinLength;
                int componentMaxLengrh;


                while (elementsQuery.Read())
                {

                    //Console.WriteLine(query.GetInt32(0) + "  " + query.GetString(1));
                    //MessageBox.Show(elementsQuery.GetString(0) + "  " + elementsQuery.GetString(1) + "  " + elementsQuery.GetString(2));

                    try
                    {
                        elementRef = elementsQuery.GetString(9);
                    }
                    catch
                    {
                        elementRef = "";
                    }

                    elementTag = elementsQuery.GetString(1);

                    //Add Element to the list
                    ElementStructure myElement = new ElementStructure(elementTag, elementsQuery.GetString(6), null, elementRef);
                    SegmentList[0].ElementsList.Add(myElement); // change the [0] in the when the full process will developed!

                    //COPONENTS LOOK
                    var selectComponentsCommand = new SQLiteCommand("select * from VW_Elements_Components where element_tag = @ElementTag and component_name like @SegmentTag order by Component_Position", db);
                    selectComponentsCommand.Parameters.AddWithValue("@ElementTag", elementTag);
                    selectComponentsCommand.Parameters.AddWithValue("@SegmentTag", mySegment + "%");
                    SQLiteDataReader componentsQuery = selectComponentsCommand.ExecuteReader();

                    iNumComponent = 0;
                    while (componentsQuery.Read())
                    {
                        // Get component attributes from database
                        componentTag = componentsQuery.GetString(1);
                        try { componentDescri = componentsQuery.GetString(6); } catch { componentDescri = ""; }
                        try { componentDataType = componentsQuery.GetString(9); } catch { componentDataType = ""; }
                        try { componentRefTable = componentsQuery.GetString(10); } catch { componentRefTable = ""; }
                        try { componentMinLength = componentsQuery.GetInt32(7); } catch { componentMinLength = 0; }
                        try { componentMaxLengrh = componentsQuery.GetInt32(8); } catch { componentMaxLengrh = 0; }

                        // Get component value from file string/segment
                        try { componentValue = myStringArray[iNumElement + 1][iNumComponent]; } catch { componentValue = ""; }

                        //Add Component to the list
                        ComponentStructure myComponent = new ComponentStructure(componentTag, componentDescri, componentMinLength, componentMaxLengrh, componentDataType, componentRefTable, componentValue);
                        SegmentList[0].ElementsList[iNumElement].ComponentList.Add(myComponent);

                        iNumComponent = iNumComponent + 1;
                    }

                    //if there is no components in the hierarcy the value is assigned to the element
                    if (iNumComponent == 0)
                    {
                        try
                        {
                            SegmentList[0].ElementsList[iNumElement].sValue = myStringArray[iNumElement + 1][0];
                        }
                        catch (Exception Myerr)
                        {
                            System.Diagnostics.Debug.WriteLine(Myerr.Message);
                        }
                    }


                    iNumElement = iNumElement + 1;


                }



                db.Close();


                //}
                /*
                catch (Exception Myerr)
                {
                    // Something unexpected went wrong.
                    System.Diagnostics.Debug.WriteLine(Myerr.Message);
                    // Maybe it is also necessary to terminate / restart the application.
                }
                */


            }
        }


    }
}
