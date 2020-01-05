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
                    Debug.LogError("Don't have SaveLoadManager in scene.");

                return m_hInstance;
            }
        }

        #endregion

        protected static DSC_IO_SaveLoadManager m_hInstance;
        protected static bool m_bAppStart;
        protected static bool m_bAppQuit;

        protected Dictionary<string, BaseSaveLoadData> m_dicData = new Dictionary<string, BaseSaveLoadData>();

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

        public static void Save<Data>(Data hData,string sFileName) where Data : BaseSaveLoadData
        {
            if (Instance == null)
                return;

            m_hInstance.MainSave(hData, sFileName);
        }

        protected virtual void MainSave<Data>(Data hData,string sFileName) where Data : BaseSaveLoadData
        {
            SaveLoadSystem<Data>.Save(hData, sFileName);
        }

        public static Data Load<Data>(string sFileName) where Data : BaseSaveLoadData
        {
            return Instance?.MainLoad<Data>(sFileName);
        }

        protected virtual Data MainLoad<Data>(string sFileName) where Data : BaseSaveLoadData
        {
            return SaveLoadSystem<Data>.Load(sFileName);
        }

        public static void SaveTempData<Data>(Data hData,string sFileName) where Data : BaseSaveLoadData,new()
        {
            Instance?.MainSaveTempData(hData, sFileName);
        }

        protected virtual void MainSaveTempData<Data>(Data hData,string sFileName) where Data : BaseSaveLoadData,new()
        {
            if (m_dicData.ContainsKey(sFileName))
            {
                var hOutData = m_dicData[sFileName];
                hOutData.Clear();
                hOutData.CopyFrom(hData);
                m_dicData[sFileName] = hOutData;
            }
            else
            {
                var hNewData = new Data();
                hNewData.CopyFrom(hData);
                m_dicData.Add(sFileName, hNewData);
            }
        }

        public static void LoadTempData<Data>(string sFileName,ref Data hLoadData) where Data : BaseSaveLoadData,new()
        {
            Instance?.MainLoadTempData(sFileName, ref hLoadData);
        }

        protected virtual void MainLoadTempData<Data>(string sFileName, ref Data hLoadData) where Data : BaseSaveLoadData,new()
        {
            MainTryLoadTempData(sFileName, ref hLoadData);
        }

        public static bool TryLoadTempData<Data>(string sFileName,ref Data hLoadData) where Data : BaseSaveLoadData,new()
        {
            if (Instance == null)
                return false;

            return m_hInstance.MainTryLoadTempData(sFileName, ref hLoadData);
        }

        protected virtual bool MainTryLoadTempData<Data>(string sFileName,ref Data hLoadData) where Data : BaseSaveLoadData,new()
        {
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

            var hData = hOutData as Data;
            if (hLoadData == null)
                hLoadData = new Data();

            hLoadData.CopyFrom(hData);
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