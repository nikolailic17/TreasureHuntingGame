using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!File.Exists(Application.persistentDataPath + "/listamapa.txt")){
            Debug.Log("1111");
            using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/listamapa.txt", true)){
                writer.Write("Default\n");
            }
        }
        if(!File.Exists(Application.persistentDataPath + "/Default.txt")){
            using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/Default.txt", true)){
                string linija1 = "0,0,0,0,0,0,0\n";
                string linija2 = "0,0\n";
                string linija3 = "1,1";
                for(int i = 0; i < 7; i++){
                    writer.Write(linija1);
                }
                writer.Write(linija2);
                writer.Write(linija3);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame(){
        SceneManager.LoadScene("ChoosePlayerScene");
    }

    public void Settings(){
        SceneManager.LoadScene("SettingsScene");
    }

    public void About(){
        SceneManager.LoadScene("AboutScene");
    }
}
