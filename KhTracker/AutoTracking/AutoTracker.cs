﻿using System;
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

        //                                 Sora   Drive   STT    TT     HB     BC     OC     AG     LoD   100AW   PL     DC     HT     PR     SP   TWTNW    GoA    AT
        //                                   0      1      2      3      4      5      6      7      8      9     10     11     12     13     14     15     16     17
        public int[] localHintMemory =   {  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1   };
        public int[] localReportMemory = {  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1  ,  -1   };
        public bool[] tornPageMemory =   { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        public bool[] driveFormMemory =  { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

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

            if (!HotkeyOption.IsChecked)
                return;

            if (!usedHotkey)
            {
                usedHotkey = true;
                InitAutoTrackerOriginal(true);
            }
        }
        public void StartPCHotkey()
        {
            Console.WriteLine("Hotkey pressed PC");

            if (!HotkeyOption.IsChecked)
                return;

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
                    ResetHotkeyState();
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
                    ResetHotkeyState();
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
                    ResetHotkeyState();
                    isWorking = false;
                    SetAutoDetectTimer();
                    return;
                }
                catch
                {
                    memory = null;
                    MessageBox.Show("Error connecting to KH2FM");
                    ResetHotkeyState();
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

            LevelRewardIcon.Visibility = Visibility.Visible;
            LevelReward.Visibility = Visibility.Visible;

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
                ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }
            catch
            {
                memory = null;
                MessageBox.Show("Error connecting to KH2FM");
                ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }

            HintText.Content = "PC Detected - Tracking";
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

            if (NextLevelRewardOption50.IsChecked || NextLevelRewardOption99.IsChecked)
            {
                LevelRewardIcon.Visibility = Visibility.Visible;
                LevelReward.Visibility = Visibility.Visible;

                if (NextLevelRewardOption50.IsChecked)
                    stats.SetMaxLevelRewardCheck(50);
                else
                    stats.SetMaxLevelRewardCheck(99);
            }
            else
                stats.SetMaxLevelRewardCheck(1);

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
                ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }
            catch
            {
                memory = null;
                MessageBox.Show("Error connecting to PCSX2");
                ResetHotkeyState();
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

            BindStats(LevelReward, "LevelReward", stats);

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


                importantChecks.ForEach(delegate (ImportantCheck importantCheck)
                {
                    importantCheck.UpdateMemory();
                });
            }
            catch
            {
                aTimer.Stop();
                //MessageBox.Show("KH2FM has exited. Stopping Auto Tracker.");
                ResetHotkeyState();
                isWorking = false;
                SetAutoDetectTimer();
                return;
            }

            UpdateCollectedItems();
            DetermineItemLocations();
            stats.SetNextLevelCheck(stats.Level);
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
                    if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
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
                    if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
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
                        if ((check.Name == "Peace" || check.Name == "Nonexistence" || check.Name == "Connection") && (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints))
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
                }
                else if (world.roomNumber == 34 && world.eventID1 == 157 && world.eventComplete == 1) // Twilight Thorn finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "TwilightThorn");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "TwilightThorn");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 88 && world.eventComplete == 1) // Setzer finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Struggle");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Struggle");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 1 && world.eventID3 == 55 && data.WorldsData[world.worldName].progress == 0) // Roxas' Room (Day 6)
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "STTChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 21 && world.eventID3 == 1) // Mansion: Computer Room
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "ComputerRoom");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "ComputerRoom");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 137 && world.eventComplete == 1) // Axel finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Axel");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "Axel");
                    data.WorldsData[world.worldName].progress = 5;
                }
            }
            else if (world.worldName == "TwilightTown")
            {
                if (world.roomNumber == 27 && world.eventID3 == 4) // Yen Sid after new clothes
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "MysteriousTower");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "MysteriousTower");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 80 && world.eventComplete == 1) // Sandlot finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "Sandlot");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "Sandlot");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 41 && world.eventID1 == 186 && world.eventComplete == 1) // Mansion fight finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "Mansion");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "Mansion");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 40 && world.eventID1 == 161 && world.eventComplete == 1) // Betwixt and Between finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "BetwixtandBetween");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "BetwixtandBetween");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 213 && world.eventComplete == 1) // Data Axel finish
                {
                    broadcast.TwilightTownProgression.SetResourceReference(ContentProperty, "DataAxel");
                    TwilightTownProgression.SetResourceReference(ContentProperty, "DataAxel");
                    data.WorldsData[world.worldName].progress = 5;
                }
            }
            else if (world.worldName == "HollowBastion")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Villain's Vale (HB1)
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 8 && world.eventID1 == 52 && world.eventComplete == 1) // Bailey finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Bailey");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Bailey");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 10 && world.eventID3 == 2 && data.WorldsData[world.worldName].progress == 0) // Marketplace (HB2)
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 5 && world.eventID3 == 20) // Ansem Study post Computer
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "AnsemStudy");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "AnsemStudy");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 86 && world.eventComplete == 1) // Corridor finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Corridor");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Corridor");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 18 && world.eventID1 == 73 && world.eventComplete == 1) // Dancers finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Dancers");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Dancers");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // HB Demyx finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "HBDemyx");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "HBDemyx");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 16 && world.eventID1 == 65 && world.eventComplete == 1) // FF Cloud finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "FinalFantasy");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "FinalFantasy");
                    data.WorldsData[world.worldName].progress = 7;
                }
                else if (world.roomNumber == 17 && world.eventID1 == 66 && world.eventComplete == 1) // 1k Heartless finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "1000Heartless");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "1000Heartless");
                    data.WorldsData[world.worldName].progress = 8;
                }
                else if (world.roomNumber == 1 && world.eventID1 == 75 && world.eventComplete == 1) // Sephiroth finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "Sephiroth");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "Sephiroth");
                    data.WorldsData[world.worldName].progress = 9;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 114 && world.eventComplete == 1) // Data Demyx finish
                {
                    broadcast.HollowBastionProgression.SetResourceReference(ContentProperty, "DataDemyx");
                    HollowBastionProgression.SetResourceReference(ContentProperty, "DataDemyx");
                    data.WorldsData[world.worldName].progress = 10;
                }
            }
            else if (world.worldName == "BeastsCastle")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Entrance Hall (BC1)
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 11 && world.eventID1 == 72 && world.eventComplete == 1) // Thresholder finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Thresholder");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Thresholder");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 69 && world.eventComplete == 1) // Beast finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Beast");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Beast");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 79 && world.eventComplete == 1) // Dark Thorn finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "DarkThorn");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "DarkThorn");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 2 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Belle's Room (BC2)
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "BCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 74 && world.eventComplete == 1) // Dragoons finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Dragoons");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Dragoons");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 82 && world.eventComplete == 1) // Xaldin finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "Xaldin");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "Xaldin");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 97 && world.eventComplete == 1) // Data Xaldin finish
                {
                    broadcast.BeastsCastleProgression.SetResourceReference(ContentProperty, "DataXaldin");
                    BeastsCastleProgression.SetResourceReference(ContentProperty, "DataXaldin");
                    data.WorldsData[world.worldName].progress = 7;
                }
            }
            else if (world.worldName == "OlympusColiseum")
            {
                if (world.roomNumber == 0 & world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // The Coliseum (OC1)
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 114 && world.eventComplete == 1) // Cerberus finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Cerberus");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Cerberus");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 17 && world.eventID1 == 123 && world.eventComplete == 1) // OC Demyx finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCDemyx");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCDemyx");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 8 && world.eventID1 == 116 && world.eventComplete == 1) // OC Pete finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCPete");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCPete");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 18 && world.eventID1 == 171 && world.eventComplete == 1) // Hydra finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hydra");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hydra");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 3 & world.eventID3 == 12 && data.WorldsData[world.worldName].progress == 0) // Underworld Entrance (OC2)
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "OCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 6 && world.eventID1 == 126 && world.eventComplete == 1) // Auron Statue fight finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "AuronStatue");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "AuronStatue");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 19 && world.eventID1 == 202 && world.eventComplete == 1) // Hades finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hades");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Hades");
                    data.WorldsData[world.worldName].progress = 7;
                }
                else if (world.roomNumber == 34 && (world.eventID1 == 151 || world.eventID1 == 152) && world.eventComplete == 1) // Zexion finish
                {
                    broadcast.OlympusColiseumProgression.SetResourceReference(ContentProperty, "Zexion");
                    OlympusColiseumProgression.SetResourceReference(ContentProperty, "Zexion");
                    data.WorldsData[world.worldName].progress = 8;
                }
            }
            else if (world.worldName == "Agrabah")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Agrabah (Ag1)
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 2 && world.eventComplete == 1) // Abu finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Abu");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Abu");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 13 && world.eventID1 == 79 && world.eventComplete == 1) // Chasm fight finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Chasm");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Chasm");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 58 && world.eventComplete == 1) // Treasure Room finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "TreasureRoom");
                    AgrabahProgression.SetResourceReference(ContentProperty, "TreasureRoom");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 59 && world.eventComplete == 1) // Lords finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Lords");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Lords");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 4 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // The Vault (Ag2)
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    AgrabahProgression.SetResourceReference(ContentProperty, "AGChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 100 && world.eventComplete == 1) // Carpet finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Carpet");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Carpet");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 62 && world.eventComplete == 1) // Genie Jafar finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "GenieJafar");
                    AgrabahProgression.SetResourceReference(ContentProperty, "GenieJafar");
                    data.WorldsData[world.worldName].progress = 7;
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 142 || world.eventID1 == 147) && world.eventComplete == 1) // Lexaeus finish
                {
                    broadcast.AgrabahProgression.SetResourceReference(ContentProperty, "Lexaeus");
                    AgrabahProgression.SetResourceReference(ContentProperty, "Lexaeus");
                    data.WorldsData[world.worldName].progress = 8;
                }
            }
            else if (world.worldName == "LandofDragons")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Bamboo Grove (LoD1)
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 5 && world.eventID1 == 72 && world.eventComplete == 1) // Cave finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "Cave");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "Cave");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 73 && world.eventComplete == 1) // Summit finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "Summit");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "Summit");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 75 && world.eventComplete == 1) // Shan Yu finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "ShanYu");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "ShanYu");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 12 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Village (LoD2)
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "LoDChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 78 && world.eventComplete == 1) // Antechamber fight finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "ThroneRoom");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "ThroneRoom");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 8 && world.eventID1 == 79 && world.eventComplete == 1) // Storm Rider finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "StormRider");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "StormRider");
                    data.WorldsData[world.worldName].progress = 6;
                }
            }
            else if (world.worldName == "HundredAcreWood")
            {
                if (world.roomNumber == 2 && (world.eventID3 == 1 || world.eventID3 == 22)) // Pooh's house (eventID3 == 1 is when not skipping AW0)
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Pooh");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Pooh");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 4 && world.eventID3 == 1) // Piglet's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Piglet");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Piglet");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 3 && world.eventID3 == 1) // Rabbit's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Rabbit");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Rabbit");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 5 && world.eventID3 == 1) // Kanga's house
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Kanga");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "Kanga");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 9 && world.eventID3 == 1) // Spooky Cave
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "SpookyCave");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "SpookyCave");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 1 && world.eventID3 == 1) // Starry Hill
                {
                    broadcast.HundredAcreWoodProgression.SetResourceReference(ContentProperty, "StarryHill");
                    HundredAcreWoodProgression.SetResourceReference(ContentProperty, "StarryHill");
                    data.WorldsData[world.worldName].progress = 6;
                }
            }
            else if (world.worldName == "PrideLands")
            {
                if (world.roomNumber == 16 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Wildebeest Valley (PL1)
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 12 && world.eventID3 == 1) // Oasis after talking to Simba
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Simba");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Simba");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 55 && world.eventComplete == 1) // Scar finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Scar");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Scar");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 4 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Savannah (PL2)
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "PLChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 59 && world.eventComplete == 1) // Groundshaker finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "Groundshaker");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "Groundshaker");
                    data.WorldsData[world.worldName].progress = 4;
                }
            }
            else if (world.worldName == "Atlantica")
            {
                if (world.roomNumber == 2 && world.eventID1 == 63) // Tutorial
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "Tutorial");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 65) // Ursula's Revenge
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "Ursula");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55) // A New Day is Dawning
                {
                    AtlanticaProgression.SetResourceReference(ContentProperty, "NewDay");
                    data.WorldsData[world.worldName].progress = 3;
                }
            }
            else if (world.worldName == "DisneyCastle")
            {
                if (world.roomNumber == 1 && world.eventID1 == 53 && data.WorldsData[world.worldName].progress == 0) // Library (DC)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 0 && world.eventID1 == 51 && world.eventComplete == 1) // Minnie Escort finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Minnie");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Minnie");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 0 && world.eventID3 == 22 && data.WorldsData[world.worldName].progress == 0) // Cornerstone Hill (TR) (Audience Chamber has no Evt 0x16)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 1 && world.eventID1 == 58 && world.eventComplete == 1) // Old Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "OldPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "OldPete");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 0 && world.eventID3 == 6) // Windows popup (Audience Chamber has no Evt 0x06)
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Windows");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Windows");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 2 && world.eventID1 == 52 && world.eventComplete == 1) // Boat Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "BoatPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "BoatPete");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 53 && world.eventComplete == 1) // DC Pete finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "DCPete");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "DCPete");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 38 && (world.eventID1 == 145 || world.eventID1 == 150) && world.eventComplete == 1) // Marluxia finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "Marluxia");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "Marluxia");
                    data.WorldsData[world.worldName].progress = 7;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 67 && world.eventComplete == 1) // Lingering Will finish
                {
                    broadcast.DisneyCastleProgression.SetResourceReference(ContentProperty, "LingeringWill");
                    DisneyCastleProgression.SetResourceReference(ContentProperty, "LingeringWill");
                    data.WorldsData[world.worldName].progress = 8;
                }
            }
            else if (world.worldName == "HalloweenTown")
            {
                if (world.roomNumber == 4 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Hinterlands (HT1)
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 6 && world.eventID1 == 53 && world.eventComplete == 1) // Candy Cane Lane fight finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "CandyCaneLane");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "CandyCaneLane");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 52 && world.eventComplete == 1) // Prison Keeper finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "PrisonKeeper");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "PrisonKeeper");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 55 && world.eventComplete == 1) // Oogie Boogie finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "OogieBoogie");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "OogieBoogie");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 1 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Dr. Finklestein's Lab (HT2)
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "HTChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 63 && world.eventComplete == 1) // Presents minigame
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Presents");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Presents");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 64 && world.eventComplete == 1) // Experiment finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Experiment");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Experiment");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 32 && (world.eventID1 == 115 || world.eventID1 == 146) && world.eventComplete == 1) // Vexen finish
                {
                    broadcast.HalloweenTownProgression.SetResourceReference(ContentProperty, "Vexen");
                    HalloweenTownProgression.SetResourceReference(ContentProperty, "Vexen");
                    data.WorldsData[world.worldName].progress = 7;
                }
            }
            else if (world.worldName == "PortRoyal")
            {
                if (world.roomNumber == 0 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Rampart (PR1)
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 2 && world.eventID1 == 55 && world.eventComplete == 1) // Town finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Town");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Town");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 60 && world.eventComplete == 1) // Barbossa finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Barbossa");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Barbossa");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 10 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Treasure Heap (PR2)
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "PRChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 62 && world.eventComplete == 1) // Gambler finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "Gambler");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "Gambler");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 1 && world.eventID1 == 54 && world.eventComplete == 1) // Grim Reaper finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "GrimReaper");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "GrimReaper");
                    data.WorldsData[world.worldName].progress = 5;
                }
            }
            else if (world.worldName == "SpaceParanoids")
            {
                if (world.roomNumber == 1 && world.eventID3 == 1 && data.WorldsData[world.worldName].progress == 0) // Canyon (SP1)
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 3 && world.eventID1 == 54 && world.eventComplete == 1) // Screens finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Screens");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Screens");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 4 && world.eventID1 == 55 && world.eventComplete == 1) // Hostile Program finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "HostileProgram");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "HostileProgram");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 1 && world.eventID3 == 10 && data.WorldsData[world.worldName].progress == 0) // Canyon (SP2)
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SPChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 7 && world.eventID1 == 57 && world.eventComplete == 1) // Solar Sailer finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SolarSailer");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "SolarSailer");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 9 && world.eventID1 == 59 && world.eventComplete == 1) // MCP finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "MCP");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "MCP");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 33 && (world.eventID1 == 143 || world.eventID1 == 148) && world.eventComplete == 1) // Larxene finish
                {
                    broadcast.SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Larxene");
                    SpaceParanoidsProgression.SetResourceReference(ContentProperty, "Larxene");
                    data.WorldsData[world.worldName].progress = 6;
                }
            }
            else if (world.worldName == "TWTNW")
            {
                if (world.roomNumber == 1 && world.eventID3 == 1) // Alley to Between
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "TWTNWChests");
                    TWTNWProgression.SetResourceReference(ContentProperty, "TWTNWChests");
                    data.WorldsData[world.worldName].progress = 1;
                }
                else if (world.roomNumber == 21 && world.eventID1 == 65 && world.eventComplete == 1) // Roxas finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Roxas");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Roxas");
                    data.WorldsData[world.worldName].progress = 2;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 57 && world.eventComplete == 1) // Xigbar finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Xigbar");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Xigbar");
                    data.WorldsData[world.worldName].progress = 3;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 58 && world.eventComplete == 1) // Luxord finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Luxord");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Luxord");
                    data.WorldsData[world.worldName].progress = 4;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 56 && world.eventComplete == 1) // Saix finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Saix");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Saix");
                    data.WorldsData[world.worldName].progress = 5;
                }
                else if (world.roomNumber == 19 && world.eventID1 == 59 && world.eventComplete == 1) // Xemnas 1 finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "Xemnas1");
                    TWTNWProgression.SetResourceReference(ContentProperty, "Xemnas1");
                    data.WorldsData[world.worldName].progress = 6;
                }
                else if (world.roomNumber == 20 && world.eventID1 == 98 && world.eventComplete == 1) // Data Xemnas finish
                {
                    broadcast.TWTNWProgression.SetResourceReference(ContentProperty, "DataXemnas");
                    TWTNWProgression.SetResourceReference(ContentProperty, "DataXemnas");
                    data.WorldsData[world.worldName].progress = 7;
                }

                // Handle data fights
                else if (world.roomNumber == 21 && world.eventID1 == 99 && world.eventComplete == 1) // Data Roxas finish
                {
                    broadcast.SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "DataRoxas");
                    SimulatedTwilightTownProgression.SetResourceReference(ContentProperty, "DataRoxas");
                    data.WorldsData["SimulatedTwilightTown"].progress = 3;
                }
                else if (world.roomNumber == 10 && world.eventID1 == 100 && world.eventComplete == 1) // Data Xigbar finish
                {
                    broadcast.LandofDragonsProgression.SetResourceReference(ContentProperty, "DataXigbar");
                    LandofDragonsProgression.SetResourceReference(ContentProperty, "DataXigbar");
                    data.WorldsData["LandofDragons"].progress = 7;
                }
                else if (world.roomNumber == 15 && world.eventID1 == 102 && world.eventComplete == 1) // Data Saix finish
                {
                    broadcast.PrideLandsProgression.SetResourceReference(ContentProperty, "DataSaix");
                    PrideLandsProgression.SetResourceReference(ContentProperty, "DataSaix");
                    data.WorldsData["PrideLands"].progress = 5;
                }
                else if (world.roomNumber == 14 && world.eventID1 == 101 && world.eventComplete == 1) // Data Luxord finish
                {
                    broadcast.PortRoyalProgression.SetResourceReference(ContentProperty, "DataLuxord");
                    PortRoyalProgression.SetResourceReference(ContentProperty, "DataLuxord");
                    data.WorldsData["PortRoyal"].progress = 6;
                }
            }
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
    }
}
