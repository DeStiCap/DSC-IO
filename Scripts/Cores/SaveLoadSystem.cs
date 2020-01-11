using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DSC.IO
{
    public static class SaveLoadSystem
    {
        public static void Save<SaveLoadData>(SaveLoadData hData,string sFileName)
        {
            string sFolder = Path.Combine(Application.persistentDataPath, "Save");
            sFileName += ".sav";
            
            if (!Directory.Exists(sFolder))
            {
                Directory.CreateDirectory(sFolder);
            }

            BinaryFormatter hFormatter = new BinaryFormatter();            
            string sPath = Path.Combine(sFolder, sFileName);
            using (FileStream hStream = new FileStream(sPath, FileMode.Create))
            {
                hFormatter.Serialize(hStream, hData);
                hStream.Close();
            }
        }

        public static SaveLoadData Load<SaveLoadData>(string sFileName)
        {
            string sFolder = Path.Combine(Application.persistentDataPath, "Save");
            if (!Directory.Exists(sFolder))
            {
                Debug.LogError("Save folder not found in " + sFolder);
                return default;
            }

            sFileName += ".sav";
            string sPath = Path.Combine(sFolder, sFileName);

            if (File.Exists(sPath))
            {
                BinaryFormatter hFormatter = new BinaryFormatter();

                using (FileStream hStream = new FileStream(sPath, FileMode.Open))
                {
                    SaveLoadData hData = (SaveLoadData)hFormatter.Deserialize(hStream);
                    hStream.Close();
                    return hData;
                }
            }
            else
            {
                Debug.LogError("Save file not found in " + sPath);                
            }

            return default;
        }

        public static bool HasSaveFile(string sFileName)
        {
            string sFolder = Path.Combine(Application.persistentDataPath, "Save");

            if (!Directory.Exists(sFolder))
                return false;

            sFileName += ".sav";
            string sPath = Path.Combine(sFolder, sFileName);

            return File.Exists(sPath);
        }
    }
}