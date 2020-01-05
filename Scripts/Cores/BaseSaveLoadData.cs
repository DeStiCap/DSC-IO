namespace DSC.IO
{
    [System.Serializable]
    public abstract class BaseSaveLoadData
    {
        public abstract void Clear();
        public abstract void CopyFrom<Data>(Data hData) where Data : BaseSaveLoadData;
       
    }
}