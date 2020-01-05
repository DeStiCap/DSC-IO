using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DSC.IO
{
    public static class SaveLoadSystem<SaveLoadData> where SaveLoadData : class
    {
        public static void Save(SaveLoadData hData,string sFileName)
        {
            sFileName += ".sav";
            BinaryFormatter hFormatter = new BinaryFormatter();            
            string sPath = Path.Combine(Application.persistentDataPath, sFileName);
            FileStream hStream = new FileStream(sPath, FileMode.Create);

            hFormatter.Serialize(hStream, hData);
            hStream.Close();
        }

        public static SaveLoadData Load(string sFileName)
        {
            sFileName += ".sav";
            string sPath = Path.Combine(Application.persistentDataPath, sFileName);
            if (File.Exists(sPath))
            {
                BinaryFormatter hFormatter = new BinaryFormatter();
                FileStream hStream = new FileStream(sPath, FileMode.Open);

                SaveLoadData hData = (SaveLoadData)hFormatter.Deserialize(hStream);
                hStream.Close();
                return hData;
            }
            else
            {
                Debug.LogError("Save file not found in " + sPath);
                return null;
            }
        }
    }
}