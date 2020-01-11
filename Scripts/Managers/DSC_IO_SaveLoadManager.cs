using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSC.IO
{
    public class DSC_IO_SaveLoadManager : MonoBehaviour
    {
        #region Variable

        #region Variable - Property

        protected static DSC_IO_SaveLoadManager Instance
        {
            get
            {
                if (m_hInstance == null && m_bAppStart && !m_bAppQuit)
                    Debug.LogWarning("Don't have SaveLoadManager in scene.");

                return m_hInstance;
            }
        }

        #endregion

        protected static DSC_IO_SaveLoadManager m_hInstance;
        protected static bool m_bAppStart;
        protected static bool m_bAppQuit;

        protected Dictionary<string, ISaveable> m_dicData = new Dictionary<string, ISaveable>();

        #endregion

        #region Base - Mono

        protected virtual void Awake()
        {
            if(m_hInstance == null)
            {
                m_hInstance = this;
            }
            else if(m_hInstance != this)
            {
                Destroy(this);
                return;
            }

            Application.quitting += OnAppQuit;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void OnAppStart()
        {
            m_bAppStart = true;
            m_bAppQuit = false;
        }

        protected void OnAppQuit()
        {
            Application.quitting -= OnAppQuit;

            m_bAppStart = false;
            m_bAppQuit = true;
        }

        #endregion

        #region Main

        public static void Save<Data>(Data hData,string sFileName) where Data : ISaveable
        {
            if (Instance == null)
                return;

            m_hInstance.MainSave(hData, sFileName);
        }

        protected virtual void MainSave<Data>(Data hData,string sFileName) where Data : ISaveable
        {
            SaveLoadSystem.Save(hData, sFileName);
        }

        public static Data Load<Data>(string sFileName) where Data : ISaveable
        {
            if (Instance == null)
                return default;

            return m_hInstance.MainLoad<Data>(sFileName);
        }

        protected virtual Data MainLoad<Data>(string sFileName) where Data : ISaveable
        {
            return SaveLoadSystem.Load<Data>(sFileName);
        }

        public static bool HasSaveFile(string sFileName)
        {
            return SaveLoadSystem.HasSaveFile(sFileName);
        }

        public static void SaveTempData<Data>(Data hData,string sFileName) where Data : struct, ISaveable
        {
            Instance?.MainSaveTempData(hData, sFileName);
        }

        protected virtual void MainSaveTempData<Data>(Data hData,string sFileName) where Data : struct, ISaveable
        {
            if (m_dicData.ContainsKey(sFileName))
            {
                m_dicData[sFileName] = hData;
            }
            else
            {
                m_dicData.Add(sFileName, hData);
            }
        }

        public static Data LoadTempData<Data>(string sFileName) where Data : struct, ISaveable
        {
            return m_hInstance.MainLoadTempData<Data>(sFileName);
        }

        protected virtual Data MainLoadTempData<Data>(string sFileName) where Data : struct, ISaveable
        {
            MainTryLoadTempData<Data>(sFileName, out var hLoadData);
            return hLoadData;
        }

        public static bool TryLoadTempData<Data>(string sFileName,out Data hLoadData) where Data : struct, ISaveable
        {
            hLoadData = default;
            if (Instance == null)
                return false;

            return m_hInstance.MainTryLoadTempData(sFileName, out hLoadData);
        }

        protected virtual bool MainTryLoadTempData<Data>(string sFileName, out Data hLoadData) where Data : struct, ISaveable
        {
            hLoadData = default;
            if (!m_dicData.TryGetValue(sFileName, out var hOutData) || hOutData == null)
            {
                Debug.LogError("Don't have temp data file " + sFileName);
                return false;
            }

            if (!(hOutData is Data))
            {
                Debug.LogError("File " + sFileName + " not type " + typeof(Data));
                return false;
            }

            hLoadData = (Data) hOutData;
            return true;
        }

        public static void WriteSaveTempData(string sFileName)
        {
            Instance?.MainWriteSaveTempData(sFileName);
        }

        protected virtual void MainWriteSaveTempData(string sFileName)
        {
            if(!m_dicData.TryGetValue(sFileName,out var hOutData) || hOutData == null)
            {
                Debug.LogError("Don't have temp data file " + sFileName);
                return;
            }

            MainSave(hOutData, sFileName);
        }

        #endregion
    }
}