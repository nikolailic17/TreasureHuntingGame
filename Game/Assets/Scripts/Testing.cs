using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
//using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    public Sprite redSquare;
    public Sprite greenSquare;
    public Sprite yellowSquare;
    public Sprite blueSquare;
    private Grid grid;
    public TMP_InputField inputFieldXPlayer;
    public TMP_InputField inputFieldYPlayer;
    public TMP_InputField inputFieldXTreasure;
    public TMP_InputField inputFieldYTreasure;

    private int koordinataXigraca, koordinataYigraca;
    private int koordinataXblaga, koordinataYblaga;

    private void Start() {
        inputFieldXPlayer.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldYPlayer.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldXTreasure.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldYTreasure.contentType = TMP_InputField.ContentType.IntegerNumber;

        this.grid = new Grid(7, 7, 3f, redSquare, greenSquare, yellowSquare, blueSquare);
        
        if(File.Exists(Application.persistentDataPath + "/matrix.txt")){
            //Debug.Log("USAO");

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/matrix.txt"); // create a new StreamReader to read from the file

            for (int i = 0; i < 7; i++)
            {
                string line = reader.ReadLine(); // read a line from the file
                string[] parts = line.Split(','); // split the line into an array of strings
                for (int j = 0; j < 7; j++)
                {
                    //loadedMatrix[i, j] = int.Parse(parts[j]); // convert each string to an integer and store it in the new array
                    grid.SetValue(i, j, int.Parse(parts[j]));
                }
            }
        }
        if(File.Exists(Application.persistentDataPath + "/player.txt")){
            StreamReader reader = new StreamReader(Application.persistentDataPath + "/player.txt"); // create a new StreamReader to read from the file
            string line = reader.ReadLine();
            string[] parts = line.Split(',');
            koordinataXigraca = int.Parse(parts[0]);
            koordinataYigraca = int.Parse(parts[1]);
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null) {
                Transform playerTransform = playerObject.GetComponent<Transform>();
                //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
                playerTransform.position = new Vector3(-5.5f + koordinataXigraca*3, -1.5f + koordinataYigraca*3, 0f);
            }
        }
        if(File.Exists(Application.persistentDataPath + "/treasure.txt")){
            StreamReader reader = new StreamReader(Application.persistentDataPath + "/treasure.txt"); // create a new StreamReader to read from the file
            string line = reader.ReadLine();
            string[] parts = line.Split(',');
            koordinataXblaga = int.Parse(parts[0]);
            koordinataYblaga = int.Parse(parts[1]);
            Debug.Log("Koordinate blaga " + koordinataXblaga + koordinataYblaga);
            GameObject treasureObject = GameObject.Find("Treasure");
            if (treasureObject != null) {
                Transform treasureTransform = treasureObject.GetComponent<Transform>();
                //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
                treasureTransform.position = new Vector3(-5.5f + koordinataXblaga*3, -1.5f + koordinataYblaga*3, 0f);
            }
        }

    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)){
           /*Vector3 mousePos = Input.mousePosition;
            Debug.Log(mousePos.x);
            Debug.Log(mousePos.y   );*/
            //grid.SetValue(UtilsClass.GetMouseWorldPosition(), 5);

            float startX = -7f;
            float startY = -3f;
            float size = 3f;

            Vector3 clickPos = Input.mousePosition;
            clickPos.z = Camera.main.nearClipPlane;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
            Vector3 offset = worldPos - transform.position;
            if((offset.x - startX)/size<0 || (offset.x - startX)/size>=7) return;
            if((offset.y - startY)/size<0 || (offset.y - startY)/size>=7) return;
            
            int x = (int)((offset.x - startX)/size);
            int y = (int)((offset.y - startY)/size);
            grid.ChangeField(x, y);

            //Debug.Log("Mouse clikc coordinates " + x + " " + y);

            //Debug.Log("Mouse click offset from center: " + offset);
        }

        //ZUMOVANJE
        float zoomSpeed = -1f;
        float maxZoom = 8f;
        float minZoom = 4f;
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float newFieldOfView = Camera.main.orthographicSize + scrollWheel * zoomSpeed;
        //Debug.Log(newFieldOfView);
        // Clamp the new field of view to the min and max zoom values
        newFieldOfView = Mathf.Clamp(newFieldOfView, minZoom, maxZoom);
        // Set the camera's field of view to the new value
        Camera.main.orthographicSize = newFieldOfView;

        //metoda za pomeranje kamere
        if (Input.GetMouseButton(1)) {
            float minX =-13f+newFieldOfView*16/9;
            float minY =-8f+newFieldOfView;
            float maxX =18.5f-newFieldOfView*16/9;
            float maxY =19f-newFieldOfView;

            float moveSpeed = 20f;
            float horizontalMovement = Input.GetAxis("Mouse X");
            float verticalMovement = Input.GetAxis("Mouse Y");
            Transform transform = Camera.main.GetComponent<Transform>();
            //transform.Translate(new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime);

            Vector3 movement = new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime;
        
            float newX = Mathf.Clamp(transform.position.x + movement.x, minX, maxX);
            float newY = Mathf.Clamp(transform.position.y + movement.y, minY, maxY);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        
    }

    private int[,] pomocnaMatrica = new int[7,7];

    public bool ProveraPostojanjaPuta(int startX, int startY, int endX, int endY){
        if(startX == endX && startY == endY){
            return true;
        }
        if(startX < 0 || startX>6 || startY < 0 || startY > 6){
            //Debug.Log("Usao1 " + startX + " " + startY);
            return false;
        }
        if(grid.gridArray[startX, startY] == 3){
            //Debug.Log("Usao2 " + startX + " " + startY);
            return false;
        }
        if(pomocnaMatrica[startX, startY] == 1){
            //Debug.Log("Usao3 " + startX + " " + startY);
            return false;
        }
        Debug.Log("Usao1 " + startX + " " + startY);
        pomocnaMatrica[startX, startY] = 1;

        bool result = ProveraPostojanjaPuta(startX + 1, startY, endX, endY) ||
            ProveraPostojanjaPuta(startX - 1, startY, endX, endY) ||
            ProveraPostojanjaPuta(startX, startY - 1, endX, endY) ||
            ProveraPostojanjaPuta(startX, startY + 1, endX, endY);
        
        //pomocnaMatrica[startX, startY] = 0;

        return result;
    }

    public void SaveMap(){
        if(!ProveraPostojanjaPuta(koordinataXigraca, koordinataYigraca, koordinataXblaga, koordinataYblaga)){
            pomocnaMatrica = new int[7,7];
            Debug.Log("Nema puta, ne moze se sacuvati mapa");
            return;
        }
        pomocnaMatrica = new int[7,7];
        string matrix = "";
        for(int i = 0; i<7;i++){
            for(int j = 0; j<7; j++){
                matrix += grid.gridArray[i,j].ToString() + ",";
            }
            matrix += "\n";
        }
        File.WriteAllText(Application.persistentDataPath + "/matrix.txt", matrix);
    }

    public void RestartMap(){
        for(int i =0; i<7; i++){
            for(int j =0; j<7; j++){
                grid.SetValue(i, j, 0);
            }
        }
    }

    public void ChangePlayerPosition(){
        int newX = int.Parse(inputFieldXPlayer.text);
        int newY = int.Parse(inputFieldYPlayer.text);
        if(newX>=0 && newX<7 && newY>=0 && newY<7){
            if(!ProveraPostojanjaPuta(newX, newY, koordinataXblaga, koordinataYblaga)){
                pomocnaMatrica = new int[7,7];
                Debug.Log("Nema puta, ne mogu se promeniti koordinate igraca!");
                return;
            }
            koordinataXigraca = newX;
            koordinataYigraca = newY;
            string upis = "";
            upis += koordinataXigraca.ToString() + "," + koordinataYigraca.ToString();
            File.WriteAllText(Application.persistentDataPath + "/player.txt", upis);
            GameObject playerObject = GameObject.Find("Player");
            Transform playerTransform = playerObject.GetComponent<Transform>();
            playerTransform.position = new Vector3(-5.5f + koordinataXigraca*3, -1.5f + koordinataYigraca*3, 0f);
        }
    }

    public void ChangeTreasurePosition(){
        int newX = int.Parse(inputFieldXTreasure.text);
        int newY = int.Parse(inputFieldYTreasure.text);
        if(newX>=0 && newX<7 && newY>=0 && newY<7){
            if(!ProveraPostojanjaPuta(koordinataXigraca, koordinataYigraca, newX, newY)){
                pomocnaMatrica = new int[7,7];
                Debug.Log("Nema puta, ne mogu se promeniti koordinate blaga!");
                return;
            }
            koordinataXblaga = newX;
            koordinataYblaga = newY;
            string upis = "";
            upis += koordinataXblaga.ToString() + "," + koordinataYblaga.ToString();
            File.WriteAllText(Application.persistentDataPath + "/treasure.txt", upis);
            GameObject playerObject = GameObject.Find("Treasure");
            Transform playerTransform = playerObject.GetComponent<Transform>();
            playerTransform.position = new Vector3(-5.5f + koordinataXblaga*3, -1.5f + koordinataYblaga*3, 0f);
        }
    }

    public void Back(){
        SceneManager.LoadScene("MainMenuScene");
    }
}
