using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class ChoosePlayerScript : MonoBehaviour
{
    public TMPro.TMP_Dropdown listamapa;

    // Start is called before the first frame update
    void Start()
    {
        if(File.Exists(Application.persistentDataPath + "/listamapa.txt")){

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/listamapa.txt"); // create a new StreamReader to read from the file
            List<string> options = new List<string>();
            while(!reader.EndOfStream){
                string line = reader.ReadLine();
                options.Add(line);
            }
            listamapa.AddOptions(options);
            listamapa.value = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/playerNumber.txt")){
            writer.Write(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        }
        //prebaciti mapu...
        List<TMPro.TMP_Dropdown.OptionData> options = listamapa.options;
        using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + options[listamapa.value].text + ".txt")){
            string text = "";
            for(int i = 0; i < 7; i++){
                text += reader.ReadLine() + "\n";
            }
            using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/matrix.txt")){
                writer.Write(text);
            }
            using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/player.txt")){
                writer.Write(reader.ReadLine());
            }
            using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/treasure.txt")){
                writer.Write(reader.ReadLine());
            }
        }
        
        SceneManager.LoadScene("GameScene");
    }
}
