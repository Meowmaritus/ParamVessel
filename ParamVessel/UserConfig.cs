﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    public enum GameKind
    {
        //DeS,
        DS1,
        DS1R,
        //DS2,
        //DS3,
        BB,
    }

    public class UserConfig : INotifyPropertyChanged
    {
        [JsonIgnore]
        private string _interrootPath = null;

        [JsonIgnore]
        private Dictionary<string, int> _lastParamEntryIndices = new Dictionary<string, int>();

        [JsonIgnore]
        private int _lastParamIndex = -1;

        public string InterrootPath
        {
            get => _interrootPath;
            set
            {
                _interrootPath = value;

                NotifyPropertyChanged(nameof(InterrootPath));
                NotifyPropertyChanged(nameof(GameParamFolder));
                NotifyPropertyChanged(nameof(DrawParamFolder));
                NotifyPropertyChanged(nameof(ParamDefBndPath));
                NotifyPropertyChanged(nameof(ParamFolder));
            }
        }


        public Dictionary<string, int> LastParamEntryIndices
        {
            get => _lastParamEntryIndices;
            set
            {
                _lastParamEntryIndices = value;
                NotifyPropertyChanged(nameof(LastParamEntryIndices));
            }
        }

        public int LastParamIndex
        {
            get => _lastParamIndex;
            set
            {
                _lastParamIndex = value;
                NotifyPropertyChanged(nameof(LastParamIndex));
            }
        }

        private GameKind _kind = GameKind.DS1;
        public GameKind Kind
        {
            get => _kind;
            set
            {
                _kind = value;
                NotifyPropertyChanged(nameof(Kind));
                NotifyPropertyChanged(nameof(IsDCX));
            }
        }

        [JsonIgnore]
        public bool IsDCX => Kind != GameKind.DS1;

        [JsonIgnore]
        public string GameParamFolder
            => IOHelper.Frankenpath(InterrootPath, "param\\GameParam\\");

        [JsonIgnore]
        public string DrawParamFolder
            => IOHelper.Frankenpath(InterrootPath, "param\\DrawParam\\");

        [JsonIgnore]
        public string ParamFolder
            => IOHelper.Frankenpath(InterrootPath, "param\\");

        [JsonIgnore]
        public string ParamDefBndPath
            => IOHelper.Frankenpath(InterrootPath, $"paramdef\\paramdef.paramdefbnd{(Kind != GameKind.DS1 ? ".dcx" : "")}");

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
