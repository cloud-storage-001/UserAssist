using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace UserAssist
{
    /// <summary>
    /// The key used by the dictionary
    /// </summary>
    public struct UserAssistKey
    {
        public string key;
        public int index;
    }

    /// <summary>
    /// Class used to access and manipulate the UserAssist registry entries
    /// </summary>
    public class UserAssistEntries
    {
        /// <summary>
        /// registry keys used by Windows Explorer for UserAssist data
        /// </summary>
        private const string regKeyUserAssist = @"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist";
        private const string regKeyExplorerAdvanced = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string regKeyFormat = regKeyUserAssist + @"\{0}\Count";
        private Dictionary<string, BinaryDataFormat> regKeys;
        private const string regKeySettings = @"Settings";
        private const string regKeyNoLog = @"NoLog";
        private const string regFileKeyFormat = @"Microsoft\Windows\CurrentVersion\Explorer\UserAssist\{0}\Count]";
        private const string regKeyStartTrackProgs = @"Start_TrackProgs";

        /// <summary>
        /// Types of binary data format: before Windows 7, Windows 7, ...
        /// </summary>
        public enum BinaryDataFormat
        {
            windows2000ThruVista,
            windows7
        }

        /// <summary>
        /// Dictionary with all the UserAssist entries
        /// </summary>
        public Dictionary<UserAssistKey, UserAssistEntry> entries;

        /// <summary>
        /// BinaryDataFormat of entries
        /// </summary>
        public BinaryDataFormat binaryDataFormat;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserAssistEntries()
        {
            regKeys = new Dictionary<string, BinaryDataFormat>();
            regKeys.Add(@"{0D6D4F41-2994-4BA0-8FEF-620E43CD2812}", BinaryDataFormat.windows2000ThruVista);
            regKeys.Add(@"{5E6AB780-7743-11CF-A12B-00AA004AE837}", BinaryDataFormat.windows2000ThruVista);
            regKeys.Add(@"{75048700-EF1F-11D0-9888-006097DEACF9}", BinaryDataFormat.windows2000ThruVista);
            regKeys.Add(@"{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}", BinaryDataFormat.windows7);
            regKeys.Add(@"{F4E57C4B-2036-45F0-A9AB-443BCFE33D9F}", BinaryDataFormat.windows7);

            entries = new Dictionary<UserAssistKey, UserAssistEntry>();
        }

        /// <summary>
        /// Get the UserAssist entries from the registry and put them in the entries dictionary
        /// </summary>
        public void GetFromRegistry()
        {
            entries.Clear();
            foreach (string regKey in regKeys.Keys)
                GetFromRegistryCount(regKey);
            GetEntriesBinaryDataFormat();
        }

        /// <summary>
        /// Read all the UserAssist entries under the given Count ssubkey,
        /// create a UserAssistEntry object for each entry and store it in the entries dictionary
        /// </summary>
        /// <param name="regKeyCount">the Count subkey</param>
        private void GetFromRegistryCount(string regKeyCount)
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkUserAssist = null;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168

            try
            {
                rkUserAssist = rkHKCU.OpenSubKey(String.Format(regKeyFormat, regKeyCount));
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }

            if (rkUserAssist == null)
            {
                rkHKCU.Close();
                return;
            }

            // retrieve a value from the registry, create a UserAssistEntry and store it in dictionary
            // entries with key UAK
            UserAssistKey UAK;
            UAK.key = regKeyCount;
            UAK.index = 0;
            foreach (string valueName in rkUserAssist.GetValueNames())
            {
                Byte[] bytes = new byte[] { };
                Object valueData = rkUserAssist.GetValue(valueName);
                if (valueData != null && valueData.GetType() == bytes.GetType())
                    bytes = (byte[])valueData;
                entries.Add(UAK, new UserAssistEntry(valueName, bytes));
                UAK.index++;
            }

            rkUserAssist.Close();
            rkHKCU.Close();
        }

        /// <summary>
        /// State machine states used when parsing a REG file
        /// </summary>
        private enum StateMachine
        {
            start,
            key,
            fullEntry,
            partialEntry,
            empty
        }

        /// <summary>
        /// Parse the content of a REG file and store the UserAssist entries in the entries dictionary
        /// </summary>
        /// <param name="registry">content of the REG file</param>
        public void GetFromStrings(string[] registry)
        {
            // Initialize
            StateMachine state = StateMachine.start;
            UserAssistKey UAK;
            string key;
            UAK.key = "";
            UAK.index = 0;
            entries.Clear();
            StringBuilder sb = new StringBuilder();
            Regex re = new Regex("^\"(.+)\"=hex:(.+)$");

            // Parse each string and create UserAssistEntry objects
            foreach (string line in registry)
            {
                // look for a UserAssist count key, but don't match the complete line, just a part, 
                // so that UserAssist entries stored with another path (when loading a Hive and exporting it)
                // are also parsed
                key = MatchUserAssistKey(line);
                if (key != "")
                {
                    state = StateMachine.key;
                    UAK.key = key;
                    UAK.index = 0;
                }
                // when we've found a UserAssist Count key, we will read all values until the end of the file or
                // an empty line
                else if (state == StateMachine.key)
                {
                    // the values can take several lines, each line of a partial entry ends with \, except the last one
                    // we build the complete entry in sb
                    sb = new StringBuilder(line);
                    if (line.EndsWith(@"\"))
                    {
                        sb.Remove(sb.Length - 1, 1);
                        state = StateMachine.partialEntry;
                    }
                    else
                        state = StateMachine.fullEntry;
                }
                else if (state == StateMachine.partialEntry)
                {
                    sb.Append(line);
                    if (line.EndsWith(@"\"))
                    {
                        sb.Remove(sb.Length - 1, 1);
                        state = StateMachine.partialEntry;
                    }
                    else
                        state = StateMachine.fullEntry;
                }

                // parse full entries and store them in a UserAssistEntry object
                if (state == StateMachine.fullEntry)
                {
                    string fullLine = sb.ToString();
                    if (fullLine == "")
                        state = StateMachine.empty;
                    else
                    {
                        // entries are parsed with a regular expression, format is "ValueName"=hex: XX, XX, XX ..., XX
                        GroupCollection gc = re.Match(fullLine).Groups;
                        if (gc.Count == 3)
                        {
                            string valueName = gc[1].Value;
                            byte[] bytes;
                            try
                            {
                                string[] hexDump = gc[2].Value.Replace(" ", "").Split(new string[] { "," }, StringSplitOptions.None);
                                bytes = new byte[hexDump.GetLength(0)];
                                for (int iter = 0; iter < hexDump.GetLength(0); iter++)
                                    bytes[iter] = Convert.ToByte(hexDump[iter], 16);
                            }
#pragma warning disable 0168
                            catch (Exception e)
                            {
                                bytes = new byte[] {};
                            }
#pragma warning restore 0168
                            entries.Add(UAK, new UserAssistEntry(valueName.Replace(@"\\", @"\"), bytes));
                            UAK.index++;
                        }
                        state = StateMachine.key;
                    }
                }
            }
            GetEntriesBinaryDataFormat();
        }

        private string MatchUserAssistKey(string line)
        {
            foreach (string regKey in regKeys.Keys)
                if (line.EndsWith(string.Format(regFileKeyFormat, regKey), StringComparison.InvariantCultureIgnoreCase))
                    return regKey;
            return "";
        }

        /// <summary>
        /// Delete all the UserAssist Count registry keys
        /// </summary>
        public void Clear()
        {
            entries.Clear();
            foreach (string regKey in regKeys.Keys)
                DeleteKey(string.Format(regKeyFormat, regKey));
        }

        /// <summary>
        /// Delete the key specified by regKey
        /// </summary>
        /// <param name="regKey">the registry key to be deleted</param>
        private void DeleteKey(string regKey)
        {
            RegistryKey rkHKCU;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168

            try
            {
                rkHKCU.DeleteSubKey(regKey);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }
        }

        /// <summary>
        /// Delete the UserAssist registry key specified by regKey (regKey1 or regKey2) and regKeyValue
        /// </summary>
        /// <param name="regKey">regKey1 or regKey2</param>
        /// <param name="regKeyValue">the value to be deleted</param>
        /// <returns>returns true when successful</returns>
        public bool ClearValue(string regKey, string regKeyValue)
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkUserAssist = null;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168

            try
            {
                rkUserAssist = rkHKCU.OpenSubKey(String.Format(regKeyFormat, regKey), true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }

            try
            {
                rkUserAssist.DeleteValue(regKeyValue);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkUserAssist.Close();
                rkHKCU.Close();
            }

            return true;
        }

        /// <summary>
        /// Configure UserAssist logging, independently of the OS
        /// </summary>
        /// <param name="value">true -> NoLog =1, false -> delete NoLog</param>
        /// <returns>true when successful</returns>
        public bool ConfigureLogging(bool value)
        {
            if (MyLibrary.IsWindowsXP() || MyLibrary.IsWindows2003())
                return ConfigureLoggingXP2003(value);
            else if (MyLibrary.IsWindowsVista())
                return ConfigureLoggingVista(value);
            else
                return false;
        }

        /// <summary>
        /// Set the UserAssist/Settings/NoLog value to 1 or delete it, according to the value
        /// Setting NoLog to 0 doesn't seem to allow logging, that's why we delete it
        /// </summary>
        /// <param name="value">true -> NoLog =1, false -> delete NoLog</param>
        /// <returns>true when successful</returns>
        public bool ConfigureLoggingXP2003(bool value)
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkUserAssist = null;
            RegistryKey rkSettings = null;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168

            try
            {
                rkUserAssist = rkHKCU.OpenSubKey(regKeyUserAssist, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }

            // try to open the Settings subkey, create it if it doesn't exist
            try
            {
                rkSettings = rkUserAssist.OpenSubKey(regKeySettings, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
            }
#pragma warning restore 0168

            if (rkSettings == null)
                try
                {
                    rkSettings = rkUserAssist.CreateSubKey(regKeySettings);
                }
#pragma warning disable 0168
                catch (Exception e)
                {
                    rkUserAssist.Close();
                    rkHKCU.Close();
                    return false;
                }
#pragma warning restore 0168

            try
            {
                if (value)
                    rkSettings.SetValue(regKeyNoLog, "1");
                else
                    rkSettings.DeleteValue(regKeyNoLog);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkSettings.Close();
                rkUserAssist.Close();
                rkHKCU.Close();
            }
            return true;
        }

        /// <summary>
        /// Set the UserAssist/Settings/NoLog value to 1 or delete it, according to the value
        /// Setting NoLog to 0 doesn't seem to allow logging, that's why we delete it
        /// </summary>
        /// <param name="value">true -> NoLog =1, false -> delete NoLog</param>
        /// <returns>true when successful</returns>
        public bool ConfigureLoggingVista(bool value)
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkExplorerAdvanced = null;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168

            try
            {
                rkExplorerAdvanced = rkHKCU.OpenSubKey(regKeyExplorerAdvanced, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }

            try
            {
                if (value)
                    rkExplorerAdvanced.SetValue(regKeyStartTrackProgs, 0, RegistryValueKind.DWord);
                else
                    rkExplorerAdvanced.SetValue(regKeyStartTrackProgs, 1, RegistryValueKind.DWord);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkExplorerAdvanced.Close();
                rkHKCU.Close();
            }
            return true;
        }

        /// <summary>
        /// Read the value of UserAssist logging switch independently of the OS version
        /// </summary>
        /// <returns>returns true only when UserAssist logging enabled</returns>
        public bool GetLogging()
        {
            if (MyLibrary.IsWindowsXP() || MyLibrary.IsWindows2003())
                return GetLoggingXP2003();
            else if (MyLibrary.IsWindowsVista())
                return GetLoggingVista();
            else
                return false;
        }

        /// <summary>
        /// Read the value of UserAssist/Settings/NoLog
        /// </summary>
        /// <returns>returns true only when UserAssist/Settings/NoLog equals "1"</returns>
        private bool GetLoggingXP2003()
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkUserAssist = null;
            RegistryKey rkSettings = null;
            bool result;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168

            try
            {
                rkUserAssist = rkHKCU.OpenSubKey(regKeyUserAssist, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                rkHKCU.Close();
                return false;
            }
#pragma warning restore 0168

            if (rkUserAssist == null)
            {
                rkHKCU.Close();
                return false;
            }

            try
            {
                rkSettings = rkUserAssist.OpenSubKey(regKeySettings, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                rkUserAssist.Close();
                rkHKCU.Close();
                return false;
            }
#pragma warning restore 0168

            try
            {
                result = rkSettings.GetValue(regKeyNoLog).ToString() == "1" ? true : false;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                if (rkSettings != null)
                    rkSettings.Close();
                rkUserAssist.Close();
                rkHKCU.Close();
            }
            return result;
        }

        /// <summary>
        /// Read the value of HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\Start_TrackProgs
        /// </summary>
        /// <returns>returns true only when Start_TrackProgs = 0</returns>
        private bool GetLoggingVista()
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkExplorerAdvanced = null;
            bool result;

            try
            {
                rkHKCU = Registry.CurrentUser;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168

            try
            {
                rkExplorerAdvanced = rkHKCU.OpenSubKey(regKeyExplorerAdvanced, true);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                rkHKCU.Close();
                return false;
            }
#pragma warning restore 0168

            if (rkExplorerAdvanced == null)
            {
                rkHKCU.Close();
                return false;
            }

            try
            {
                result = rkExplorerAdvanced.GetValue(regKeyStartTrackProgs).ToString() == "1" ? false : true;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
            finally
            {
                rkExplorerAdvanced.Close();
                rkHKCU.Close();
            }
            return result;
        }

        /// <summary>
        /// Get the UserAssist entries from a NTUSER.DAT file loaded in the registry and put them in the entries dictionary
        /// </summary>
        public void GetFromLoadedHive(string file)
        {
            entries.Clear();
            string loadedHiveKey = RegHive.Load(file);
            foreach (string regKey in regKeys.Keys)
                GetFromLoadedHiveCount(loadedHiveKey, regKey);
            RegHive.Unload();
            GetEntriesBinaryDataFormat();
        }

        /// <summary>
        /// Read all the UserAssist entries under the given Count ssubkey,
        /// create a UserAssistEntry object for each entry and store it in the entries dictionary
        /// </summary>
        /// <param name="regKeyCount">the Count subkey</param>
        private void GetFromLoadedHiveCount(string loadedHiveKey, string regKeyCount)
        {
            RegistryKey rkHKCU = null;
            RegistryKey rkUserAssist = null;

            try
            {
                rkHKCU = Registry.Users;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168

            try
            {
                rkUserAssist = rkHKCU.OpenSubKey(loadedHiveKey + @"\" + String.Format(regKeyFormat, regKeyCount));
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168
            finally
            {
                rkHKCU.Close();
            }

            if (rkUserAssist == null)
            {
                rkHKCU.Close();
                return;
            }

            // retrieve a value from the registry, create a UserAssistEntry and store it in dictionary
            // entries with key UAK
            UserAssistKey UAK;
            UAK.key = regKeyCount;
            UAK.index = 0;
            foreach (string valueName in rkUserAssist.GetValueNames())
            {
                Byte[] bytes = new byte[] { };
                Object valueData = rkUserAssist.GetValue(valueName);
                if (valueData != null && valueData.GetType() == bytes.GetType())
                    bytes = (byte[])valueData;
                entries.Add(UAK, new UserAssistEntry(valueName, bytes));
                UAK.index++;
            }

            rkUserAssist.Close();
            rkHKCU.Close();
        }

        private void GetEntriesBinaryDataFormat()
        {
            foreach (UserAssistKey key in entries.Keys)
                binaryDataFormat = regKeys[key.key];
        }
    }

    /// <summary>
    /// Retrieve and decode the binary data of the UserAssist registry key specified by regKey (regKey1 or regKey2) and regKeyValue (ROT13 decrypted)
    /// When the binary data is 8 bytes long, it contains the following data:
    /// 4 bytes (unknown): purpose unknown
    /// 4 bytes (session): session ID, is sometimes increased with 1 when Windows Explorer is restared
    /// When the binary data is 16 bytes long, it contains the following data:
    /// 4 bytes (session): session ID, set to the session ID of the UEME_CTLSESSION entry
    /// 4 bytes (count):   counts the number of times a program was executed, this value is set to 6 the first time a program is executed
    /// 8 bytes (last):    timestamp when the program was last executed, sometimes equal to 0,
    ///                    when it's equal to 0, the count can be less than 6
    /// </summary>
    public class UserAssistEntry
    {
        private string _name;
        private byte[] _data;
        private int? _session;
        private int? _count;
        private DateTime? _last;
        private DateTime? _lastutc;
        private int? _unknown;
        private int? _countAll;
        private int? _totalRunningTime;
        private int? _flags;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">value name of the entry</param>
        /// <param name="data">binary data of the entry</param>
        public UserAssistEntry(string name, byte[] data)
        {
            this._name = name;
            this._data = data;

            BinaryReader br = new BinaryReader(new MemoryStream((byte[])data));;

            switch (data.Length)
            {
                case 8:
                    try
                    {
                        _unknown = br.ReadInt32();
                        _session = br.ReadInt32();
                    }
#pragma warning disable 0168
                    catch (Exception e)
                    {
                    }
#pragma warning restore 0168
                    finally
                    {
                        br.Close();
                    }
                    break;

                case 16:
                    try
                    {
                        _session = br.ReadInt32();
                        _count = br.ReadInt32();
                        Int64 last = br.ReadInt64();
                        if (last != 0)
                        {
                            // doesn't take into account possible timezone differencies
                            _last = DateTime.FromFileTime(last);
                            _lastutc = DateTime.FromFileTimeUtc(last);
                            _count -= 5;
                        }
                    }
#pragma warning disable 0168
                    catch (Exception e)
                    {
                    }
#pragma warning restore 0168
                    finally
                    {
                        br.Close();
                    }
                    break;

                case 72:
                    try
                    {
                        int iDummy;

                        iDummy = br.ReadInt32();
                        _count = br.ReadInt32();
                        _countAll = br.ReadInt32();
                        _totalRunningTime = br.ReadInt32();
                        for (int iIter = 0; iIter < 11; iIter++)
                            iDummy = br.ReadInt32();
                        Int64 last = br.ReadInt64();
                        if (last != 0)
                        {
                            // doesn't take into account possible timezone differencies
                            _last = DateTime.FromFileTime(last);
                            _lastutc = DateTime.FromFileTimeUtc(last);
                        }
                        _flags = br.ReadInt32();
                    }
#pragma warning disable 0168
                    catch (Exception e)
                    {
                    }
#pragma warning restore 0168
                    finally
                    {
                        br.Close();
                    }
                    break;
            }
        }

        /// <summary>
        /// encrypted name, no logic foreseen when the name is not ROT13 encrypted
        /// </summary>
        public string name
        {
            get { return this._name; }
        }

        /// decrypted name, logic foreseen when the name is not ROT13 encrypted
        public string ReadableName
        {
            get 
            {
                if (this._name.StartsWith("UEME"))
                    return this._name; 
                else
                    return ROT13(this._name);
            }
        }

        /// explain what is stored in the name property
        public string Explain()
        {
            UserAssistNameAnalysis explain = new UserAssistNameAnalysis(this.ReadableName);
            return explain.explanation;
        }

        public int? count
        {
            get { return this._count; }
        }

        public int? session
        {
            get { return this._session; }
        }

        public DateTime? last
        {
            get { return this._last; }
        }

        public DateTime? lastutc
        {
            get { return this._lastutc; }
        }

        public int? unknown
        {
            get { return this._unknown; }
        }

        public int? countAll
        {
            get { return this._countAll; }
        }

        public int? totalRunningTime
        {
            get { return this._totalRunningTime; }
        }

        public int? flags
        {
            get { return this._flags; }
        }

        /// <summary>
        /// Decrypt/encrypt string cypherText with ROT13
        /// </summary>
        /// <param name="cypherText">cyphertext</param>
        /// <returns>cleartext</returns>
        private string ROT13(string cypherText)
        {
            StringBuilder sb = new StringBuilder();

            CharEnumerator charEnum = cypherText.GetEnumerator();

            while (charEnum.MoveNext())
            {
                char chr = char.ToUpper(charEnum.Current);
                if (chr >= 'A' && chr <= 'M')
                    sb.Append(Convert.ToChar(Convert.ToUInt16(charEnum.Current) + 13));
                else if (chr >= 'N' && chr <= 'Z')
                    sb.Append(Convert.ToChar(Convert.ToUInt16(charEnum.Current) - 13));
                else
                    sb.Append(chr);
            }

            return sb.ToString();
        }
    }

	public class RegHive
	{
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public LUID Luid;
            public int Attributes;
            public int PrivilegeCount;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int tokenhandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeValue(string lpsystemname, string lpname, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, int PreivousState, int Returnlength);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegLoadKey(uint hKey, string lpSubKey, string lpFile);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegUnLoadKey(uint hKey, string lpSubKey);

        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const int TOKEN_QUERY = 0x00000008;
        public const int SE_PRIVILEGE_ENABLED = 0x00000002;
        public const string SE_RESTORE_NAME = "SeRestorePrivilege";
        public const string SE_BACKUP_NAME = "SeBackupPrivilege";
        public const uint HKEY_USERS = 0x80000003;
        public const string HIVE_SUBKEY = "LoadedHive";
        
        static private Boolean gotPrivileges = false;

		static private void GetPrivileges()
		{
			int token = 0;
			int retval = 0;
			TOKEN_PRIVILEGES tpRestore = new TOKEN_PRIVILEGES();
			TOKEN_PRIVILEGES tpBackup = new TOKEN_PRIVILEGES();
			LUID RestoreLuid = new LUID();
			LUID BackupLuid = new LUID();

			retval = LookupPrivilegeValue(null, SE_RESTORE_NAME, ref RestoreLuid);
			tpRestore.PrivilegeCount = 1;
			tpRestore.Attributes = SE_PRIVILEGE_ENABLED;
			tpRestore.Luid = RestoreLuid;

			retval = LookupPrivilegeValue(null, SE_BACKUP_NAME, ref BackupLuid);
			tpBackup.PrivilegeCount = 1;
			tpBackup.Attributes = SE_PRIVILEGE_ENABLED;
			tpBackup.Luid = BackupLuid;

			retval = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
			retval = AdjustTokenPrivileges(token, 0, ref tpRestore, 1024, 0, 0);
			retval = AdjustTokenPrivileges(token, 0, ref tpBackup, 1024, 0, 0);

			gotPrivileges = true;
		}

		static public string Load(string file)
		{
			if (!gotPrivileges)
				GetPrivileges();
			RegLoadKey(HKEY_USERS, HIVE_SUBKEY, file);
			return HIVE_SUBKEY;
		}

		static public void Unload()
		{
			if (!gotPrivileges)
				GetPrivileges();
			RegUnLoadKey(HKEY_USERS, HIVE_SUBKEY);
		}
	}

    /// <summary>
    /// Analyse the name property
    /// </summary>
    public class UserAssistNameAnalysis
    {
        private string _name;
//        private bool _knownEntryType;
//        private bool _explained;
        private string _explanation;

        public enum UEME_TYPES {
                UEME_CTLCUACOUNT,
                UEME_CTLSESSION,
                UEME_DBSLEEP,
                UEME_DBTRACE,
                UEME_DONECANCEL,
                UEME_DONEFAIL,
                UEME_DONEOK,
                UEME_ERROR,
                UEME_INSTRBROWSER,
                UEME_RUNCPL,
                UEME_RUNINVOKE,
                UEME_RUNOLECMD,
                UEME_RUNPATH,
                UEME_RUNPIDL,
                UEME_RUNWMCMD,
                UEME_RUN,
                UEME_UIHOTKEY,
                UEME_UIMENU,
                UEME_UIQCUT,
                UEME_UISCUT,
                UEME_UITOOLBAR,
                UEME_USER,
                UNKNOWN = 0xFF
            };

        // http://source.winehq.org/source/include/shlobj.h
        public enum CSIDL
        {
            CSIDL_DESKTOP = 0x0000,
            CSIDL_INTERNET = 0x0001,
            CSIDL_PROGRAMS = 0x0002,
            CSIDL_CONTROLS = 0x0003,
            CSIDL_PRINTERS = 0x0004,
            CSIDL_PERSONAL = 0x0005,
            CSIDL_FAVORITES = 0x0006,
            CSIDL_STARTUP = 0x0007,
            CSIDL_RECENT = 0x0008,
            CSIDL_SENDTO = 0x0009,
            CSIDL_BITBUCKET = 0x000a,
            CSIDL_STARTMENU = 0x000b,
            CSIDL_MYDOCUMENTS = 0x000c,
            CSIDL_MYMUSIC = 0x000d,
            CSIDL_MYVIDEO = 0x000e,
            CSIDL_DESKTOPDIRECTORY = 0x0010,
            CSIDL_DRIVES = 0x0011,
            CSIDL_NETWORK = 0x0012,
            CSIDL_NETHOOD = 0x0013,
            CSIDL_FONTS = 0x0014,
            CSIDL_TEMPLATES = 0x0015,
            CSIDL_COMMON_STARTMENU = 0x0016,
            CSIDL_COMMON_PROGRAMS = 0X0017,
            CSIDL_COMMON_STARTUP = 0x0018,
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,
            CSIDL_APPDATA = 0x001a,
            CSIDL_PRINTHOOD = 0x001b,
            CSIDL_LOCAL_APPDATA = 0x001c,
            CSIDL_ALTSTARTUP = 0x001d,
            CSIDL_COMMON_ALTSTARTUP = 0x001e,
            CSIDL_COMMON_FAVORITES = 0x001f,
            CSIDL_INTERNET_CACHE = 0x0020,
            CSIDL_COOKIES = 0x0021,
            CSIDL_HISTORY = 0x0022,
            CSIDL_COMMON_APPDATA = 0x0023,
            CSIDL_WINDOWS = 0x0024,
            CSIDL_SYSTEM = 0x0025,
            CSIDL_PROGRAM_FILES = 0x0026,
            CSIDL_MYPICTURES = 0x0027,
            CSIDL_PROFILE = 0x0028,
            CSIDL_SYSTEMX86 = 0x0029,
            CSIDL_PROGRAM_FILESX86 = 0x002a,
            CSIDL_PROGRAM_FILES_COMMON = 0x002b,
            CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,
            CSIDL_COMMON_TEMPLATES = 0x002d,
            CSIDL_COMMON_DOCUMENTS = 0x002e,
            CSIDL_COMMON_ADMINTOOLS = 0x002f,
            CSIDL_ADMINTOOLS = 0x0030,
            CSIDL_CONNECTIONS = 0x0031,
            CSIDL_COMMON_MUSIC = 0x0035,
            CSIDL_COMMON_PICTURES = 0x0036,
            CSIDL_COMMON_VIDEO = 0x0037,
            CSIDL_RESOURCES = 0x0038,
            CSIDL_RESOURCES_LOCALIZED = 0x0039,
            CSIDL_COMMON_OEM_LINKS = 0x003a,
            CSIDL_CDBURN_AREA = 0x003b,
            CSIDL_COMPUTERSNEARME = 0x003d,
            CSIDL_PROFILES = 0x003e
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">value name of the entry</param>
        public UserAssistNameAnalysis(string name)
        {
            this._name = name;
            Analyse();
        }

        /// <summary>
        /// Return the name
        /// </summary>
        public string name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Return the explanation for the name
        /// </summary>
        public string explanation
        {
            get { return this._explanation; }
        }

        /// <summary>
        /// Analyse what is stored in the name property
        /// </summary>
        private void Analyse()
        {
            bool fullMatch;
            UEME_TYPES type;
            string program;
            int csidl;
            string GUID;
            Dictionary<string, string> ShellGUIDs = new Dictionary<string,string>();
            Dictionary<string, string> ButtonIDs = new Dictionary<string, string>();

            string messageUnknown = "该条目的目的未知.";
            string messageUnexpected = "该条目的格式是意外的.";
            string messagePleaseInform = string.Format("请告知Didier Stevens ({1}.{2}@gmail.com).", messageUnknown, "didier", "stevens");
            string messageUnknownPleaseInform = string.Format("{0}\n{1}", messageUnknown, messagePleaseInform);
            string messageUnexpectedPleaseInform = string.Format("{0}\n{1}", messageUnexpected, messagePleaseInform);
            string messagePIDL = "PIDL是指向ITEMIDLIST结构的指针，用于标识Shell名称空间中的对象.\nPIDL的示例是程序开始菜单中的文件夹或快捷方式.\n参考 http://msdn2.microsoft.com/en-us/library/ms538107.aspx.";
            string messageCSIDL = "CSIDL (常数特殊项目ID清单) 值提供了一种独特的与系统无关的方式来标识特殊文件夹.\n参考 http://msdn2.microsoft.com/en-us/library/ms649274.aspx.";

            ShellGUIDs.Add("{208D2C60-3AEA-1069-A2D7-08002B30309D}", "CLSID_NetworkPlaces");
            ShellGUIDs.Add("{46e06680-4bf0-11d1-83ee-00a0c90dc849}", "CLSID_NetworkDomain");
            ShellGUIDs.Add("{c0542a90-4bf0-11d1-83ee-00a0c90dc849}", "CLSID_NetworkServer");
            ShellGUIDs.Add("{54a754c0-4bf1-11d1-83ee-00a0c90dc849}", "CLSID_NetworkShare");
            ShellGUIDs.Add("{20D04FE0-3AEA-1069-A2D8-08002B30309D}", "CLSID_MyComputer");
            ShellGUIDs.Add("{871C5380-42A0-1069-A2EA-08002B30309D}", "CLSID_Internet");
            ShellGUIDs.Add("{F3364BA0-65B9-11CE-A9BA-00AA004AE837}", "CLSID_ShellFSFolder");
            ShellGUIDs.Add("{645FF040-5081-101B-9F08-00AA002F954E}", "CLSID_RecycleBin");
            ShellGUIDs.Add("{21EC2020-3AEA-1069-A2DD-08002B30309D}", "CLSID_ControlPanel");
            ShellGUIDs.Add("{2227A280-3AEA-1069-A2DE-08002B30309D}", "CLSID_Printers");
            ShellGUIDs.Add("{450D8FBA-AD25-11D0-98A8-0800361B1103}", "CLSID_MyDocuments");

            ButtonIDs.Add("0x1,120", "Back");
            ButtonIDs.Add("0x1,121", "Forward");
            ButtonIDs.Add("0x1,130", "Up");
            ButtonIDs.Add("0x1,123", "Search");
            ButtonIDs.Add("0x1,133", "Folders");
            ButtonIDs.Add("0x4,7031", "Views");
            ButtonIDs.Add("0x1,701f", "Move To");
            ButtonIDs.Add("0x4,701f", "Move To");
            ButtonIDs.Add("0x1,701e", "Copy To");
            ButtonIDs.Add("0x4,701e", "Copy To");
            ButtonIDs.Add("0x1,7011", "Delete");
            ButtonIDs.Add("0x4,7011", "Delete");
            ButtonIDs.Add("0x1,701b", "Undo");
            ButtonIDs.Add("0x4,701b", "Undo");

            if (CheckType(out fullMatch, out type, out program, out csidl, out GUID))
            {
                switch (type)
                {
                    case UEME_TYPES.UEME_CTLCUACOUNT:
                        this._explanation = messageUnknown;
                        break;

                    case UEME_TYPES.UEME_DBSLEEP:
                    case UEME_TYPES.UEME_DBTRACE:
                    case UEME_TYPES.UEME_DONECANCEL:
                    case UEME_TYPES.UEME_DONEFAIL:
                    case UEME_TYPES.UEME_DONEOK:
                    case UEME_TYPES.UEME_ERROR:
                    case UEME_TYPES.UEME_INSTRBROWSER:
                    case UEME_TYPES.UEME_RUN:
                    case UEME_TYPES.UEME_RUNINVOKE:
                    case UEME_TYPES.UEME_RUNOLECMD:
                    case UEME_TYPES.UEME_RUNWMCMD:
                    case UEME_TYPES.UEME_UIHOTKEY:
                    case UEME_TYPES.UEME_UIMENU:
                    case UEME_TYPES.UEME_USER:
                        this._explanation = messageUnknownPleaseInform;
                        break;

                    case UEME_TYPES.UEME_CTLSESSION:
                        this._explanation = "该条目用于会话ID，不包含有关已执行程序的数据";
                        break;

                    case UEME_TYPES.UEME_UIQCUT:
                        this._explanation = "计算通过“快速启动”菜单快捷方式启动的程序";
                        if (!fullMatch)
                            this._explanation += "\n" + messageUnexpectedPleaseInform;
                        break;

                    case UEME_TYPES.UEME_UISCUT:
                        this._explanation = "计算通过桌面快捷方式启动的程序";
                        if (!fullMatch)
                            this._explanation += "\n" + messageUnexpectedPleaseInform;
                        break;

                    case UEME_TYPES.UEME_RUNCPL:
                        if (fullMatch)
                            this._explanation = "此项保留有关已执行控制小程序(.cpl)的数据";
                        else
                            this._explanation = "该条目用于执行控制小程序" + program;
                        break;

                    case UEME_TYPES.UEME_RUNPATH:
                        if (fullMatch)
                            this._explanation = "此项保留有关已执行程序的数据";
                        else
                            this._explanation = "该条目用于执行程序 " + program;
                        break;

                    case UEME_TYPES.UEME_RUNPIDL:
                        if (fullMatch)
                            this._explanation = string.Format("此项保留有关已执行的PIDL的数据.\n\n{0}", messagePIDL);
                        else
                        {
                            this._explanation = string.Format("此项用于执行PIDL {0}.", program);
                            if (program.ToLower().EndsWith(".lnk"))
                                this._explanation += "\n这个PIDL看起来像个捷径.";
                            else if (!program.ToLower().Contains("."))
                                this._explanation += "\n该PIDL看起来像一个文件夹。";
                            this._explanation += string.Format("\n通常，UEME_RUNPIDL具有对应的UEME_RUNPATH.\n\n{0}", messagePIDL);
                        }
                        break;

                    case UEME_TYPES.UEME_UITOOLBAR:
                        if (fullMatch)
                            this._explanation = "此项保留有关Windows资源管理器工具栏按钮上的单击的数据";
                        else
                        {
                            this._explanation = "此项用于单击具有ID的Windows资源管理器工具栏按钮 " + program;
                            if (ButtonIDs.ContainsKey(program))
                                this._explanation += string.Format("\n\nID {0} 是 {1} 的按钮.", program, ButtonIDs[program]);
                        }
                        break;

                    default:
                        this._explanation = messageUnknownPleaseInform;
                        break;
                }

                if (GUID != "")
                {
                    if (ShellGUIDs.ContainsKey(GUID))
                        this._explanation += string.Format("\n\n{0} 是已知的Shell GUID，称为 {1}.", GUID, ShellGUIDs[GUID]);
                    else if (LookUpGUIDInCLSID(GUID) != "")
                        this._explanation += string.Format("\n\n{0} 是GUID，在此计算机上，它与CLSID关联 {1}.", GUID, LookUpGUIDInCLSID(GUID));
                    else
                        this._explanation += string.Format("\n\n{0} 是GUID，请在要分析的计算机的注册表中搜索它.", GUID);
                }

                if (csidl >= 0)
                {
                    string specialFolder = "<UNKNOWN>";
                    try
                    {
                        specialFolder = ((CSIDL)csidl).ToString();
                    }
#pragma warning disable 0168
                    catch (Exception e)
                    {
                    }
#pragma warning restore 0168
                    this._explanation += string.Format("\n\n%csidl{0}% 是特殊文件夹 {1}.\n\n{2}", csidl, specialFolder, messageCSIDL);
                }
            }
            else
                this._explanation = messageUnknownPleaseInform;

        }

        /// <summary>
        /// Check the type of the name
        /// </summary>
        private bool CheckType(out bool fullMatch, out UEME_TYPES type, out string program, out int csidl, out string GUID)
        {
            // XP SP2 entries:
            //UEME_CTLCUACount:ctor
            //UEME_CTLSESSION
            //UEME_DBSLEEP
            //UEME_DBTRACE
            //UEME_DONECANCEL
            //UEME_DONEFAIL
            //UEME_DONEOK
            //UEME_ERROR
            //UEME_INSTRBROWSER
            //UEME_RUN
            //UEME_RUNCPL
            //UEME_RUNINVOKE
            //UEME_RUNOLECMD
            //UEME_RUNPATH
            //UEME_RUNPIDL
            //UEME_RUNWMCMD
            //UEME_UIHOTKEY
            //UEME_UIMENU
            //UEME_UIQCUT
            //UEME_UISCUT
            //UEME_UITOOLBAR
            //UEME_USER

            // Vista entries:
            //UEME_RUNPATH
            //UEME_CTLCUACount:ctor
            //UEME_CTLSESSION
            //UEME_RUNPIDL
            //UEME_RUN

            string[] types = {
                "UEME_CTLCUACount:ctor", 
                "UEME_CTLSESSION", 
                "UEME_DBSLEEP", 
                "UEME_DBTRACE", 
                "UEME_DONECANCEL", 
                "UEME_DONEFAIL", 
                "UEME_DONEOK", 
                "UEME_ERROR", 
                "UEME_INSTRBROWSER", 
                "UEME_RUNCPL", 
                "UEME_RUNINVOKE", 
                "UEME_RUNOLECMD", 
                "UEME_RUNPATH", 
                "UEME_RUNPIDL", 
                "UEME_RUNWMCMD", 
                "UEME_RUN", 
                "UEME_UIHOTKEY", 
                "UEME_UIMENU", 
                "UEME_UIQCUT", 
                "UEME_UISCUT", 
                "UEME_UITOOLBAR", 
                "UEME_USER", 
            };

            fullMatch = false;
            type = UEME_TYPES.UNKNOWN;
            program = "";
            csidl = -1;
            GUID = "";

            for (int i = 0; i < types.Length; i++)
                if (this._name.ToUpper().StartsWith(types[i].ToUpper()))
                {
                    type = (UEME_TYPES) i;
                    fullMatch = this._name.ToUpper() == types[i].ToUpper();
                    if (!fullMatch)
                    {
                        program = this._name.Substring(types[i].Length);
                        if (program.StartsWith(":"))
                            program = program.Substring(1);

                        Regex reCSIDL = new Regex("%csidl([0-9]+)%", RegexOptions.IgnoreCase);
                        Match matchCSIDL = reCSIDL.Match(program);
                        if (matchCSIDL.Success)
                            csidl = int.Parse(matchCSIDL.Groups[1].ToString());

                        Regex reGUID = new Regex(@"\{{1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}{1}", RegexOptions.IgnoreCase);
                        Match matchGUID = reGUID.Match(program);
                        if (matchGUID.Success)
                            GUID = matchGUID.Groups[0].ToString().ToUpper();
                    }
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Look up a GUID in the registry, Class IDs
        /// </summary>
        /// <param name="GUID">the GUID to look up</param>
        private string LookUpGUIDInCLSID(string GUID)
        {
            RegistryKey rkHKCR = null;
            RegistryKey rkCLSID = null;

            try
            {
                rkHKCR = Registry.ClassesRoot;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return "";
            }
#pragma warning restore 0168

            try
            {
                rkCLSID = rkHKCR.OpenSubKey(@"CLSID\" + GUID);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return "";
            }
#pragma warning restore 0168
            finally
            {
                rkHKCR.Close();
            }

            if (rkCLSID == null)
                return "";

            string value;
            try
            {
                value = rkCLSID.GetValue("").ToString();
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return "";
            }
#pragma warning restore 0168
            finally
            {
                rkCLSID.Close();
                rkHKCR.Close();
            }
            return value;
        }
    }

    /// <summary>
    /// Collection of low level functions
    /// </summary>
    public class MyLibrary
    {
        public static bool IsWindowsXP()
        {
            return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1;
        }

        public static bool IsWindows2003()
        {
            return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 2;
        }

        public static bool IsWindowsVista()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0;
        }

        public static bool IsWindows2008()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
        }

        public static bool IsWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2;
        }
    }
}
