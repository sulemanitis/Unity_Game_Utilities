using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameUtilities
{
    public class ScriptableObjectSavaData : MonoBehaviour
    {

        [Header("Meta")]
        public string persisterName;

        [Header("Objects to Persist")]
        public List<ScriptableObject> objectsToPersist;

        private void OnEnable()
        {
            if(PlayerPrefs.GetInt("newGame", 1) == 1)
            {
                ClearData();
                //GameManager.Instance.OnNewGame();
                PlayerPrefs.SetInt("newGame", 0);
            }
            else
            {
                LoadData();
            }
        }
        private void OnDisable() => SaveData();

        private void LoadData()
        {
#if !UNITY_EDITOR
            for (int i = 0; i < objectsToPersist.Count; i++)
            {
                if (File.Exists(Application.persistentDataPath + string.Format("/{0}_{1}.save", persisterName, i)))
                {
                    BinaryFormatter binary = new BinaryFormatter();
                    FileStream file = File.Open(Application.persistentDataPath + string.Format("/{0}_{1}.save", persisterName, i), FileMode.Open);
                    JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objectsToPersist[i]);
                    file.Close();
                }
                else
                {
                    Debug.LogWarning(objectsToPersist[i].name +" File not found");
                }
            }
#endif
        }


        private void SaveData()
        {
            for (int i = 0; i < objectsToPersist.Count; i++)
            {
                BinaryFormatter binary = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.save", persisterName, i));
                var json = JsonUtility.ToJson(objectsToPersist[i]);
                binary.Serialize(file, json);
                file.Close();
            }

        }
        public void ClearData()
        {
            for (int i = 0; i < objectsToPersist.Count; i++)
            {
                string path = Application.persistentDataPath + string.Format("/{0}_{1}.save", persisterName, i);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    Debug.LogWarning(objectsToPersist[i].name + " File not found and not deleted");
                }

            }
        }
    }
}