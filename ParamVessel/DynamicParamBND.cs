using MeowDSIO;
using MeowDSIO.DataFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    public class DynamicParamBND
    {
        public List<PARAM> Params { get; set; } = new List<PARAM>();

        public string FilePath = "";

        public bool IsBND4 = false;

        public static DynamicParamBND Load(string fileName, bool isBnd4)
        {
            var result = new DynamicParamBND();
            result.LoadFromFile(fileName, isBnd4);
            return result;
        }


        public string FileBackupPath
        {
            get
            {
                if (FilePath == null)
                    return null;

                return FilePath + ".bak";
            }
        }

        public void Reload()
        {
            LoadFromFile(FilePath, IsBND4);
        }

        /// <summary>
        /// Checks if a backup exists.
        /// </summary>
        /// <returns>Null if FilePath is null, True if backup exists, etc.</returns>
        public bool? CheckBackupExist()
        {
            if (FilePath == null)
                return null;

            return File.Exists(FileBackupPath);
        }

        /// <summary>
        /// Creates backup.
        /// </summary>
        /// <param name="overwriteExisting">If this is false, no new backups will be created if one already exists.
        /// This preserves the initial backup, preventing any changes from ocurring to it.</param>
        /// <returns>True if backup is saved, False if no backup is saved to preserve existing, Null if FilePath == Null.</returns>
        public bool? CreateBackup(bool overwriteExisting = false)
        {
            if (FilePath == null)
                return null;

            if (overwriteExisting || CheckBackupExist() == false)
            {
                File.Copy(FilePath, FileBackupPath);
                return true;
            }
            else
            {
                return false;
            }
        }

        public DateTime? GetBackupDate()
        {
            if (FilePath == null)
                return null;

            return File.GetCreationTime(FileBackupPath);
        }

        /// <summary>
        /// Overwrites the file with a previously-created backup.
        /// </summary>
        /// <returns>True if backup restored, False if no backup exists, Null if FilePath is Null</returns>
        public bool? RestoreBackup()
        {
            if (FilePath == null)
                return null;

            if (CheckBackupExist() == true)
            {
                File.Copy(FileBackupPath, FilePath, overwrite: true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadFromFile(string fileName, bool isBnd4)
        {
            IsBND4 = isBnd4;

            FilePath = fileName;

            Params.Clear();

            if (isBnd4)
            {
                var bnd4 = SoulsFormats.BND4.Read(fileName);
                foreach (var f in bnd4.Files)
                {
                    var param = DataFile.LoadFromBytes<PARAM>(f.Bytes, f.Name);
                    Params.Add(param);
                }
            }
            else
            {
                var parambnd = DataFile.LoadFromFile<PARAMBND>(fileName);

                parambnd.ApplyDefaultParamDefs();

                foreach (var f in parambnd)
                {
                    Params.Add(f.Param);
                }
            }
        }

        public void Resave()
        {
            SaveToFile(FilePath, IsBND4);
        }

        public void SaveToFile(string fileName, bool isBnd4)
        {
            int i = 0;

            FilePath = fileName;

            if (isBnd4)
            {
                var bnd4 = SoulsFormats.BND4.Read(fileName);

                bnd4.Files.Clear();

                foreach (var p in Params)
                {
                    bnd4.Files.Add(new SoulsFormats.BND4.File(i++, p.VirtualUri, 64, DataFile.SaveAsBytes(p, p.VirtualUri)));
                }

                bnd4.Write(fileName);
            }
            else
            {
                var parambnd = DataFile.LoadFromFile<PARAMBND>(fileName);

                parambnd.Clear();

                foreach (var p in Params)
                {
                    parambnd.Add(new MeowDSIO.DataTypes.PARAMBND.PARAMBNDEntry(MiscUtil.GetFileNameWithoutDirectoryOrExtension(p.VirtualUri), p));
                }

                DataFile.SaveToFile(parambnd, fileName);
            }
        }
    }
}
