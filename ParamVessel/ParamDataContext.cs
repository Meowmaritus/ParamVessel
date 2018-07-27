using MeowDSIO;
using MeowDSIO.DataFiles;
using MeowDSIO.DataTypes.PARAM;
using MeowDSIO.DataTypes.PARAMDEF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MeowsBetterParamEditor
{
    public class ParamDataContext : INotifyPropertyChanged
    {
        private bool _isParamRowClipboardValid = false;
        public bool IsParamRowClipboardValid
        {
            get => _isParamRowClipboardValid;
            set
            {
                _isParamRowClipboardValid = value;
                NotifyPropertyChanged(nameof(IsParamRowClipboardValid));
            }
        }

        private UserConfig _config = new UserConfig();
        public UserConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                NotifyPropertyChanged(nameof(Config));
            }
        }

        public void LoadConfig()
        {
            if (File.Exists(UserConfigPath))
            {
                string cfgJson = File.ReadAllText(UserConfigPath);
                Config = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConfig>(cfgJson);
            }
            else
            {
                Config = new UserConfig();
                SaveConfig();
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(UserConfigPath))
            {
                File.Delete(UserConfigPath);
            }
            string cfgJson = Newtonsoft.Json.JsonConvert.SerializeObject(Config, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(UserConfigPath, cfgJson);
        }

        public string UserConfigPath => IOHelper.Frankenpath(Environment.CurrentDirectory, CONFIG_FILE);

        public const string CONFIG_FILE = "ParamVessel_UserConfig.json";

        public const string EXT_PARAM = ".param";
        public const string EXT_PARAMDEF = ".paramdef";

        public static Dictionary<string, string> SpecialInternalParamNameOverrides = new Dictionary<string, string>
        {
            ["BehaviorParam"] = "BEHAVIOR_PARAM_ST (NPC)",
            ["BehaviorParam_PC"] = "BEHAVIOR_PARAM_ST (PC)",
            ["AtkParam_Pc"] = "ATK_PARAM_ST (PC)",
            ["AtkParam_Npc"] = "ATK_PARAM_ST (NPC)",
        };

        public List<BND> PARAMBNDs = new List<BND>();
        public BND PARAMDEFBND = new BND();

        private ObservableCollection<PARAMDEFRef> _paramDefs = new ObservableCollection<PARAMDEFRef>();
        private ObservableCollection<PARAMRef> _params = new ObservableCollection<PARAMRef>();

        public ObservableCollection<PARAMDEFRef> ParamDefs
        {
            get => _paramDefs;
            set
            {
                _paramDefs = value;
                NotifyPropertyChanged(nameof(ParamDefs));
            }
        }

        public ObservableCollection<PARAMRef> Params
        {
            get => _params;
            set
            {
                _params = value;
                NotifyPropertyChanged(nameof(Params));
            }
        }

        public async Task LoadParamsInOtherThread(Action<bool> setIsLoading)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    setIsLoading?.Invoke(true);
                });

                LoadAllPARAMs();

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    Mouse.OverrideCursor = null;
                //    setIsLoading?.Invoke(false);
                //});
            });
        }

        private static readonly Dictionary<string, string> EmbeddedParamDefBndNameMap = new Dictionary<string, string>
        {
            [@"N:\FRPG\DATA\INTERROOT_WIN32\PARAMDEF"] = $"{nameof(MeowsBetterParamEditor)}.EmbeddedResources.paramdef_frpg.paramdefbnd",
            [@"N:\SPRJ\DATA\INTERROOT_PS4\PARAMDEF\64BIT"] = $"{nameof(MeowsBetterParamEditor)}.EmbeddedResources.paramdef_sprj.paramdefbnd",
            [@"N:\FRPG\DATA\INTERROOT_X64\PARAMDEF"] = $"{nameof(MeowsBetterParamEditor)}.EmbeddedResources.paramdef_frpg_x64.paramdefbnd"
        };

        private void LoadEmbeddedParamDefBnd(string internalParamDefFolder)
        {
            string embeddedParamDefBndName 
                = EmbeddedParamDefBndNameMap[internalParamDefFolder.Trim('\\', '/').ToUpper()];
            using (var stream = typeof(ParamDataContext).Assembly
                .GetManifestResourceStream(embeddedParamDefBndName))
            {
                PARAMDEFBND = DataFile.LoadFromStream<BND>(stream, @"paramdef.paramdefbnd");
            }
        }

        private string CheckInternalParamDefDirectory()
        {
#if REMASTER
            var gameParamDefBnd = DataFile.LoadFromDcxFile<BND>(Config.ParamDefBndPath);
#else
            var gameParamDefBnd = DataFile.LoadFromFile<BND>(Config.ParamDefBndPath);
#endif
            return new FileInfo(gameParamDefBnd.Entries.First().Name).DirectoryName;
        }

        private void LoadAllPARAMs()
        {
            if (Config?.InterrootPath == null)
                return;

            string internalParamDefDirectory = CheckInternalParamDefDirectory();

            LoadEmbeddedParamDefBnd(internalParamDefDirectory);

            PARAMBNDs.Clear();
            var UPCOMING_Params = new ObservableCollection<PARAMRef>();

            ParamDefs.Clear();

#if REMASTER
            var gameparamBnds = Directory.GetFiles(Config.GameParamFolder, "*.parambnd.dcx")
                .Select(p => DataFile.LoadFromDcxFile<BND>(p));

            var drawparamBnds = Directory.GetFiles(Config.DrawParamFolder, "*.parambnd.dcx")
                .Select(p => DataFile.LoadFromDcxFile<BND>(p));
#else
            var gameparamBnds = Directory.GetFiles(Config.GameParamFolder, "*.parambnd")
                .Select(p => DataFile.LoadFromFile<BND>(p));

            var drawparamBnds = Directory.GetFiles(Config.DrawParamFolder, "*.parambnd")
                .Select(p => DataFile.LoadFromFile<BND>(p));
#endif



            PARAMBNDs = gameparamBnds.Concat(drawparamBnds).ToList();

            foreach (var paramDef in PARAMDEFBND)
            {
                var newParamDefName = IOHelper.RemoveExtension(new FileInfo(paramDef.Name).Name, EXT_PARAMDEF);

#if REMASTER
                if (newParamDefName.Contains("ToneCorrectBank") || newParamDefName.Contains("ToneMapBank") || newParamDefName.Contains("LevelSync"))
                {
                    continue;
                }
#endif

                ParamDefs.Add(new PARAMDEFRef(newParamDefName, paramDef.ReadDataAs<PARAMDEF>()));
            }

            for (int i = 0; i < PARAMBNDs.Count; i++)
            {
                foreach (var param in PARAMBNDs[i])
                {
                   

                    var newParam = param.ReadDataAs<PARAM>();

#if REMASTER
                    if (newParam.ID == "LEVELSYNC_PARAM_ST" || newParam.ID == "TONE_CORRECT_BANK" || newParam.ID == "TONE_MAP_BANK")
                    {
                        continue;
                    }
#endif

                    var newParamName = IOHelper.RemoveExtension(new FileInfo(param.Name).Name, EXT_PARAM);

                    newParam.ApplyPARAMDEFTemplate(ParamDefs.Where(x => x.Value.ID == newParam.ID).First().Value);

                    string newParamBndName = MiscUtil.GetFileNameWithoutDirectoryOrExtension(new FileInfo(PARAMBNDs[i].FilePath).Name);

#if REMASTER
                    //Extra pass due to the .dcx
                    newParamBndName = MiscUtil.GetFileNameWithoutDirectoryOrExtension(newParamBndName);
#endif


                    UPCOMING_Params.Add(new PARAMRef(newParamName, newParam, 
                        PARAMBNDs[i].FilePath.ToUpper().Contains("DRAW"),
                        newParamBndName));
                }
            }

            UPCOMING_Params = new ObservableCollection<PARAMRef>(UPCOMING_Params
                .OrderBy(x => x.Value.ID)
                .OrderBy(x => x.IsDrawParam));

            Application.Current.Dispatcher.Invoke(() =>
            {
                Params = UPCOMING_Params;
            });
        }

        public void ApplyParamDefEnglishPatch()
        {
            var originalBnd = DataFile.LoadFromFile<BND>(Config.ParamDefBndPath, null);

            TranslateParamDefs(originalBnd, Config.ParamDefBndPath);
        }

        static void TranslateParamDefs(BND inputParamDefBnd, string outputParamDefBndFileName)
        {
            var inputParamDefs = new Dictionary<string, PARAMDEF>();
            for (int i = 0; i < inputParamDefBnd.Count; i++)
            {
                inputParamDefs.Add(inputParamDefBnd.Entries[i].Name, inputParamDefBnd.Entries[i].ReadDataAs<PARAMDEF>(null));
            }

            foreach (var pd in inputParamDefs.Values)
            {
                foreach (var e in pd.Entries)
                {
                    if (!(e.GuiValueType == ParamTypeDef.dummy8 || e.InternalValueType == ParamTypeDef.dummy8))
                    {
                        string niceName = e.Name;
                        int colonIndex = niceName.LastIndexOf(':');
                        if (colonIndex >= 0)
                        {
                            niceName = niceName.Substring(0, colonIndex);
                        }

                        e.DisplayName = niceName;
                    }
                }
            }

            for (int i = 0; i < inputParamDefBnd.Count; i++)
            {
                inputParamDefBnd.Entries[i].ReplaceData(inputParamDefs[inputParamDefBnd.Entries[i].Name], null);
            }

            DataFile.SaveToFile(inputParamDefBnd, outputParamDefBndFileName, null);
        }

        //public void InitDataGridColumns(DataGrid dg, PARAM p)
        //{
        //    var matchingDef = p.AppliedPARAMDEF;

        //    dg.Columns.Clear();

        //    for (int i = 0; i < matchingDef.Entries.Count; i++)
        //    {
        //        var c = new DataGridTextColumn();

        //        c.Width = dg.ColumnWidth;
        //        c.Header = matchingDef.Entries[i].Name;

        //        var cellBinding = new Binding($"Cells[{i}].Value");

        //        //cellBinding.IsAsync = true;
        //        //cellBinding.FallbackValue = "(Loading...)";

        //        c.Binding = cellBinding;

        //        dg.Columns.Add(c);
        //    }
        //}

        public async Task SaveInOtherThread(Action<bool> setIsLoading)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    setIsLoading?.Invoke(true);
                });

                var backupsCreated = new List<string>();
                SaveAllPARAMs(backupsCreated);

                if (backupsCreated.Count > 0)
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("The following param-related file backup(s) did not exist and had to be created before saving:");

                    foreach (var b in backupsCreated)
                    {
                        sb.AppendLine($"\t'{b.Replace(Config.InterrootPath, ".")}'");
                    }

                    sb.AppendLine();

                    sb.AppendLine("Note: previously-created backups are NEVER overridden by this application. " +
                        "Subsequent file save operations will not display a messagebox if a backup of every file already exists.");

                    MessageBox.Show(sb.ToString(), "Backups Created Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                SaveConfig();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    setIsLoading?.Invoke(false);
                });
            });
        }

        //public void DEBUG_RestoreBackupsLoadResave()
        //{
        //    LoadConfig();

        //    var gameparamBnds = Directory.GetFiles(Config.GameParamFolder, "*.parambnd")
        //        .Select(p => DataFile.LoadFromFile<BND>(p, new Progress<(int, int)>((pr) =>
        //        {

        //        })));

        //    var drawparamBnds = Directory.GetFiles(Config.DrawParamFolder, "*.parambnd")
        //        .Select(p => DataFile.LoadFromFile<BND>(p, new Progress<(int, int)>((pr) =>
        //        {

        //        })));

        //    PARAMBNDs = gameparamBnds.Concat(drawparamBnds).ToList();

        //    foreach (var bnd in PARAMBNDs)
        //    {
        //        bnd.RestoreBackup();
        //    }

        //    LoadAllPARAMs();

        //    var asdf = new List<string>();
        //    SaveAllPARAMs(asdf);

        //    Application.Current.Shutdown();
        //}

        public async Task RestoreBackupsInOtherThread(Action<bool> setIsLoading)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    setIsLoading?.Invoke(true);
                });

                foreach (var bnd in PARAMBNDs)
                {
                    bnd.RestoreBackup();

#if REMASTER
                    DataFile.ReloadDcx(bnd);
#else
                    DataFile.Reload(bnd);
#endif
                }

                LoadAllPARAMs();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    setIsLoading?.Invoke(false);
                });
            });
        }

        private void SaveAllPARAMs(List<string> backupsCreated)
        {
            foreach (var paramBnd in PARAMBNDs)
            {
                if(paramBnd.CreateBackup(overwriteExisting: false) == true)
                {
                    backupsCreated.Add(paramBnd.FileBackupPath);
                }

                foreach (var param in paramBnd)
                {
                    string paramName = IOHelper.RemoveExtension(new FileInfo(param.Name).Name, EXT_PARAM);
                    var matchingParams = Params.Where(x => x.Key == paramName);

#if REMASTER
                    if (paramName.Contains("ToneCorrectBank") || paramName.Contains("ToneMapBank") || paramName.Contains("LevelSync"))
                    {
                        continue;
                    }
#endif

                    if (matchingParams.Any())
                    {
                        param.ReplaceData(matchingParams.First().Value);
                    }
                    else
                    {
                        MessageBox.Show($"Param \"{paramName}\" was not found " +
                            $"in \"{new FileInfo(paramBnd.FilePath).Name}\".", 
                            "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }


                }

#if REMASTER
                DataFile.ResaveDcx(paramBnd);
#else
                DataFile.Resave(paramBnd);
#endif


            }
        }

        //public void SaveAllPARAMDEFs(List<string> backupsCreated)
        //{
        //    if (PARAMDEFBND.CreateBackup(overwriteExisting: false) == true)
        //    {
        //        backupsCreated.Add(PARAMDEFBND.FileBackupPath);
        //    }

        //    foreach (var paramDef in PARAMDEFBND)
        //    {
        //        paramDef.ReplaceData(ParamDefs.Where(x => x.Key == new FileInfo(paramDef.Name).Name).First().Value, new Progress<(int, int)>((p) =>
        //        {

        //        }));
        //    }

        //    DataFile.Resave(PARAMDEFBND, new Progress<(int, int)>((p) =>
        //    {

        //    }));
        //}





        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
