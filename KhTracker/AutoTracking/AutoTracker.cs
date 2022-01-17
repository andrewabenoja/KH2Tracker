using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace KhTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MemoryReader memory, testMemory;

        private Int32 ADDRESS_OFFSET;
        private static DispatcherTimer aTimer, autoTimer, pcsx2OffsetTimer;
        private List<ImportantCheck> importantChecks;
        private Ability highJump;
        private Ability quickRun;
        private Ability dodgeRoll;
        private Ability aerialDodge;
        private Ability glide;

        private Ability secondChance;
        private Ability onceMore;

        private DriveForm valor;
        private DriveForm wisdom;
        private DriveForm master;
        private DriveForm limit;
        private DriveForm final;
        
        private Magic fire;
        private Magic blizzard;
        private Magic thunder;
        private Magic magnet;
        private Magic reflect;
        private Magic cure;

        private Report rep1;
        private Report rep2;
        private Report rep3;
        private Report rep4;
        private Report rep5;
        private Report rep6;
        private Report rep7;
        private Report rep8;
        private Report rep9;
        private Report rep10;
        private Report rep11;
        private Report rep12;
        private Report rep13;

        private Summon chickenLittle;
        private Summon stitch;
        private Summon genie;
        private Summon peterPan;

        private ImportantCheck promiseCharm;
        private ImportantCheck peace;
        private ImportantCheck nonexist;
        private ImportantCheck connection;

        private TornPage pages;

        private World world;
        private Stats stats;
        private Rewards rewards;
        private List<ImportantCheck> collectedChecks;
        private List<ImportantCheck> newChecks;
        private List<ImportantCheck> previousChecks;

        private int fireLevel;
        private int blizzardLevel;
        private int thunderLevel;
        private int cureLevel;
        private int reflectLevel;
        private int magnetLevel;
        private int tornPageCount;

        //extra controls
        private int storedDetectedVersion = 0; //0 = nothing detected, 1 = PC, 2 = PCSX2
        private bool isWorking = false;
        private bool firstRun = true;

        private bool usedHotkey = false;

        public int levelCheckOption;
        
        private bool forcedFinal;
        private CheckEveryCheck checkEveryCheck;

        private int[] lastEvent = { 0, 0, 0 };

        //                                 Sora   Drive   STT    TT     HB     BC     OC     AG     LoD   100AW   PL     DC     HT     PR     SP   TWTNW    GoA    AT
        //                                   0      1      2      3      4      5      6      7      8      9     10     11     12     13     14     15     16     17
        public int[] localHintMemory =   {  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1   };
        public int[] localReportMemory = {  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1   };
        public bool[] tornPageMemory =   { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        public bool[] driveFormMemory =  { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        //Scaled Points Stuff
        private int[] requiredPoints = { 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8 };
        //Simulated Twilight Town
        private int stt_TwilightThorn = 2;
        private int stt_Struggle = 2;
        private int stt_Axel = 3;
        private int stt_DataRoxas = 4;
        //Twilight Town
        private int tt_Tower = 2;
        private int tt_Sandlot = 2;
        private int tt_Mansion = 3;
        private int tt_Betwixt = 3;
        private int tt_DataAxel = 4;
        //Hallow Bastion
        private int hb_Bailey = 2;
        private int hb_Ansem = 1;
        private int hb_Corridors = 2;
        private int hb_Dancers = 2;
        private int hb_Demyx = 3;
        private int hb_FFFights = 3;
        private int hb_1k = 4;
        private int hb_Sephiroth = 4;
        private int hb_DataDemyx = 5;
        //Beasts Castle
        private int bc_Thresholder = 2;
        private int bc_Beast = 1;
        private int bc_DarkThorn = 2;
        private int bc_Dragoons = 2;
        private int bc_Xaldin = 4;
        private int bc_DataXaldin = 5;
        //Olympus Coliseum
        private int oc_Cerberus = 3;
        private int oc_Demyx = 1;
        private int oc_Pete = 1;
        private int oc_Hydra = 3;
        private int oc_Auron = 2;
        private int oc_Hades = 3;
        private int oc_Zexion = 4;
        //Agrabah
        private int ag_Abu = 1;
        private int ag_Chasm = 2;
        private int ag_Treasure = 2;
        private int ag_Twins = 3;
        private int ag_Carpet = 3;
        private int ag_Jafar = 4;
        private int ag_Lexaeus = 4;
        //Land of Dragons
        private int lod_Cave = 3;
        private int lod_Summit = 1;
        private int lod_ShanYu = 2;
        private int lod_Throne = 3;
        private int lod_StormRider = 4;
        private int lod_DataXigbar = 5;
        //100 AW
        private int ha_Piglet = 1;
        private int ha_Rabbit = 1;
        private int ha_Kanga = 2;
        private int ha_SpookyCave = 3;
        private int ha_StarryHill = 4;
        //Pride Lands
        private int pl_Simba = 1;
        private int pl_Scar = 3;
        private int pl_Groundshaker = 4;
        private int pl_DataSaix = 4;
        //Atlantica
        private int at_Tutorial = 1;
        private int at_Ursula = 3;
        private int at_NewDay = 4;
        //Disney Castle
        private int dc_Minnie = 1;
        private int dc_OldPete = 1;
        private int dc_Windows = 3;
        private int dc_Steamboat = 1;
        private int dc_NewPete = 3;
        private int dc_Marluxia = 4;
        private int dc_Terra = 5;
        //Halloween Town
        private int ht_CandyCaneLane = 1;
        private int ht_PrisonKeeper = 3;
        private int ht_Oogie = 2;
        private int ht_Presents = 1;
        private int ht_Experiment = 4;
        private int ht_Vexen = 4;
        //Port Royal
        private int pr_Town = 2;
        private int pr_Barbossa = 3;
        private int pr_Gambler = 2;
        private int pr_Grim2 = 4;
        private int pr_DataLuxord = 5;
        //Space Paranoids
        private int sp_Screens = 3;
        private int sp_HostileProgram = 3;
        private int sp_SolarSailer = 3;
        private int sp_MCP = 4;
        private int sp_Larxene = 4;
        //The World That Never Was
        private int twtnw_Roxas = 2;
        private int twtnw_Xigbar = 3;
        private int twtnw_Luxord = 2;
        private int twtnw_Saix = 2;
        private int twtnw_Xemnas = 3;
        private int twtnw_DataXemnas = 5;
        //Level Points Stuff
        private int sora_PreviousLevel = 1;
        private int sora_NextPPLevel = 10;
        //private bool sora_Update = false;
        private int sora_Level10 = 1;
        private int sora_Level20 = 1;
        private int sora_Level30 = 2;
        private int sora_Level40 = 2;
        private int sora_Level50 = 3;
        //Drive Points Stuff
            //valor
        private int valor_PreviousLevel = 1;
        private int valor_Level5 = 1;
        private int valor_Level7 = 3;
        private int valor_NextLevel = 5;
            //wisdom
        private int wisdom_PreviousLevel = 1;
        private int wisdom_Level5 = 1;
        private int wisdom_Level7 = 3;
        private int wisdom_NextLevel = 5;
            //limit
        private int limit_PreviousLevel = 1;
        private int limit_Level5 = 1;
        private int limit_Level7 = 3;
        private int limit_NextLevel = 5;
            //master
        private int master_PreviousLevel = 1;
        private int master_Level5 = 1;
        private int master_Level7 = 3;
        private int master_NextLevel = 5;
            //final
        private int final_PreviousLevel = 1;
        private int final_Level5 = 1;
        private int final_Level7 = 3;
        private int final_NextLevel = 5;

        public void InitPCSX2Tracker(object sender, RoutedEventArgs e)
        {
            InitAutoTracker(true);
        }

        public void InitPCTracker(object sender, RoutedEventArgs e)
        {
            InitAutoTracker(false);
        }

        public void StartPCSX2Hotkey()
        {
            Console.WriteLine("Hotkey pressed PCSX2");

            //if (!HotkeyOption.IsChecked)
                //return;

            if (!usedHotkey)
            {
                usedHotkey = true;
                InitAutoTrackerOriginal(true);
            }
        }
        public void StartPCHotkey()
        {
            Console.WriteLine("Hotkey pressed PC");

            //if (!HotkeyOption.IsChecked)
                //return;

            if (!usedHotkey)
            {
                usedHotkey = true;
                InitAutoTracker(false);
            }
        }
        public void ResetHotkeyState()
        {
            usedHotkey = false;
        }

        private void SetAutoDetectTimer()
        {
            if (isWorking)
                return;

            if (aTimer != null)
                aTimer.Stop();

            //autoTimer = new DispatcherTimer();
            if (firstRun)
            {
                //Console.WriteLine("Started search");
                autoTimer = new DispatcherTimer();
                autoTimer.Tick += searchVersion;
                firstRun = false;
                autoTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            }
            autoTimer.Start();

            //Console.WriteLine("AutoDetect Started");
        }

        private bool alternateCheck = false;
        private int alternateCheckInt = 1;
        public void searchVersion(object sender, EventArgs e)
        {
            if (!AutoDetectOption.IsChecked)
            {
                Console.WriteLine("disabling auto-detect");
                autoTimer.Stop();
                return;
            }

            if (isWorking || data.mode == Mode.None)
                return;

            Console.WriteLine("searchVersion called");

            if (CheckVersion(alternateCheck))
            {
                autoTimer.Stop();

                if (alternateCheck)
                {
                    Console.WriteLine("PCSX2 Found, starting Auto-Tracker");
                    HintText.Content = "PCSX2 Detected - Tracking";
                }
                else
                {
                    Console.WriteLine("PC Found, starting Auto-Tracker");
                    HintText.Content = "PC Detected - Connecting...";
                }

                if (storedDetectedVersion != alternateCheckInt && storedDetectedVersion != 0)
                {
                    //Console.WriteLine("storedDetectedVerison = " + storedDetectedVersion + " || alternateCheck = " + alternateCheck);
                    OnResetBody();
                }
                storedDetectedVersion = alternateCheckInt;

                InitAutoTracker(alternateCheck);

                isWorking = true;

                return;
            }

            alternateCheck = !alternateCheck;
            if (alternateCheckInt == 1)
                alternateCheckInt = 2;
            else
                alternateCheckInt = 1;
        }

        public void SetWorking(bool state)
        {
            isWorking = state;
        }

        public bool CheckVersion(bool state)
        {
            if (isWorking)
                return true;

            int tries = 0;
            do
            {
                testMemory = new MemoryReader(state);
                if (tries < 20)
                {
                    tries++;
                }
                else
                {
                    testMemory = null;
                    Console.WriteLine("No game running");
                    return false;
                }
            } while (!testMemory.Hooked);

            return true;
        }

        public void InitAutoTracker(bool PCSX2)
        {
            autoTimer.Stop();
            isWorking = true;

            int tries = 0;
            do
            {
                memory = new MemoryReader(PCSX2);
                if (tries < 20)
                {
                    tries++;
                }
                else
                {
                    memory = null;
                    MessageBox.Show("Please start KH2 before loading the Auto Tracker.");
                    //ResetHotkeyState();
                    return;
                }
            } while (!memory.Hooked);

            // PC Address anchors
            int Now = 0x0714DB8;
            int Save = 0x09A70B0;
            int Sys3 = 0x2A59DF0;
            int Bt10 = 0x2A74880;
            int BtlEnd = 0x2A0D3E0;
            int Slot1 = 0x2A20C98;

            if (PCSX2 == false)
            {
                finishSetupHelper(PCSX2, Now, Save, Sys3, Bt10, BtlEnd, Slot1);
            }
            else
            {
                checkForPCSX2();
            }
        }

        public void InitAutoTrackerOriginal(bool PCSX2)
        {
            autoTimer.Stop();
            isWorking = true;

            int tries = 0;
            do
            {
                memory = new MemoryReader(PCSX2);
                if (tries < 20)
                {
                    tries++;
                }
                else
                {
                    memory = null;
                    MessageBox.Show("Please start KH2 before loading the Auto Tracker.");
                    //ResetHotkeyState();
                    return;
                }
            } while (!memory.Hooked);

            // PC Address anchors
            int Now = 0x0714DB8;
            int Save = 0x09A70B0;
            int Sys3 = 0x2A59DF0;
            int Bt10 = 0x2A74880;
            int BtlEnd = 0x2A0D3E0;
            int Slot1 = 0x2A20C98;

            if (PCSX2 == false)
            {
                try
                {
                    CheckPCOffset();
                }
                catch (Win32Exception)
                {
                    memory = null;
                    MessageBox.Show("Unable to access KH2FM try running KHTracker as admin");
                    //ResetHotkeyState();
                    isWorking = false;
                    SetAutoDetectTimer();
                    return;
                }
                catch
                {
                    memory = null;
                    MessageBox.Show("Error connecting to KH2FM");
                    //ResetHotkeyState();
                    isWorking = false;
                    SetAutoDetectTimer();
                    return;
                }
            }
            else
            {
                try
                {
                    findAddressOffset();
                }
                catch (Win32Exception)
                {
                    memory = null;
                    MessageBox.Show("Unable to access PCSX2 try running KHTracker as admin");
                    isWorking = false;
                    SetAutoDetectTimer();
                    return;
                }
                catch
                {
                    memory = null;
                    MessageBox.Show("Error connecting to PCSX2");
                    isWorking = false;
                    SetAutoDetectTimer();
                    return;
                }

                // PCSX2 anchors 
                Now = 0x032BAE0;
                Save = 0x032BB30;
                Sys3 = 0x1CCB300;
                Bt10 = 0x1CE5D80;
                BtlEnd = 0x1D490C0;
                Slot1 = 0x1C6C750;
            }

            
            importantChecks = new List<ImportantCheck>();
            importantChecks.Add(highJump = new Ability(memory, Save + 0x25CE, ADDRESS_OFFSET, 93, "HighJump"));
            importantChecks.Add(quickRun = new Ability(memory, Save + 0x25D0, ADDRESS_OFFSET, 97, "QuickRun"));
            importantChecks.Add(dodgeRoll = new Ability(memory, Save + 0x25D2, ADDRESS_OFFSET, 563, "DodgeRoll"));
            importantChecks.Add(aerialDodge = new Ability(memory, Save + 0x25D4, ADDRESS_OFFSET, 101, "AerialDodge"));
            importantChecks.Add(glide = new Ability(memory, Save + 0x25D6, ADDRESS_OFFSET, 105, "Glide"));

            importantChecks.Add(secondChance = new Ability(memory, Save + 0x2544, ADDRESS_OFFSET, "SecondChance", Save));
            importantChecks.Add(onceMore = new Ability(memory, Save + 0x2544, ADDRESS_OFFSET, "OnceMore", Save));
            
            importantChecks.Add(valor = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 1, Save + 0x32F6, Save + 0x06B2, "Valor"));
            importantChecks.Add(wisdom = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 2, Save + 0x332E, "Wisdom"));
            importantChecks.Add(limit = new DriveForm(memory, Save + 0x36CA, ADDRESS_OFFSET, 3, Save + 0x3366, "Limit"));
            importantChecks.Add(master = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 6, Save + 0x339E, "Master"));
            importantChecks.Add(final = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 4, Save + 0x33D6, "Final"));

            int fireCount = fire != null ? fire.Level : 0;
            int blizzardCount = blizzard != null ? blizzard.Level : 0;
            int thunderCount = thunder != null ? thunder.Level : 0;
            int cureCount = cure != null ? cure.Level : 0;
            int magnetCount = magnet != null ? magnet.Level : 0;
            int reflectCount = reflect != null ? reflect.Level : 0;

            importantChecks.Add(fire = new Magic(memory, Save + 0x3594, Save + 0x1CF2, ADDRESS_OFFSET, "Fire"));
            importantChecks.Add(blizzard = new Magic(memory, Save + 0x3595, Save + 0x1CF3, ADDRESS_OFFSET, "Blizzard"));
            importantChecks.Add(thunder = new Magic(memory, Save + 0x3596, Save + 0x1CF4, ADDRESS_OFFSET, "Thunder"));
            importantChecks.Add(cure = new Magic(memory, Save + 0x3597, Save + 0x1CF5, ADDRESS_OFFSET, "Cure"));
            importantChecks.Add(magnet = new Magic(memory, Save + 0x35CF, Save + 0x1CF6, ADDRESS_OFFSET, "Magnet"));
            importantChecks.Add(reflect = new Magic(memory, Save + 0x35D0, Save + 0x1CF7, ADDRESS_OFFSET, "Reflect"));

            fire.Level = fireCount;
            blizzard.Level = blizzardCount;
            thunder.Level = thunderCount;
            cure.Level = cureCount;
            magnet.Level = magnetCount;
            reflect.Level = reflectCount;

            importantChecks.Add(rep1 = new Report(memory, Save + 0x36C4, ADDRESS_OFFSET, 6, "Report1"));
            importantChecks.Add(rep2 = new Report(memory, Save + 0x36C4, ADDRESS_OFFSET, 7, "Report2"));
            importantChecks.Add(rep3 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 0, "Report3"));
            importantChecks.Add(rep4 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 1, "Report4"));
            importantChecks.Add(rep5 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 2, "Report5"));
            importantChecks.Add(rep6 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 3, "Report6"));
            importantChecks.Add(rep7 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 4, "Report7"));
            importantChecks.Add(rep8 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 5, "Report8"));
            importantChecks.Add(rep9 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 6, "Report9"));
            importantChecks.Add(rep10 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 7, "Report10"));
            importantChecks.Add(rep11 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 0, "Report11"));
            importantChecks.Add(rep12 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 1, "Report12"));
            importantChecks.Add(rep13 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 2, "Report13"));

            importantChecks.Add(chickenLittle = new Summon(memory, Save + 0x36C0, ADDRESS_OFFSET, 3, "Baseball"));
            importantChecks.Add(stitch = new Summon(memory, Save + 0x36C0, ADDRESS_OFFSET, 0, "Ukulele"));
            importantChecks.Add(genie = new Summon(memory, Save + 0x36C4, ADDRESS_OFFSET, 4, "Lamp"));
            importantChecks.Add(peterPan = new Summon(memory, Save + 0x36C4, ADDRESS_OFFSET, 5, "Feather"));

            importantChecks.Add(promiseCharm = new Proof(memory, Save + 0x3694, ADDRESS_OFFSET, "PromiseCharm"));
            importantChecks.Add(peace = new Proof(memory, Save + 0x36B4, ADDRESS_OFFSET, "Peace"));
            importantChecks.Add(nonexist = new Proof(memory, Save + 0x36B3, ADDRESS_OFFSET, "Nonexistence"));
            importantChecks.Add(connection = new Proof(memory, Save + 0x36B2, ADDRESS_OFFSET, "Connection"));

            int count = pages != null ? pages.Quantity : 0;
            importantChecks.Add(pages = new TornPage(memory, Save + 0x3598, ADDRESS_OFFSET, "TornPage"));
            pages.Quantity = count;

            if (PCSX2)
                world = new World(memory, ADDRESS_OFFSET, Now, 0x00351EC8, Save + 0x1CFF);
            else
                world = new World(memory, ADDRESS_OFFSET, Now, BtlEnd + 0x820, Save + 0x1CFF);

            stats = new Stats(memory, ADDRESS_OFFSET, Save + 0x24FE, Slot1 + 0x188, Save + 0x3524);
            rewards = new Rewards(memory, ADDRESS_OFFSET, Bt10);

            forcedFinal = false;
            checkEveryCheck = new CheckEveryCheck(memory, ADDRESS_OFFSET, Save, Sys3, Bt10, world, stats, rewards);

            LevelIcon.Visibility = Visibility.Visible;
            Level.Visibility = Visibility.Visible;
            StrengthIcon.Visibility = Visibility.Visible;
            Strength.Visibility = Visibility.Visible;
            MagicIcon.Visibility = Visibility.Visible;
            Magic.Visibility = Visibility.Visible;
            DefenseIcon.Visibility = Visibility.Visible;
            Defense.Visibility = Visibility.Visible;
            Weapon.Visibility = Visibility.Visible;

            //LevelRewardIcon.Visibility = Visibility.Visible;
            //LevelReward.Visibility = Visibility.Visible;

            broadcast.LevelIcon.Visibility = Visibility.Visible;
            broadcast.Level.Visibility = Visibility.Visible;
            broadcast.StrengthIcon.Visibility = Visibility.Visible;
            broadcast.Strength.Visibility = Visibility.Visible;
            broadcast.MagicIcon.Visibility = Visibility.Visible;
            broadcast.Magic.Visibility = Visibility.Visible;
            broadcast.DefenseIcon.Visibility = Visibility.Visible;
            broadcast.Defense.Visibility = Visibility.Visible;
            broadcast.Weapon.Visibility = Visibility.Visible;

            broadcast.WorldRow.Height = new GridLength(6, GridUnitType.Star);
            broadcast.GrowthAbilityRow.Height = new GridLength(1, GridUnitType.Star);
            //FormRow.Height = new GridLength(0.65, GridUnitType.Star);

            SetBindings();
            SetTimer();
            OnTimedEvent(null, null);
        }

        private async void finishSetupHelper(bool PCSX2, Int32 Now, Int32 Save, Int32 Sys3, Int32 Bt10, Int32 BtlEnd, Int32 Slot1)
        {
            //Console.WriteLine("calling finishSetupHelper");
            await Task.Delay(9000);
            finishSetup(PCSX2, Now, Save, Sys3, Bt10, BtlEnd, Slot1);
            //Console.WriteLine("delayed writeline finishSetupHelper");
        }

        private void finishSetup(bool PCSX2, Int32 Now, Int32 Save, Int32 Sys3, Int32 Bt10, Int32 BtlEnd, Int32 Slot1)
        {
            try
            {
                CheckPCOffset();
            }
            catch (Win32Exception)
            {
                memory = null;
                MessageBox.Show("Unable to access KH2FM try running KHTracker as admin");
                //ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }
            catch
            {
                memory = null;
                MessageBox.Show("Error connecting to KH2FM");
                //ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }

            if (!PCSX2)
                HintText.Content = "PC Detected - Tracking";
            else
                HintText.Content = "PCSX2 Detected - Tracking";
            SetHintTextDelayed("");

            importantChecks = new List<ImportantCheck>();
            importantChecks.Add(highJump = new Ability(memory, Save + 0x25CE, ADDRESS_OFFSET, 93, "HighJump"));
            importantChecks.Add(quickRun = new Ability(memory, Save + 0x25D0, ADDRESS_OFFSET, 97, "QuickRun"));
            importantChecks.Add(dodgeRoll = new Ability(memory, Save + 0x25D2, ADDRESS_OFFSET, 563, "DodgeRoll"));
            importantChecks.Add(aerialDodge = new Ability(memory, Save + 0x25D4, ADDRESS_OFFSET, 101, "AerialDodge"));
            importantChecks.Add(glide = new Ability(memory, Save + 0x25D6, ADDRESS_OFFSET, 105, "Glide"));

            importantChecks.Add(secondChance = new Ability(memory, Save + 0x2544, ADDRESS_OFFSET, "SecondChance", Save));
            importantChecks.Add(onceMore = new Ability(memory, Save + 0x2544, ADDRESS_OFFSET, "OnceMore", Save));

            importantChecks.Add(valor = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 1, Save + 0x32F6, Save + 0x06B2, "Valor"));
            importantChecks.Add(wisdom = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 2, Save + 0x332E, "Wisdom"));
            importantChecks.Add(limit = new DriveForm(memory, Save + 0x36CA, ADDRESS_OFFSET, 3, Save + 0x3366, "Limit"));
            importantChecks.Add(master = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 6, Save + 0x339E, "Master"));
            importantChecks.Add(final = new DriveForm(memory, Save + 0x36C0, ADDRESS_OFFSET, 4, Save + 0x33D6, "Final"));

            int fireCount = fire != null ? fire.Level : 0;
            int blizzardCount = blizzard != null ? blizzard.Level : 0;
            int thunderCount = thunder != null ? thunder.Level : 0;
            int cureCount = cure != null ? cure.Level : 0;
            int magnetCount = magnet != null ? magnet.Level : 0;
            int reflectCount = reflect != null ? reflect.Level : 0;

            importantChecks.Add(fire = new Magic(memory, Save + 0x3594, Save + 0x1CF2, ADDRESS_OFFSET, "Fire"));
            importantChecks.Add(blizzard = new Magic(memory, Save + 0x3595, Save + 0x1CF3, ADDRESS_OFFSET, "Blizzard"));
            importantChecks.Add(thunder = new Magic(memory, Save + 0x3596, Save + 0x1CF4, ADDRESS_OFFSET, "Thunder"));
            importantChecks.Add(cure = new Magic(memory, Save + 0x3597, Save + 0x1CF5, ADDRESS_OFFSET, "Cure"));
            importantChecks.Add(magnet = new Magic(memory, Save + 0x35CF, Save + 0x1CF6, ADDRESS_OFFSET, "Magnet"));
            importantChecks.Add(reflect = new Magic(memory, Save + 0x35D0, Save + 0x1CF7, ADDRESS_OFFSET, "Reflect"));

            fire.Level = fireCount;
            blizzard.Level = blizzardCount;
            thunder.Level = thunderCount;
            cure.Level = cureCount;
            magnet.Level = magnetCount;
            reflect.Level = reflectCount;

            importantChecks.Add(rep1 = new Report(memory, Save + 0x36C4, ADDRESS_OFFSET, 6, "Report1"));
            importantChecks.Add(rep2 = new Report(memory, Save + 0x36C4, ADDRESS_OFFSET, 7, "Report2"));
            importantChecks.Add(rep3 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 0, "Report3"));
            importantChecks.Add(rep4 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 1, "Report4"));
            importantChecks.Add(rep5 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 2, "Report5"));
            importantChecks.Add(rep6 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 3, "Report6"));
            importantChecks.Add(rep7 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 4, "Report7"));
            importantChecks.Add(rep8 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 5, "Report8"));
            importantChecks.Add(rep9 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 6, "Report9"));
            importantChecks.Add(rep10 = new Report(memory, Save + 0x36C5, ADDRESS_OFFSET, 7, "Report10"));
            importantChecks.Add(rep11 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 0, "Report11"));
            importantChecks.Add(rep12 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 1, "Report12"));
            importantChecks.Add(rep13 = new Report(memory, Save + 0x36C6, ADDRESS_OFFSET, 2, "Report13"));

            importantChecks.Add(chickenLittle = new Summon(memory, Save + 0x36C0, ADDRESS_OFFSET, 3, "Baseball"));
            importantChecks.Add(stitch = new Summon(memory, Save + 0x36C0, ADDRESS_OFFSET, 0, "Ukulele"));
            importantChecks.Add(genie = new Summon(memory, Save + 0x36C4, ADDRESS_OFFSET, 4, "Lamp"));
            importantChecks.Add(peterPan = new Summon(memory, Save + 0x36C4, ADDRESS_OFFSET, 5, "Feather"));

            importantChecks.Add(promiseCharm = new Proof(memory, Save + 0x3694, ADDRESS_OFFSET, "PromiseCharm"));
            importantChecks.Add(peace = new Proof(memory, Save + 0x36B4, ADDRESS_OFFSET, "Peace"));
            importantChecks.Add(nonexist = new Proof(memory, Save + 0x36B3, ADDRESS_OFFSET, "Nonexistence"));
            importantChecks.Add(connection = new Proof(memory, Save + 0x36B2, ADDRESS_OFFSET, "Connection"));

            int count = pages != null ? pages.Quantity : 0;
            importantChecks.Add(pages = new TornPage(memory, Save + 0x3598, ADDRESS_OFFSET, "TornPage"));
            pages.Quantity = count;

            if (PCSX2)
                world = new World(memory, ADDRESS_OFFSET, Now, 0x00351EC8, Save + 0x1CFF);
            else
                world = new World(memory, ADDRESS_OFFSET, Now, BtlEnd + 0x820, Save + 0x1CFF);

            stats = new Stats(memory, ADDRESS_OFFSET, Save + 0x24FE, Slot1 + 0x188, Save + 0x3524);
            rewards = new Rewards(memory, ADDRESS_OFFSET, Bt10);

            forcedFinal = false;
            checkEveryCheck = new CheckEveryCheck(memory, ADDRESS_OFFSET, Save, Sys3, Bt10, world, stats, rewards);

            LevelIcon.Visibility = Visibility.Visible;
            Level.Visibility = Visibility.Visible;
            StrengthIcon.Visibility = Visibility.Visible;
            Strength.Visibility = Visibility.Visible;
            MagicIcon.Visibility = Visibility.Visible;
            Magic.Visibility = Visibility.Visible;
            DefenseIcon.Visibility = Visibility.Visible;
            Defense.Visibility = Visibility.Visible;
            Weapon.Visibility = Visibility.Visible;

            broadcast.LevelIcon.Visibility = Visibility.Visible;
            broadcast.Level.Visibility = Visibility.Visible;
            broadcast.StrengthIcon.Visibility = Visibility.Visible;
            broadcast.Strength.Visibility = Visibility.Visible;
            broadcast.MagicIcon.Visibility = Visibility.Visible;
            broadcast.Magic.Visibility = Visibility.Visible;
            broadcast.DefenseIcon.Visibility = Visibility.Visible;
            broadcast.Defense.Visibility = Visibility.Visible;
            broadcast.Weapon.Visibility = Visibility.Visible;

            broadcast.WorldRow.Height = new GridLength(6, GridUnitType.Star);
            broadcast.GrowthAbilityRow.Height = new GridLength(1, GridUnitType.Star);
            //FormRow.Height = new GridLength(0.65, GridUnitType.Star);

            SetBindings();
            SetTimer();
            OnTimedEvent(null, null);
            if (!data.startedProgression)
            {
                stats.SetProgressPoints(0);
                stats.SetHintIndex(0);
                data.startedProgression = true;
                SetRequiredPoints(requiredPoints[stats.hintIndex]);
            }
            else
            {
                stats.SetProgressPoints(data.storedProgressPoints);
                stats.SetHintIndex(data.storedProgressIndex);
            }
        }

        private void finishSetupPCSX2()
        {
            try
            {
                findAddressOffset();
            }
            catch (Win32Exception)
            {
                memory = null;
                MessageBox.Show("Unable to access PCSX2 try running KHTracker as admin");
                //ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }
            catch
            {
                memory = null;
                MessageBox.Show("Error connecting to PCSX2");
                //ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }

            SetHintTextDelayed("");

            // PCSX2 anchors 
            int Now = 0x032BAE0;
            int Save = 0x032BB30;
            int Sys3 = 0x1CCB300;
            int Bt10 = 0x1CE5D80;
            int BtlEnd = 0x1D490C0;
            int Slot1 = 0x1C6C750;

            finishSetup(true, Now, Save, Sys3, Bt10, BtlEnd, Slot1);
        }

        private void CheckPCOffset()
        {
            Int32 testAddr = 0x009AA376 - 0x1000;
            string good = "F680";
            string tester = BytesToHex(memory.ReadMemory(testAddr, 2));
            if (tester == good)
            {
                ADDRESS_OFFSET = -0x1000;
            }
        }

        private void checkForPCSX2()
        {
            pcsx2OffsetTimer = new DispatcherTimer();
            pcsx2OffsetTimer.Tick += findAddressOffsetCheck;
            pcsx2OffsetTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            pcsx2OffsetTimer.Start();
            //Console.WriteLine("Am I here?");
        }

        private void findAddressOffsetCheck(object sender, EventArgs e)
        {
            Console.WriteLine("checking for PCSX2 - findAddressOffsetCheck");

            bool found = false;
            Int32 offset = 0x00000000;
            Int32 testAddr = 0x0032EE36;
            string good = "F680";
            int i = 0;
            while (!found)
            {
                i++;
                string tester = BytesToHex(memory.ReadMemory(testAddr + offset, 2));
                if (tester == "Service not started. Waiting for PCSX2")
                {
                    //Console.WriteLine("am I here then?");
                    break;
                }
                else if (tester == good)
                {
                    Console.WriteLine("Found it Pogchamp");

                    found = true;
                    pcsx2OffsetTimer.Stop();
                    pcsx2OffsetTimer.Tick -= findAddressOffsetCheck;
                    finishSetupPCSX2();
                    return;
                }
                else if (i > 15)
                {
                    Console.WriteLine("loop finished, no address found");
                    return;
                }
                else
                {
                    offset = offset + 0x10000000;
                }
            }
            ADDRESS_OFFSET = offset;
        }

        private void findAddressOffset()
        {
            bool found = false;
            Int32 offset = 0x00000000;
            Int32 testAddr = 0x0032EE36;
            string good = "F680";
            while (!found)
            {
                string tester = BytesToHex(memory.ReadMemory(testAddr + offset, 2));
                if (tester == "Service not started. Waiting for PCSX2")
                {
                    break;
                }
                else if (tester == good)
                {
                    found = true;
                }
                else
                {
                    offset = offset + 0x10000000;
                }
            }
            ADDRESS_OFFSET = offset;
        }

        private void SetBindings()
        {
            BindStats(Level, "Level", stats);
            BindWeapon(Weapon, "Weapon", stats);
            BindStats(Strength, "Strength", stats);
            BindStats(Magic, "Magic", stats);
            BindStats(Defense, "Defense", stats);

            //Addition Binds
            //BindStats(LevelReward, "LevelReward", stats);
            BindStats(ProgressPoints, "ProgressPoints", stats);

            BindLevel(broadcast.ValorLevel, "Level", valor);
            BindLevel(broadcast.WisdomLevel, "Level", wisdom);
            BindLevel(broadcast.LimitLevel, "Level", limit);
            BindLevel(broadcast.MasterLevel, "Level", master);
            BindLevel(broadcast.FinalLevel, "Level", final);

            BindAbility(broadcast.HighJump, "Obtained", highJump);
            BindAbility(broadcast.QuickRun, "Obtained", quickRun);
            BindAbility(broadcast.DodgeRoll, "Obtained", dodgeRoll);
            BindAbility(broadcast.AerialDodge, "Obtained", aerialDodge);
            BindAbility(broadcast.Glide, "Obtained", glide);

            BindAbilityLevel(broadcast.HighJumpLevel, "Level", highJump, new GrowthAbilityConverter());
            BindAbilityLevel(broadcast.QuickRunLevel, "Level", quickRun, new GrowthAbilityConverter());
            BindAbilityLevel(broadcast.DodgeRollLevel, "Level", dodgeRoll, new GrowthAbilityConverter());
            BindAbilityLevel(broadcast.AerialDodgeLevel, "Level", aerialDodge, new GrowthAbilityConverter());
            BindAbilityLevel(broadcast.GlideLevel, "Level", glide, new GrowthAbilityConverter());

            //track in main window
            BindAbility(HighJump, "Obtained", highJump);
            BindAbility(QuickRun, "Obtained", quickRun);
            BindAbility(DodgeRoll, "Obtained", dodgeRoll);
            BindAbility(AerialDodge, "Obtained", aerialDodge);
            BindAbility(Glide, "Obtained", glide);

            BindAbilityLevel(HighJumpLevel, "Level", highJump, new GrowthAbilityConverter());
            BindAbilityLevel(QuickRunLevel, "Level", quickRun, new GrowthAbilityConverter());
            BindAbilityLevel(DodgeRollLevel, "Level", dodgeRoll, new GrowthAbilityConverter());
            BindAbilityLevel(AerialDodgeLevel, "Level", aerialDodge, new GrowthAbilityConverter());
            BindAbilityLevel(GlideLevel, "Level", glide, new GrowthAbilityConverter());

            BindLevel(ValorLevel, "Level", valor);
            BindLevel(WisdomLevel, "Level", wisdom);
            BindLevel(LimitLevel, "Level", limit);
            BindLevel(MasterLevel, "Level", master);
            BindLevel(FinalLevel, "Level", final);
            
            BindForm(ValorM, "Obtained", valor);
            BindForm(WisdomM, "Obtained", wisdom);
            BindForm(LimitM, "Obtained", limit);
            BindForm(MasterM, "Obtained", master);
            BindForm(FinalM, "Obtained", final);
        }

        private void SetTimer()
        {
            if (aTimer != null)
                aTimer.Stop();

            aTimer = new DispatcherTimer();
            aTimer.Tick += OnTimedEvent;
            aTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            aTimer.Start();
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            previousChecks.Clear();
            previousChecks.AddRange(newChecks);
            newChecks.Clear();

            try
            {
                stats.UpdateMemory();
                world.UpdateMemory();
                UpdateMagicAddresses();
                UpdateWorldProgress(world);

                //check level/drive changes here
                UpdateProgressPoints_H(ProgressionHintsSoraLevels());
                UpdateProgressPoints_H(ProgressionHintsDriveLevels());

                importantChecks.ForEach(delegate (ImportantCheck importantCheck)
                {
                    importantCheck.UpdateMemory();
                });
            }
            catch
            {
                aTimer.Stop();
                //MessageBox.Show("KH2FM has exited. Stopping Auto Tracker.");
                //ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();

                HintText.Content = "Connection Lost, Reconnecting...";
                data.storedProgressPoints = stats.ProgressPoints;
                data.storedProgressIndex = stats.hintIndex;
                return;
            }

            UpdateCollectedItems();
            DetermineItemLocations();
            //stats.SetNextLevelCheck(stats.Level);
        }

        private void TrackItem(string itemName, WorldGrid world)
        {
            foreach (ContentControl item in ItemPool.Children)
            {
                if (item.Name == itemName && item.IsVisible)
                {
                    if (world.Handle_Report(item as Item, this, data))
                    {
                        world.Add_Item(item as Item, this);
                        if (App.logger != null)
                            App.logger.Record(item.Name + " tracked");
                    }
                    break;
                }
            }
        }

        private void TrackQuantities()
        {
            while (fire.Level > fireLevel)
            {
                ++fireLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Fire" + fireLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (blizzard.Level > blizzardLevel)
            {
                ++blizzardLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Blizzard" + blizzardLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (thunder.Level > thunderLevel)
            {
                ++thunderLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Thunder" + thunderLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (cure.Level > cureLevel)
            {
                ++cureLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Cure" + cureLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (reflect.Level > reflectLevel)
            {
                ++reflectLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Reflect" + reflectLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (magnet.Level > magnetLevel)
            {
                ++magnetLevel;
                Magic magic = new Magic(null, 0, 0, 0, "Magnet" + magnetLevel.ToString());
                newChecks.Add(magic);
                collectedChecks.Add(magic);
            }
            while (pages.Quantity > tornPageCount)
            {
                ++tornPageCount;
                TornPage page = new TornPage(null, 0, 0, "TornPage" + tornPageCount.ToString());
                newChecks.Add(page);
                collectedChecks.Add(page);
            }
        }

        private void UpdateMagicAddresses()
        {
            if (world.worldName == "SimulatedTwilightTown"  // (and not in Data Roxas fight)
                && !(world.roomNumber == 21 && (world.eventID1 == 99 || world.eventID3 == 113 || world.eventID1 == 114)))
            {
                fire.UseSTTAddress(true);
                blizzard.UseSTTAddress(true);
                thunder.UseSTTAddress(true);
                cure.UseSTTAddress(true);
                reflect.UseSTTAddress(true);
                magnet.UseSTTAddress(true);
            }
            else
            {
                fire.UseSTTAddress(false);
                blizzard.UseSTTAddress(false);
                thunder.UseSTTAddress(false);
                cure.UseSTTAddress(false);
                reflect.UseSTTAddress(false);
                magnet.UseSTTAddress(false);
            }
        }

        private void UpdateCollectedItems()
        {
            foreach (ImportantCheck check in importantChecks)
            {
                // handle these separately due to the way they are stored in memory
                if (check.GetType() == typeof(Magic) || check.GetType() == typeof(TornPage))
                    continue;

                if (check.Obtained && collectedChecks.Contains(check) == false)
                {
                    // skip auto tracking final if it was forced and valor
                    if (check.Name == "Valor" && valor.genieFix == true)
                    {
                        valor.Obtained = false;
                    }
                    else if (check.Name == "Final")
                    {
                        // if forced Final, start tracking the Final Form check
                        if (!forcedFinal && stats.form == 5)
                        {
                            forcedFinal = true;
                            checkEveryCheck.TrackCheck(0x001D);
                        }
                        // if not forced Final, track Final Form check like normal
                        // else if Final was forced, check the tracked Final Form check
                        else if (!forcedFinal || checkEveryCheck.UpdateTargetMemory())
                        {
                            collectedChecks.Add(check);
                            newChecks.Add(check);
                        }
                    }
                    else
                    {
                        collectedChecks.Add(check);
                        newChecks.Add(check);
                    }
                }
            }
            TrackQuantities();
        }

        // Sometimes level rewards and levels dont update on the same tick
        // Previous tick checks are placed on the current tick with the info of both ticks
        // This way level checks don't get misplaced 
        private void DetermineItemLocations()
        {
            if (previousChecks.Count == 0)
                return;

            // Get rewards between previous level and current level
            List<string> levelRewards = rewards.GetLevelRewards(stats.Weapon)
                .Where(reward => reward.Item1 > stats.previousLevels[0] && reward.Item1 <= stats.Level)
                .Select(reward => reward.Item2).ToList();
            // Get drive rewards between previous level and current level
            List<string> driveRewards = rewards.valorChecks
                .Where(reward => reward.Item1 > valor.previousLevels[0] && reward.Item1 <= valor.Level)
                .Select(reward => reward.Item2).ToList();
            driveRewards.AddRange(rewards.wisdomChecks
                .Where(reward => reward.Item1 > wisdom.previousLevels[0] && reward.Item1 <= wisdom.Level)
                .Select(reward => reward.Item2));
            driveRewards.AddRange(rewards.limitChecks
                .Where(reward => reward.Item1 > limit.previousLevels[0] && reward.Item1 <= limit.Level)
                .Select(reward => reward.Item2));
            driveRewards.AddRange(rewards.masterChecks
                .Where(reward => reward.Item1 > master.previousLevels[0] && reward.Item1 <= master.Level)
                .Select(reward => reward.Item2));
            driveRewards.AddRange(rewards.finalChecks
                .Where(reward => reward.Item1 > final.previousLevels[0] && reward.Item1 <= final.Level)
                .Select(reward => reward.Item2));

            if (stats.Level > stats.previousLevels[0] && App.logger != null)
                App.logger.Record("Levels " + stats.previousLevels[0].ToString() + " to " + stats.Level.ToString());
            if (valor.Level > valor.previousLevels[0] && App.logger != null)
                App.logger.Record("Valor Levels " + valor.previousLevels[0].ToString() + " to " + valor.Level.ToString());
            if (wisdom.Level > wisdom.previousLevels[0] && App.logger != null)
                App.logger.Record("Wisdom Levels " + wisdom.previousLevels[0].ToString() + " to " + wisdom.Level.ToString());
            if (limit.Level > limit.previousLevels[0] && App.logger != null)
                App.logger.Record("Limit Levels " + limit.previousLevels[0].ToString() + " to " + limit.Level.ToString());
            if (master.Level > master.previousLevels[0] && App.logger != null)
                App.logger.Record("Master Levels " + master.previousLevels[0].ToString() + " to " + master.Level.ToString());
            if (final.Level > final.previousLevels[0] && App.logger != null)
                App.logger.Record("Final Levels " + final.previousLevels[0].ToString() + " to " + final.Level.ToString());
            foreach (string str in levelRewards)
            {
                if (App.logger != null)
                    App.logger.Record("Level reward " + str);
            }
            foreach (string str in driveRewards)
            {
                if (App.logger != null)
                    App.logger.Record("Drive reward " + str);
            }

            foreach (ImportantCheck check in previousChecks)
            {
                string count = "";
                // remove magic and torn page count for comparison with item codes and readd to track specific ui copies
                if (check.GetType() == typeof(Magic) || check.GetType() == typeof(TornPage))
                {
                    count = check.Name.Substring(check.Name.Length - 1);
                    check.Name = check.Name.Substring(0, check.Name.Length - 1);
                }

                if (levelRewards.Exists(x => x == check.Name))
                {
                    if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints) && false)
                    {
                        data.WorldsData["SorasHeart"].hinted = true;
                        data.WorldsData["SorasHeart"].hintedHint = true;

                        if (localHintMemory[WorldNameToIndex("SorasHeart")] != -1)
                            SetReportValue(data.WorldsData["SorasHeart"].hint, localHintMemory[WorldNameToIndex("SorasHeart")] + 1);

                        // loop through hinted world for reports to set their info as hinted hints
                        for (int i = 0; i < data.WorldsData["SorasHeart"].worldGrid.Children.Count; ++i)
                        {
                            Item gridItem = data.WorldsData["SorasHeart"].worldGrid.Children[i] as Item;
                            if (gridItem.Name.Contains("Report"))
                            {
                                int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                //Console.WriteLine("Found a report here!");
                            }
                        }

                        // check and determine what world has the report for the world that just contained a proof
                        if (localReportMemory[WorldNameToIndex("SorasHeart")] != -1 && localReportMemory[WorldNameToIndex("SorasHeart")] != 16)
                        {
                            string hintedWorld = IndexToWorldName(localReportMemory[WorldNameToIndex("SorasHeart")]);
                            // loop through world for reports to set their info as hinted hints
                            for (int i = 0; i < data.WorldsData[hintedWorld].worldGrid.Children.Count; ++i)
                            {
                                Item gridItem = data.WorldsData[hintedWorld].worldGrid.Children[i] as Item;
                                if (gridItem.Name.Contains("Report"))
                                {
                                    int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                    data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                    SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                    //Console.WriteLine("Found a report here!");
                                }
                            }
                        }
                    }

                    // locally track if a torn page is found in the world
                    if (check.Name.Contains("TornPage") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                        tornPageMemory[WorldNameToIndex("SorasHeart")] = true;
                    // locally track if a form is found in the world
                    if ((check.Name.Contains("Valor") || check.Name.Contains("Wisdom") || check.Name.Contains("Limit") || check.Name.Contains("Master") || check.Name.Contains("Final"))
                         && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                        driveFormMemory[WorldNameToIndex("SorasHeart")] = true;


                    // add check to levels
                    TrackItem(check.Name + count, SorasHeartGrid);
                    levelRewards.Remove(check.Name);
                }
                else if (driveRewards.Exists(x => x == check.Name))
                {
                    if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints) && false)
                    {
                        data.WorldsData["DriveForms"].hinted = true;
                        data.WorldsData["DriveForms"].hintedHint = true;

                        if (localHintMemory[WorldNameToIndex("DriveForms")] != -1)
                            SetReportValue(data.WorldsData["DriveForms"].hint, localHintMemory[WorldNameToIndex("DriveForms")] + 1);

                        // loop through hinted world for reports to set their info as hinted hints
                        for (int i = 0; i < data.WorldsData["DriveForms"].worldGrid.Children.Count; ++i)
                        {
                            Item gridItem = data.WorldsData["DriveForms"].worldGrid.Children[i] as Item;
                            if (gridItem.Name.Contains("Report"))
                            {
                                int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                //Console.WriteLine("Found a report here!");
                            }
                        }

                        // if a proof is found in forms, make every world that contains a form hinted
                        for (int i = 0; i < driveFormMemory.Length; i++)
                        {
                            if (driveFormMemory[i] && i != 1) // if the world has a form, set that world to be hinted
                            {
                                data.WorldsData[IndexToWorldName(i)].hinted = true;

                                // loop through hinted world for reports to set their info as hinted hints
                                for (int j = 0; j < data.WorldsData[IndexToWorldName(i)].worldGrid.Children.Count; ++j)
                                {
                                    Item gridItem = data.WorldsData[IndexToWorldName(i)].worldGrid.Children[j] as Item;
                                    if (gridItem.Name.Contains("Report"))
                                    {
                                        int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                        data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                        SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                        //Console.WriteLine("Found a report here!");
                                    }
                                }
                            }
                        }

                        // check and determine what world has the report for the world that just contained a proof
                        if (localReportMemory[WorldNameToIndex("DriveForms")] != -1 && localReportMemory[WorldNameToIndex("DriveForms")] != 16)
                        {
                            string hintedWorld = IndexToWorldName(localReportMemory[WorldNameToIndex("DriveForms")]);
                            // loop through world for reports to set their info as hinted hints
                            for (int i = 0; i < data.WorldsData[hintedWorld].worldGrid.Children.Count; ++i)
                            {
                                Item gridItem = data.WorldsData[hintedWorld].worldGrid.Children[i] as Item;
                                if (gridItem.Name.Contains("Report"))
                                {
                                    int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                    data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                    SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                    //Console.WriteLine("Found a report here!");
                                }
                            }
                        }
                    }

                    // locally track if a torn page is found in the world
                    if (check.Name.Contains("TornPage") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                        tornPageMemory[WorldNameToIndex("DriveForms")] = true;
                    // locally track if a form is found in the world
                    if ((check.Name.Contains("Valor") || check.Name.Contains("Wisdom") || check.Name.Contains("Limit") || check.Name.Contains("Master") || check.Name.Contains("Final"))
                         && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                        driveFormMemory[WorldNameToIndex("DriveForms")] = true;

                    // add check to drives
                    TrackItem(check.Name + count, DriveFormsGrid);
                    driveRewards.Remove(check.Name);
                }
                else
                {
                    if (data.WorldsData.ContainsKey(world.previousworldName))
                    {
                        if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints) && false)
                        {
                            data.WorldsData[world.worldName].hinted = true;
                            data.WorldsData[world.worldName].hintedHint = true;

                            if (localHintMemory[WorldNameToIndex(world.worldName)] != -1)
                                SetReportValue(data.WorldsData[world.worldName].hint, localHintMemory[WorldNameToIndex(world.worldName)] + 1);

                            // loop through hinted world for reports to set their info as hinted hints
                            for (int i = 0; i < data.WorldsData[world.worldName].worldGrid.Children.Count; ++i)
                            {
                                Item gridItem = data.WorldsData[world.worldName].worldGrid.Children[i] as Item;
                                if (gridItem.Name.Contains("Report"))
                                {
                                    int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                    data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                    SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                    //Console.WriteLine("Found a report here!");
                                }
                            }

                            // if a proof is found in 100AW, make every world that contains a page hinted
                            if (world.worldName == "HundredAcreWood")
                            {
                                for (int i = 0; i < tornPageMemory.Length; i++)
                                {
                                    if (tornPageMemory[i] && i != 9) // if the world has a torn page, set that world to be hinted
                                    {
                                        data.WorldsData[IndexToWorldName(i)].hinted = true;

                                        // loop through hinted world for reports to set their info as hinted hints
                                        for (int j = 0; j < data.WorldsData[IndexToWorldName(i)].worldGrid.Children.Count; ++j)
                                        {
                                            Item gridItem = data.WorldsData[IndexToWorldName(i)].worldGrid.Children[j] as Item;
                                            if (gridItem.Name.Contains("Report"))
                                            {
                                                int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                                data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                                SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                                //Console.WriteLine("Found a report here!");
                                            }
                                        }
                                    }
                                }
                            }

                            // check and determine what world has the report for the world that just contained a proof
                            if (localReportMemory[WorldNameToIndex(world.worldName)] != -1 && localReportMemory[WorldNameToIndex(world.worldName)] != 16)
                            {
                                string hintedWorld = IndexToWorldName(localReportMemory[WorldNameToIndex(world.worldName)]);
                                // loop through world for reports to set their info as hinted hints
                                for (int i = 0; i < data.WorldsData[hintedWorld].worldGrid.Children.Count; ++i)
                                {
                                    Item gridItem = data.WorldsData[hintedWorld].worldGrid.Children[i] as Item;
                                    if (gridItem.Name.Contains("Report"))
                                    {
                                        int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                                        data.WorldsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                                        SetReportValue(data.WorldsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                                        //Console.WriteLine("Found a report here!");
                                    }
                                }
                            }
                        }

                        // locally track if a torn page is found in the world
                        if (check.Name.Contains("TornPage") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                            tornPageMemory[WorldNameToIndex(world.worldName)] = true;
                        // locally track if a form is found in the world
                        if ((check.Name.Contains("Valor") || check.Name.Contains("Wisdom") || check.Name.Contains("Limit") || check.Name.Contains("Master") || check.Name.Contains("Final"))
                             && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
                            driveFormMemory[WorldNameToIndex(world.worldName)] = true;

                        // add check to current world
                        TrackItem(check.Name + count, data.WorldsData[world.previousworldName].worldGrid);
                    }
                }
            }
        }

        void UpdateWorldProgress(World world)
        {
            if (world.worldName == "SimulatedTwilightTown")
            {
                if (world.roomNumber == 1 && world.eventID3 == 56 && data.WorldsData[world.worldName].progress == 0) // Roxas' Room (Day 1)
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 34 && world.eventID1 == 157 && world.eventComplete == 1) // Twilight Thorn finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "TwilightThorn");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "TwilightThorn");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID1 == 88 && world.eventComplete == 1) // Setzer finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Struggle");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Struggle");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID3 == 55 && data.WorldsData[world.worldName].progress == 0) // Roxas' Room (Day 6)
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 21 && world.eventID3 == 1) // Mansion: Computer Room
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "ComputerRoom");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "ComputerRoom");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 20 && world.eventID1 == 137 && world.eventComplete == 1) // Axel finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Axel");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Axel");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "TwilightTown")
            {
                if (world.roomNumber == 27 && world.eventID3 == 4) // Yen Sid after new clothes
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "MysteriousTower");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "MysteriousTower");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 80 && world.eventComplete == 1) // Sandlot finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "Sandlot");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "Sandlot");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 41 && world.eventID1 == 186 && world.eventComplete == 1) // Mansion fight finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "Mansion");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "Mansion");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 40 && world.eventID1 == 161 && world.eventComplete == 1) // Betwixt and Between finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "BetwixtandBetween");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "BetwixtandBetween");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 20 && world.eventID1 == 213 && world.eventComplete == 1) // Data Axel finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "DataAxel");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "DataAxel");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "HollowBastion")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Villain's Vale (HB1)
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 8 && world.eventID1 == 52 && world.eventComplete == 1) // Bailey finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Bailey");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Bailey");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID3 == 2 && data.WorldsData[world.worldName].progress == 0) // Marketplace (HB2)
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID3 == 20) // Ansem Study post Computer
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "AnsemStudy");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "AnsemStudy");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 20 && world.eventID1 == 86 && world.eventComplete == 1) // Corridor finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Corridor");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Corridor");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 18 && world.eventID1 == 73 && world.eventComplete == 1) // Dancers finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Dancers");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Dancers");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // HB Demyx finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBDemyx");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBDemyx");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 16 && world.eventID1 == 65 && world.eventComplete == 1) // FF Cloud finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "FinalFantasy");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "FinalFantasy");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 17 && world.eventID1 == 66 && world.eventComplete == 1) // 1k Heartless finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "1000Heartless");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "1000Heartless");
                    data.WorldsData[world.worldName].progress = 8;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID1 == 75 && world.eventComplete == 1) // Sephiroth finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Sephiroth");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Sephiroth");
                    data.WorldsData[world.worldName].progress = 9;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 114 && world.eventComplete == 1) // Data Demyx finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "DataDemyx");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "DataDemyx");
                    data.WorldsData[world.worldName].progress = 10;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "BeastsCastle")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Entrance Hall (BC1)
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 11 && world.eventID1 == 72 && world.eventComplete == 1) // Thresholder finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Thresholder");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Thresholder");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID1 == 69 && world.eventComplete == 1) // Beast finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Beast");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Beast");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID1 == 79 && world.eventComplete == 1) // Dark Thorn finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "DarkThorn");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "DarkThorn");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 2 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Belle's Room (BC2)
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 74 && world.eventComplete == 1) // Dragoons finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Dragoons");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Dragoons");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 15 && world.eventID1 == 82 && world.eventComplete == 1) // Xaldin finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Xaldin");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Xaldin");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 15 && world.eventID1 == 97 && world.eventComplete == 1) // Data Xaldin finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "DataXaldin");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "DataXaldin");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "OlympusColiseum")
            {
                if (world.roomNumber == 0 & world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // The Coliseum (OC1)
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 7 && world.eventID1 == 114 && world.eventComplete == 1) // Cerberus finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Cerberus");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Cerberus");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 17 && world.eventID1 == 123 && world.eventComplete == 1) // OC Demyx finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCDemyx");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCDemyx");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 8 && world.eventID1 == 116 && world.eventComplete == 1) // OC Pete finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCPete");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCPete");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 18 && world.eventID1 == 171 && world.eventComplete == 1) // Hydra finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hydra");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hydra");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 & world.eventID3 == 12 && data.WorldsData[world.worldName].progress == 0) // Underworld Entrance (OC2)
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 6 && world.eventID1 == 126 && world.eventComplete == 1) // Auron Statue fight finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "AuronStatue");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "AuronStatue");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 19 && world.eventID1 == 202 && world.eventComplete == 1) // Hades finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hades");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hades");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 34 && (world.eventID1 == 151 || world.eventID1 == 152) && world.eventComplete == 1) // Zexion finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Zexion");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Zexion");
                    data.WorldsData[world.worldName].progress = 8;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "Agrabah")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Agrabah (Ag1)
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID1 == 2 && world.eventComplete == 1) // Abu finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Abu");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Abu");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 13 && world.eventID1 == 79 && world.eventComplete == 1) // Chasm fight finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Chasm");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Chasm");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 58 && world.eventComplete == 1) // Treasure Room finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "TreasureRoom");
                    AgrabahProgression.SetResourceReference(ContentProperty, "TreasureRoom");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID1 == 59 && world.eventComplete == 1) // Lords finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Lords");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Lords");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // The Vault (Ag2)
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 14 && world.eventID1 == 100 && world.eventComplete == 1) // Carpet finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Carpet");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Carpet");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID1 == 62 && world.eventComplete == 1) // Genie Jafar finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "GenieJafar");
                    AgrabahProgression.SetResourceReference(ContentProperty, "GenieJafar");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 142 || world.eventID1 == 147) && world.eventComplete == 1) // Lexaeus finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Lexaeus");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Lexaeus");
                    data.WorldsData[world.worldName].progress = 8;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "LandofDragons")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Bamboo Grove (LoD1)
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID1 == 72 && world.eventComplete == 1) // Cave finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "Cave");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "Cave");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 7 && world.eventID1 == 73 && world.eventComplete == 1) // Summit finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "Summit");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "Summit");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID1 == 75 && world.eventComplete == 1) // Shan Yu finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "ShanYu");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "ShanYu");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 12 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Village (LoD2)
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 78 && world.eventComplete == 1) // Antechamber fight finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "ThroneRoom");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "ThroneRoom");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 8 && world.eventID1 == 79 && world.eventComplete == 1) // Storm Rider finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "StormRider");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "StormRider");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "HundredAcreWood")
            {
                if (world.roomNumber == 2 && (world.eventID3 == 1 || world.eventID3 == 22)) // Pooh's house (eventID3 == 1 is when not skipping AW0)
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Pooh");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Pooh");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID3 == 1) // Piglet's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Piglet");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Piglet");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID3 == 1) // Rabbit's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Rabbit");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Rabbit");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 5 && world.eventID3 == 1) // Kanga's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Kanga");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Kanga");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID3 == 1) // Spooky Cave
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "SpookyCave");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "SpookyCave");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID3 == 1) // Starry Hill
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "StarryHill");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "StarryHill");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "PrideLands")
            {
                if (world.roomNumber == 16 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Wildebeest Valley (PL1)
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 12 && world.eventID3 == 1) // Oasis after talking to Simba
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Simba");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Simba");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 14 && world.eventID1 == 55 && world.eventComplete == 1) // Scar finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Scar");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Scar");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Savannah (PL2)
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 15 && world.eventID1 == 59 && world.eventComplete == 1) // Groundshaker finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Groundshaker");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Groundshaker");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "Atlantica")
            {
                if (world.roomNumber == 2 && world.eventID1 == 63) // Tutorial
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "Tutorial");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID1 == 65) // Ursula's Revenge
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "Ursula");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55) // A New Day is Dawning
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "NewDay");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "DisneyCastle")
            {
                if (world.roomNumber == 1 && world.eventID1 == 53 && data.WorldsData[world.worldName].progress == 0) // Library (DC)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 0 && world.eventID1 == 51 && world.eventComplete == 1) // Minnie Escort finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Minnie");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Minnie");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 0 && world.eventID3 == 22 && data.WorldsData[world.worldName].progress == 0) // Cornerstone Hill (TR) (Audience Chamber has no Evt 0x16)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID1 == 58 && world.eventComplete == 1) // Old Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "OldPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "OldPete");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 0 && world.eventID3 == 6) // Windows popup (Audience Chamber has no Evt 0x06)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Windows");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Windows");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 2 && world.eventID1 == 52 && world.eventComplete == 1) // Boat Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "BoatPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "BoatPete");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID1 == 53 && world.eventComplete == 1) // DC Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCPete");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 38 && (world.eventID1 == 145 || world.eventID1 == 150) && world.eventComplete == 1) // Marluxia finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Marluxia");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Marluxia");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 7 && world.eventID1 == 67 && world.eventComplete == 1) // Lingering Will finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "LingeringWill");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "LingeringWill");
                    data.WorldsData[world.worldName].progress = 8;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "HalloweenTown")
            {
                if (world.roomNumber == 4 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Hinterlands (HT1)
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 6 && world.eventID1 == 53 && world.eventComplete == 1) // Candy Cane Lane fight finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "CandyCaneLane");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "CandyCaneLane");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID1 == 52 && world.eventComplete == 1) // Prison Keeper finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "PrisonKeeper");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "PrisonKeeper");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID1 == 55 && world.eventComplete == 1) // Oogie Boogie finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "OogieBoogie");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "OogieBoogie");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Dr. Finklestein's Lab (HT2)
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 63 && world.eventComplete == 1) // Presents minigame
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Presents");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Presents");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 7 && world.eventID1 == 64 && world.eventComplete == 1) // Experiment finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Experiment");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Experiment");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 32 && (world.eventID1 == 115 || world.eventID1 == 146) && world.eventComplete == 1) // Vexen finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Vexen");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Vexen");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "PortRoyal")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Rampart (PR1)
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 2 && world.eventID1 == 55 && world.eventComplete == 1) // Town finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Town");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Town");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 60 && world.eventComplete == 1) // Barbossa finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Barbossa");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Barbossa");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Treasure Heap (PR2)
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 14 && world.eventID1 == 62 && world.eventComplete == 1) // Gambler finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Gambler");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Gambler");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID1 == 54 && world.eventComplete == 1) // Grim Reaper finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "GrimReaper");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "GrimReaper");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "SpaceParanoids")
            {
                if (world.roomNumber == 1 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Canyon (SP1)
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 3 && world.eventID1 == 54 && world.eventComplete == 1) // Screens finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Screens");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Screens");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // Hostile Program finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "HostileProgram");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "HostileProgram");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 1 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Canyon (SP2)
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 7 && world.eventID1 == 57 && world.eventComplete == 1) // Solar Sailer finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SolarSailer");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SolarSailer");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 9 && world.eventID1 == 59 && world.eventComplete == 1) // MCP finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "MCP");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "MCP");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 143 || world.eventID1 == 148) && world.eventComplete == 1) // Larxene finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Larxene");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Larxene");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
            }
            else if (world.worldName == "TWTNW")
            {
                if (world.roomNumber == 1 && world.eventID3 == 1) // Alley to Between
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "TWTNWChests");
                    TWTNWProgression.SetResourceReference(ContentProperty, "TWTNWChests");
                    data.WorldsData[world.worldName].progress = 1;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 21 && world.eventID1 == 65 && world.eventComplete == 1) // Roxas finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Roxas");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Roxas");
                    data.WorldsData[world.worldName].progress = 2;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 57 && world.eventComplete == 1) // Xigbar finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Xigbar");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Xigbar");
                    data.WorldsData[world.worldName].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 14 && world.eventID1 == 58 && world.eventComplete == 1) // Luxord finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Luxord");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Luxord");
                    data.WorldsData[world.worldName].progress = 4;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 15 && world.eventID1 == 56 && world.eventComplete == 1) // Saix finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Saix");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Saix");
                    data.WorldsData[world.worldName].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 19 && world.eventID1 == 59 && world.eventComplete == 1) // Xemnas 1 finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Xemnas1");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Xemnas1");
                    data.WorldsData[world.worldName].progress = 6;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 20 && world.eventID1 == 98 && world.eventComplete == 1) // Data Xemnas finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "DataXemnas");
                    TWTNWProgression.SetResourceReference(ContentProperty, "DataXemnas");
                    data.WorldsData[world.worldName].progress = 7;
                    CheckLastEventProgression();
                }

                // Handle data fights
                else if (world.roomNumber == 21 && world.eventID1 == 99 && world.eventComplete == 1) // Data Roxas finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "DataRoxas");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "DataRoxas");
                    data.WorldsData["SimulatedTwilightTown"].progress = 3;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 10 && world.eventID1 == 100 && world.eventComplete == 1) // Data Xigbar finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "DataXigbar");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "DataXigbar");
                    data.WorldsData["LandofDragons"].progress = 7;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 15 && world.eventID1 == 102 && world.eventComplete == 1) // Data Saix finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "DataSaix");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "DataSaix");
                    data.WorldsData["PrideLands"].progress = 5;
                    CheckLastEventProgression();
                }
                else if (world.roomNumber == 14 && world.eventID1 == 101 && world.eventComplete == 1) // Data Luxord finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "DataLuxord");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "DataLuxord");
                    data.WorldsData["PortRoyal"].progress = 6;
                    CheckLastEventProgression();
                }
            }

            //progression hints call

        }

        private string BytesToHex(byte[] bytes)
        {
            if (Enumerable.SequenceEqual(bytes, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }))
            {
                return "Service not started. Waiting for PCSX2";
            }
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private void BindStats(Image img, string property, object source)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new NumberConverter();
            img.SetBinding(Image.SourceProperty, binding);
        }

        private void BindLevel(Image img, string property, object source)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new LevelConverter();
            img.SetBinding(Image.SourceProperty, binding);
        }

        private void BindForm(ContentControl img, string property, object source)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new ObtainedConverter();
            img.SetBinding(OpacityProperty, binding);
        }

        private void BindFormLevel(Image img, string property, object source, IValueConverter convertor)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new LevelConverter();
            img.SetBinding(Image.SourceProperty, binding);
        }

        private void BindWeapon(Image img, string property, object source)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new WeaponConverter();
            img.SetBinding(Image.SourceProperty, binding);
        }

        private void BindAbilityLevel(Image img, string property, object source, IValueConverter convertor)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = convertor;
            img.SetBinding(Image.SourceProperty, binding);
        }

        private void BindAbility(Image img, string property, object source)
        {
            Binding binding = new Binding(property);
            binding.Source = source;
            binding.Converter = new ObtainedConverter();
            img.SetBinding(OpacityProperty, binding);
        }

        public string GetWorld()
        {
            return world.worldName;
        }

        public void SetLocalHintValues(string worldName, int value)
        {
            if (data.mode != Mode.Hints && data.mode != Mode.OpenKHHints)
                return;

            //stores locally the value of a report for a world - i.e. remembers that TWTNW is a 5
            localHintMemory[WorldNameToIndex(worldName)] = value;

            //stores locally what world hinted it - i.e. remembers that LoD was hinted by HB
            localReportMemory[WorldNameToIndex(worldName)] = WorldNameToIndex(GetWorld());
        }

        private int WorldNameToIndex(string worldName)
        {
            if (worldName == "SorasHeart")
                return 0;
            else if (worldName == "DriveForms")
                return 1;
            else if (worldName == "SimulatedTwilightTown")
                return 2;
            else if (worldName == "TwilightTown")
                return 3;
            else if (worldName == "HollowBastion")
                return 4;
            else if (worldName == "BeastsCastle")
                return 5;
            else if (worldName == "OlympusColiseum")
                return 6;
            else if (worldName == "Agrabah")
                return 7;
            else if (worldName == "LandofDragons")
                return 8;
            else if (worldName == "HundredAcreWood")
                return 9;
            else if (worldName == "PrideLands")
                return 10;
            else if (worldName == "DisneyCastle")
                return 11;
            else if (worldName == "HalloweenTown")
                return 12;
            else if (worldName == "PortRoyal")
                return 13;
            else if (worldName == "SpaceParanoids")
                return 14;
            else if (worldName == "TWTNW")
                return 15;
            else if (worldName == "GoA")
                return 16;
            else
                return 17;
        }
        private string IndexToWorldName(int index)
        {
            if (index == 0)
                return "SorasHeart";
            else if (index == 1)
                return "DriveForms";
            else if (index == 2)
                return "SimulatedTwilightTown";
            else if (index == 3)
                return "TwilightTown";
            else if (index == 4)
                return "HollowBastion";
            else if (index == 5)
                return "BeastsCastle";
            else if (index == 6)
                return "OlympusColiseum";
            else if (index == 7)
                return "Agrabah";
            else if (index == 8)
                return "LandofDragons";
            else if (index == 9)
                return "HundredAcreWood";
            else if (index == 10)
                return "PrideLands";
            else if (index == 11)
                return "DisneyCastle";
            else if (index == 12)
                return "HalloweenTown";
            else if (index == 13)
                return "PortRoyal";
            else if (index == 14)
                return "SpaceParanoids";
            else if (index == 15)
                return "TWTNW";
            else if (index == 16)
                return "GoA";
            else
                return "Atlantica";
        }

        public void ResetLocaLHintMemory()
        {
            localHintMemory = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            localReportMemory = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            tornPageMemory = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            driveFormMemory = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        }

        public async void SetHintTextDelayed(string msg)
        {
            await Task.Delay(30000);
            if ((string) HintText.Content == "PC Detected - Tracking" || (string) HintText.Content == "PCSX2 Detected - Tracking")
                HintText.Content = msg;
        }

        //progression based hints
        public void UpdateProgressPoints_H(int pp)
        {
            if (pp == 0)
                return;

            stats.UpdateProgressPoints(pp);

            if (stats.hintIndex >= 13)
                return;

            while (stats.ProgressPoints >= requiredPoints[stats.hintIndex])
            {
                stats.UpdateProgressPoints(-1 * requiredPoints[stats.hintIndex]);
                int tempIndex = stats.GetNextHintIndex();
                SetHintText(Codes.GetHintTextName(data.reportInformation[tempIndex].Item1) + " has " + data.reportInformation[tempIndex].Item2 + " important checks");
                SetLocalHintValues(data.reportInformation[tempIndex].Item1, data.reportInformation[tempIndex].Item2);
                SetReportValue(data.WorldsData[data.reportInformation[tempIndex].Item1].hint, data.reportInformation[tempIndex].Item2 + 1);
                data.WorldsData[data.reportInformation[tempIndex].Item1].hinted = true;

                if (stats.hintIndex >= 13)
                    SetNumbersBlue();
            }

            if (requiredTotal != requiredPoints[stats.hintIndex])
            {
                SetRequiredPoints(requiredPoints[stats.hintIndex]);
            }
        }

        private void CheckLastEventProgression()
        {
            if (lastEvent[0] != WorldNameToIndex(world.worldName) || lastEvent[1] != world.roomNumber || lastEvent[2] != world.eventID1)
            {
                lastEvent = new int[] { WorldNameToIndex(world.worldName), world.roomNumber, world.eventID1 };
                stats.UpdateProgressPoints(GetProgressionPoints());

                if (stats.hintIndex >= 13)
                    return;

                while (stats.ProgressPoints >= requiredPoints[stats.hintIndex])
                {
                    stats.UpdateProgressPoints(-1 * requiredPoints[stats.hintIndex]);
                    int tempIndex = stats.GetNextHintIndex();
                    SetHintText(Codes.GetHintTextName(data.reportInformation[tempIndex].Item1) + " has " + data.reportInformation[tempIndex].Item2 + " important checks");
                    SetLocalHintValues(data.reportInformation[tempIndex].Item1, data.reportInformation[tempIndex].Item2);
                    SetReportValue(data.WorldsData[data.reportInformation[tempIndex].Item1].hint, data.reportInformation[tempIndex].Item2 + 1);
                    data.WorldsData[data.reportInformation[tempIndex].Item1].hinted = true;

                    if (stats.hintIndex >= 13)
                        SetNumbersBlue();
                }
            }

            if (requiredTotal != requiredPoints[stats.hintIndex])
            {
                SetRequiredPoints(requiredPoints[stats.hintIndex]);
            }
        }

        //returns values of each progress spot
        private int GetProgressionPoints()
        {
            if (world.worldName == "SimulatedTwilightTown")
            {
                if (world.roomNumber == 34 && world.eventID1 == 157 && world.eventComplete == 1) // Twilight Thorn finish
                {
                    return stt_TwilightThorn;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 88 && world.eventComplete == 1) // Setzer finish
                {
                    return stt_Struggle;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 137 && world.eventComplete == 1) // Axel finish
                {
                    return stt_Axel;
                }
            }
            else if (world.worldName == "TwilightTown")
            {
                if (world.roomNumber == 27 && world.eventID3 == 4) // Yen Sid after new clothes
                {
                    return tt_Tower;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 80 && world.eventComplete == 1) // Sandlot finish
                {
                    return tt_Sandlot;
                }
                else if (world.roomNumber == 41 && world.eventID1 == 186 && world.eventComplete == 1) // Mansion fight finish
                {
                    return tt_Mansion;
                }
                else if (world.roomNumber == 40 && world.eventID1 == 161 && world.eventComplete == 1) // Betwixt and Between finish
                {
                    return tt_Betwixt;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 213 && world.eventComplete == 1) // Data Axel finish
                {
                    return tt_DataAxel;
                }
            }
            else if (world.worldName == "HollowBastion")
            {
                if (world.roomNumber == 8 && world.eventID1 == 52 && world.eventComplete == 1) // Bailey finish
                {
                    return hb_Bailey;
                }
                if (world.roomNumber == 5 && world.eventID3 == 20) // Ansem Study post Computer
                {
                    return hb_Ansem;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 86 && world.eventComplete == 1) // Corridor finish
                {
                    return hb_Corridors;
                }
                else if (world.roomNumber == 18 && world.eventID1 == 73 && world.eventComplete == 1) // Dancers finish
                {
                    return hb_Dancers;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // HB Demyx finish
                {
                    return hb_Demyx;
                }
                else if (world.roomNumber == 16 && world.eventID1 == 65 && world.eventComplete == 1) // FF Cloud finish
                {
                    return hb_FFFights;
                }
                else if (world.roomNumber == 17 && world.eventID1 == 66 && world.eventComplete == 1) // 1k Heartless finish
                {
                    return hb_1k;
                }
                else if (world.roomNumber == 1 && world.eventID1 == 75 && world.eventComplete == 1) // Sephiroth finish
                {
                    return hb_Sephiroth;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 114 && world.eventComplete == 1) // Data Demyx finish
                {
                    return hb_DataDemyx;
                }
            }
            else if (world.worldName == "BeastsCastle")
            {
                if (world.roomNumber == 11 && world.eventID1 == 72 && world.eventComplete == 1) // Thresholder finish
                {
                    return bc_Thresholder;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 69 && world.eventComplete == 1) // Beast finish
                {
                    return bc_Beast;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 79 && world.eventComplete == 1) // Dark Thorn finish
                {
                    return bc_DarkThorn;
                }
                if (world.roomNumber == 4 && world.eventID1 == 74 && world.eventComplete == 1) // Dragoons finish
                {
                    return bc_Dragoons;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 82 && world.eventComplete == 1) // Xaldin finish
                {
                    return bc_Xaldin;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 97 && world.eventComplete == 1) // Data Xaldin finish
                {
                    return bc_DataXaldin;
                }
            }
            else if (world.worldName == "OlympusColiseum")
            {
                if (world.roomNumber == 7 && world.eventID1 == 114 && world.eventComplete == 1) // Cerberus finish
                {
                    return oc_Cerberus;
                }
                else if (world.roomNumber == 17 && world.eventID1 == 123 && world.eventComplete == 1) // OC Demyx finish
                {
                    return oc_Demyx;
                }
                else if (world.roomNumber == 8 && world.eventID1 == 116 && world.eventComplete == 1) // OC Pete finish
                {
                    return oc_Pete;
                }
                else if (world.roomNumber == 18 && world.eventID1 == 171 && world.eventComplete == 1) // Hydra finish
                {
                    return oc_Hydra;
                }
                if (world.roomNumber == 6 && world.eventID1 == 126 && world.eventComplete == 1) // Auron Statue fight finish
                {
                    return oc_Auron;
                }
                else if (world.roomNumber == 19 && world.eventID1 == 202 && world.eventComplete == 1) // Hades finish
                {
                    return oc_Hades;
                }
                else if (world.roomNumber == 34 && (world.eventID1 == 151 || world.eventID1 == 152) && world.eventComplete == 1) // Zexion finish
                {
                    return oc_Zexion;
                }
            }
            else if (world.worldName == "Agrabah")
            {
                if (world.roomNumber == 9 && world.eventID1 == 2 && world.eventComplete == 1) // Abu finish
                {
                    return ag_Abu;
                }
                else if (world.roomNumber == 13 && world.eventID1 == 79 && world.eventComplete == 1) // Chasm fight finish
                {
                    return ag_Chasm;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 58 && world.eventComplete == 1) // Treasure Room finish
                {
                    return ag_Treasure;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 59 && world.eventComplete == 1) // Lords finish
                {
                    return ag_Twins;
                }
                if (world.roomNumber == 14 && world.eventID1 == 100 && world.eventComplete == 1) // Carpet finish
                {
                    return ag_Carpet;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 62 && world.eventComplete == 1) // Genie Jafar finish
                {
                    return ag_Jafar;
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 142 || world.eventID1 == 147) && world.eventComplete == 1) // Lexaeus finish
                {
                    return ag_Lexaeus;
                }
            }
            else if (world.worldName == "LandofDragons")
            {
                if (world.roomNumber == 5 && world.eventID1 == 72 && world.eventComplete == 1) // Cave finish
                {
                    return lod_Cave;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 73 && world.eventComplete == 1) // Summit finish
                {
                    return lod_Summit;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 75 && world.eventComplete == 1) // Shan Yu finish
                {
                    return lod_ShanYu;
                }
                if (world.roomNumber == 10 && world.eventID1 == 78 && world.eventComplete == 1) // Antechamber fight finish
                {
                    return lod_Throne;
                }
                else if (world.roomNumber == 8 && world.eventID1 == 79 && world.eventComplete == 1) // Storm Rider finish
                {
                    return lod_StormRider;
                }
            }
            else if (world.worldName == "HundredAcreWood")
            {
                if (world.roomNumber == 4 && world.eventID3 == 1) // Piglet's house
                {
                    return ha_Piglet;
                }
                else if (world.roomNumber == 3 && world.eventID3 == 1) // Rabbit's house
                {
                    return ha_Rabbit;
                }
                else if (world.roomNumber == 5 && world.eventID3 == 1) // Kanga's house
                {
                    return ha_Kanga;
                }
                else if (world.roomNumber == 9 && world.eventID3 == 1) // Spooky Cave
                {
                    return ha_SpookyCave;
                }
                else if (world.roomNumber == 1 && world.eventID3 == 1) // Starry Hill
                {
                    return ha_StarryHill;
                }
            }
            else if (world.worldName == "PrideLands")
            {
                //if (world.roomNumber == 16 && world.eventID3 == 1) // Wildebeest Valley (PL1)
                //{
                //    return 20;
                //}
                if (world.roomNumber == 12 && world.eventID3 == 1) // Oasis after talking to Simba
                {
                    return pl_Simba;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 55 && world.eventComplete == 1) // Scar finish
                {
                    return pl_Scar;
                }
                else if (world.roomNumber == 4 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Savannah (PL2)
                {
                    return pl_Groundshaker;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 59 && world.eventComplete == 1) // Groundshaker finish
                {
                    return pl_DataSaix;
                }
            }
            else if (world.worldName == "Atlantica")
            {
                if (world.roomNumber == 2 && world.eventID1 == 63) // Tutorial
                {
                    return at_Tutorial;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 65) // Ursula's Revenge
                {
                    return at_Ursula;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55) // A New Day is Dawning
                {
                    return at_NewDay;
                }
            }
            else if (world.worldName == "DisneyCastle")
            {
                if (world.roomNumber == 0 && world.eventID1 == 51 && world.eventComplete == 1) // Minnie Escort finish
                {
                    return dc_Minnie;
                }
                if (world.roomNumber == 1 && world.eventID1 == 58 && world.eventComplete == 1) // Old Pete finish
                {
                    return dc_OldPete;
                }
                else if (world.roomNumber == 0 && world.eventID3 == 6) // Windows popup (Audience Chamber has no Evt 0x06)
                {
                    return dc_Windows;
                }
                else if (world.roomNumber == 2 && world.eventID1 == 52 && world.eventComplete == 1) // Boat Pete finish
                {
                    return dc_Steamboat;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 53 && world.eventComplete == 1) // DC Pete finish
                {
                    return dc_NewPete;
                }
                else if (world.roomNumber == 38 && (world.eventID1 == 145 || world.eventID1 == 150) && world.eventComplete == 1) // Marluxia finish
                {
                    return dc_Marluxia;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 67 && world.eventComplete == 1) // Lingering Will finish
                {
                    return dc_Terra;
                }
            }
            else if (world.worldName == "HalloweenTown")
            {
                if (world.roomNumber == 6 && world.eventID1 == 53 && world.eventComplete == 1) // Candy Cane Lane fight finish
                {
                    return ht_CandyCaneLane;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 52 && world.eventComplete == 1) // Prison Keeper finish
                {
                    return ht_PrisonKeeper;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 55 && world.eventComplete == 1) // Oogie Boogie finish
                {
                    return ht_Oogie;
                }
                if (world.roomNumber == 10 && world.eventID1 == 63 && world.eventComplete == 1) // Presents minigame
                {
                    return ht_Presents;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 64 && world.eventComplete == 1) // Experiment finish
                {
                    return ht_Experiment;
                }
                else if (world.roomNumber == 32 && (world.eventID1 == 115 || world.eventID1 == 146) && world.eventComplete == 1) // Vexen finish
                {
                    return ht_Vexen;
                }
            }
            else if (world.worldName == "PortRoyal")
            {
                if (world.roomNumber == 2 && world.eventID1 == 55 && world.eventComplete == 1) // Town finish
                {
                    return pr_Town;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 60 && world.eventComplete == 1) // Barbossa finish
                {
                    return pr_Barbossa;
                }
                if (world.roomNumber == 14 && world.eventID1 == 62 && world.eventComplete == 1) // Gambler finish
                {
                    return pr_Gambler;
                }
                else if (world.roomNumber == 1 && world.eventID1 == 54 && world.eventComplete == 1) // Grim Reaper finish
                {
                    return pr_Grim2;
                }
            }
            else if (world.worldName == "SpaceParanoids")
            {
                if (world.roomNumber == 3 && world.eventID1 == 54 && world.eventComplete == 1) // Screens finish
                {
                    return sp_Screens;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // Hostile Program finish
                {
                    return sp_HostileProgram;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 57 && world.eventComplete == 1) // Solar Sailer finish
                {
                    return sp_SolarSailer;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 59 && world.eventComplete == 1) // MCP finish
                {
                    return sp_MCP;
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 143 || world.eventID1 == 148) && world.eventComplete == 1) // Larxene finish
                {
                    return sp_Larxene;
                }
            }
            else if (world.worldName == "TWTNW")
            {
                if (world.roomNumber == 1 && world.eventID3 == 1) // Alley to Between
                {
                    return 1;
                }
                else if (world.roomNumber == 21 && world.eventID1 == 65 && world.eventComplete == 1) // Roxas finish
                {
                    return twtnw_Roxas;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 57 && world.eventComplete == 1) // Xigbar finish
                {
                    return twtnw_Xigbar;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 58 && world.eventComplete == 1) // Luxord finish
                {
                    return twtnw_Luxord;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 56 && world.eventComplete == 1) // Saix finish
                {
                    return twtnw_Saix;
                }
                else if (world.roomNumber == 19 && world.eventID1 == 59 && world.eventComplete == 1) // Xemnas 1 finish
                {
                    return twtnw_Xemnas;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 98 && world.eventComplete == 1) // Data Xemnas finish
                {
                    return twtnw_DataXemnas;
                }

                // Handle data fights
                else if (world.roomNumber == 21 && world.eventID1 == 99 && world.eventComplete == 1) // Data Roxas finish
                {
                    return stt_DataRoxas;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 100 && world.eventComplete == 1) // Data Xigbar finish
                {
                    return lod_DataXigbar;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 102 && world.eventComplete == 1) // Data Saix finish
                {
                    return pl_DataSaix;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 101 && world.eventComplete == 1) // Data Luxord finish
                {
                    return pr_DataLuxord;
                }
            }

            return 0;
        }

        private int ProgressionHintsSoraLevels()
        {
            if (stats.Level > sora_PreviousLevel)
            {
                if (stats.Level >= 10 && sora_NextPPLevel == 10)
                {
                    sora_PreviousLevel = stats.Level;
                    sora_NextPPLevel = 20;
                    return sora_Level10;
                }
                else if (stats.Level >= 20 && sora_NextPPLevel == 20)
                {
                    sora_PreviousLevel = stats.Level;
                    sora_NextPPLevel = 30;
                    return sora_Level20;
                }
                else if (stats.Level >= 30 && sora_NextPPLevel == 30)
                {
                    sora_PreviousLevel = stats.Level;
                    sora_NextPPLevel = 40;
                    return sora_Level30;
                }
                else if (stats.Level >= 40 && sora_NextPPLevel == 40)
                {
                    sora_PreviousLevel = stats.Level;
                    sora_NextPPLevel = 50;
                    return sora_Level40;
                }
                else if (stats.Level >= 50 && sora_NextPPLevel == 50)
                {
                    sora_PreviousLevel = stats.Level;
                    sora_NextPPLevel = 99;
                    return sora_Level50;
                }
            }

            return 0;
        }

        private int ProgressionHintsDriveLevels()
        {
            if (valor.Level > valor_PreviousLevel)
            {
                if (valor.Level >= 5 && valor_NextLevel == 5)
                {
                    valor_PreviousLevel = valor.Level;
                    valor_NextLevel = 7;
                    return valor_Level5;
                }
                else if (valor.Level >= 7 && valor_NextLevel == 7)
                {
                    valor_PreviousLevel = valor.Level;
                    valor_NextLevel = 8;
                    return valor_Level7;
                }
            }
            if (wisdom.Level > wisdom_PreviousLevel)
            {
                if (wisdom.Level >= 5 && wisdom_NextLevel == 5)
                {
                    wisdom_PreviousLevel = wisdom.Level;
                    wisdom_NextLevel = 7;
                    return wisdom_Level5;
                }
                else if (wisdom.Level >= 7 && wisdom_NextLevel == 7)
                {
                    wisdom_PreviousLevel = wisdom.Level;
                    wisdom_NextLevel = 8;
                    return wisdom_Level7;
                }
            }
            if (limit.Level > limit_PreviousLevel)
            {
                if (limit.Level >= 5 && limit_NextLevel == 5)
                {
                    limit_PreviousLevel = limit.Level;
                    limit_NextLevel = 7;
                    return limit_Level5;
                }
                else if (limit.Level >= 7 && limit_NextLevel == 7)
                {
                    limit_PreviousLevel = limit.Level;
                    limit_NextLevel = 8;
                    return limit_Level7;
                }
            }
            if (master.Level > master_PreviousLevel)
            {
                if (master.Level >= 5 && master_NextLevel == 5)
                {
                    master_PreviousLevel = master.Level;
                    master_NextLevel = 7;
                    return master_Level5;
                }
                else if (master.Level >= 7 && master_NextLevel == 7)
                {
                    master_PreviousLevel = master.Level;
                    master_NextLevel = 8;
                    return master_Level7;
                }
            }
            if (final.Level > final_PreviousLevel)
            {
                if (final.Level >= 5 && final_NextLevel == 5)
                {
                    final_PreviousLevel = final.Level;
                    final_NextLevel = 7;
                    return final_Level5;
                }
                else if (final.Level >= 7 && final_NextLevel == 7)
                {
                    final_PreviousLevel = final.Level;
                    final_NextLevel = 8;
                    return final_Level7;
                }
            }

            return 0;
        }

        public void ResetProgressionHints()
        {
            stats.SetProgressPoints(0);
            stats.ResetProgressPoints();
        }

        public async void SetNumbersBlue()
        {
            await Task.Delay(500);
            //Console.WriteLine("Where do I crash?");
            for (int i = 0; i < 13; ++i)
            {
                //Console.WriteLine("Here? " + i);
                if (data.WorldsData[data.reportInformation[i].Item1].hinted)
                {
                    //Console.WriteLine(data.WorldsData[data.reportInformation[i].Item1].checkCount);
                    data.WorldsData[data.reportInformation[i].Item1].hintedHint = true;
                    SetReportValue(data.WorldsData[data.reportInformation[i].Item1].hint, data.reportInformation[i].Item2 + 1);
                    //Console.WriteLine("Found a report here!");
                }

                SetReportValue(data.WorldsData[data.reportInformation[i].Item1].hint, data.reportInformation[i].Item2 + 1);
            }
        }
    }
}
