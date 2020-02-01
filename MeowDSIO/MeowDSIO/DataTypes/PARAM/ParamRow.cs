using MeowDSIO.DataTypes.PARAMDEF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.PARAM
{
    public class ParamRow
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public byte[] RawData { get; set; }

        public ObservableCollection<ParamCellValueRef> Cells { get; set; }

        public object this[string fieldName]
        {
            get
            {
                var fieldsOfName = Cells.Where(x => x.Def.Name == fieldName);
                if (fieldsOfName.Any())
                {
                    return fieldsOfName.First().Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var fieldsOfName = Cells.Where(x => x.Def.Name == fieldName);
                if (fieldsOfName.Any())
                {
                    fieldsOfName.First().Value = value;
                }
                else
                {
                    throw new Exception($"Param column {fieldName} does not exist in this param");
                }
            }
        }

        public void LoadValuesFromRawData(DataFiles.PARAM Parent)
        {
            int offset = 0, bitVal = 0, bitField = 0;

            string paramNameCheck = (Parent.FilePath ?? Parent.VirtualUri).ToUpper();

            bool isBadToneMapParamDS1R = paramNameCheck.Contains(@"_X64\PARAM\DRAWPARAM\M99_TONEMAPBANK");

            bool isBadToneCorrectParamDS1R = paramNameCheck.Contains(@"_X64\PARAM\DRAWPARAM\M99_TONECORRECTBANK") ||
                paramNameCheck.Contains(@"_X64\PARAM\DRAWPARAM\DEFAULT_TONECORRECTBANK");

            Cells = new ObservableCollection<ParamCellValueRef>();
            for (int i = 0; i < Parent.AppliedPARAMDEF.Entries.Count; i++)
            {
                if (i == Parent.AppliedPARAMDEF.Entries.Count - 1)
                {
                    if (isBadToneMapParamDS1R)
                    {
                        var nextCellOof = new ParamCellValueRef(Parent.AppliedPARAMDEF.Entries[i]);
                        nextCellOof.Value = 1.0f;
                        Cells.Add(nextCellOof);
                        //Parent.EntrySize = 48;
                        break;
                    }
                    else if (isBadToneCorrectParamDS1R)
                    {
                        var nextCellOof = new ParamCellValueRef(Parent.AppliedPARAMDEF.Entries[i]);
                        nextCellOof.Value = 1.0f;
                        Cells.Add(nextCellOof);
                        //Parent.EntrySize = 36;
                        break;
                    }

                }

                var nextCell = new ParamCellValueRef(Parent.AppliedPARAMDEF.Entries[i]);

                //if (nextCell.Def.Name.StartsWith("modelDispMask0"))
                //{
                //    Console.WriteLine($"{(Parent.FilePath ?? Parent.VirtualUri)} modelDispMask0 offset: 0x{offset:X3}");
                //}

                //if (nextCell.Def.Name.StartsWith("modelDispMask16"))
                //{
                //    Console.WriteLine($"{(Parent.FilePath ?? Parent.VirtualUri)} modelDispMask16 offset: 0x{offset:X3}");
                //}

                //if (nextCell.Def.Name.StartsWith("equipModelId"))
                //{
                //    Console.WriteLine($"{(Parent.FilePath ?? Parent.VirtualUri)} equipModelId offset: 0x{offset:X3}");
                //}

                if (nextCell.Def.Name.StartsWith("throwTypeId"))
                {
                    Console.WriteLine($"{(Parent.FilePath ?? Parent.VirtualUri)} throwTypeId offset: 0x{offset:X}");
                }

                //if (nextCell.Def.Name.StartsWith("invisibleFlag48"))
                //{
                //    Console.WriteLine($"{(Parent.FilePath ?? Parent.VirtualUri)} invisibleFlag48 offset: 0x{offset:X}");
                //}

                nextCell.Value = Parent.AppliedPARAMDEF.Entries[i].ReadValueFromParamEntryRawData(this, ref offset, ref bitField, ref bitVal);
                Cells.Add(nextCell);
            }
        }

        public void ClearRawData()
        {
            RawData = null;
        }

        public void SaveValuesToRawData(DataFiles.PARAM Parent)
        {
            int offset = 0, bitVal = 0, bitField = 0;

            string paramNameCheck = (Parent.FilePath ?? Parent.VirtualUri).ToUpper();

            bool isBadToneMapParamDS1R = Parent.IsDarkSoulsRemastered 
                && paramNameCheck.Contains(@"M99_TONEMAPBANK");

            bool isBadToneCorrectParamDS1R = Parent.IsDarkSoulsRemastered && (
                paramNameCheck.Contains(@"M99_TONECORRECTBANK") ||
                paramNameCheck.Contains(@"DEFAULT_TONECORRECTBANK")
                );

            for (int i = 0; i < Parent.AppliedPARAMDEF.Entries.Count; i++)
            {
                if (i == Parent.AppliedPARAMDEF.Entries.Count - 1)
                {
                    if (isBadToneMapParamDS1R || isBadToneCorrectParamDS1R)
                    {
                        // Write them like PTDE params all shitty like ds1r expects.
                        break;
                    }
                }

                Parent.AppliedPARAMDEF.Entries[i]
                    .WriteValueToParamEntryRawData(this, Cells[i].Value, ref offset, ref bitField, ref bitVal);
            }
        }

        public void ReInitRawData(DataFiles.PARAM Parent)
        {
            RawData = new byte[Parent.EntrySize];
        }

        public void SaveDefaultValuesToRawData(DataFiles.PARAM Parent)
        {
            RawData = new byte[Parent.EntrySize];

            int offset = 0, bitVal = 0, bitField = 0;

            for (int i = 0; i < Parent.AppliedPARAMDEF.Entries.Count; i++)
            {
                Parent.AppliedPARAMDEF.Entries[i]
                    .WriteValueToParamEntryRawData(this, Cells[i].Value, ref offset, ref bitField, ref bitVal);
            }
        }

        public override string ToString()
        {
            return ID + ": " + Name;
        }

    }
}