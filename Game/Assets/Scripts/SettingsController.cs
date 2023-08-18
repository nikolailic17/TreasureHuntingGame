using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class SettingsController : MonoBehaviour
{
    public TMPro.TMP_Dropdown listamapa;
    private Grid grid;
    public TMP_InputField inputFieldXPlayer;
    public TMP_InputField inputFieldYPlayer;
    public TMP_InputField inputFieldXTreasure;
    public TMP_InputField inputFieldYTreasure;
    public TMP_InputField inputFieldMapName;
    public Sprite redSquare;
    public Sprite greenSquare;
    public Sprite yellowSquare;
    public Sprite blueSquare;
    private int xPlayer=0, yPlayer=0;
    private int xTreasure=1, yTreasure=1;
    private string previousValue;
    public GameObject panelObject;
    private bool panelIsActive = false;
    public TextMeshProUGUI panelText;

    private float initialCameraSize = 3f;
    private void OnValueChanged(string newValue){
        if (!Regex.IsMatch(newValue, "^[a-zA-Z0-9]*$") || newValue.Length > 20)
        {
            inputFieldMapName.text = previousValue;
        }
        else
        {
            previousValue = newValue;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //iskjucujemo panel objekat
        panelObject.SetActive(false);

        //namestam da fileName input prima slova brojeve i duzinu do 20 karaktera
        previousValue = inputFieldMapName.text;
        inputFieldMapName.onValueChanged.AddListener(OnValueChanged);

        inputFieldXPlayer.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldYPlayer.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldXTreasure.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputFieldYTreasure.contentType = TMP_InputField.ContentType.IntegerNumber;

        this.grid = new Grid(7, 7, 3f, redSquare, greenSquare, yellowSquare, blueSquare);

        //dodavanje elemenata u dropdown menu
        if(File.Exists(Application.persistentDataPath + "/listamapa.txt")){
            using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/listamapa.txt")){ // create a new StreamReader to read from the file
                List<string> options = new List<string>();
                while(!reader.EndOfStream){
                    string line = reader.ReadLine();
                    options.Add(line);
                }
                listamapa.AddOptions(options);
                listamapa.value = 0;
            }
        }
    }

    // Update is called once per frame
    private void Update() {
        if(panelIsActive){
            return;
        }
        /*if(Input.GetMouseButtonDown(0)){
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
        }*/

        //ZUMOVANJE
        float zoomSpeed = -1f;
        float maxZoom = 7f;
        float minZoom = 4f;
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float newFieldOfView = Camera.main.orthographicSize + scrollWheel * zoomSpeed;
        newFieldOfView = Mathf.Clamp(newFieldOfView, minZoom, maxZoom);
        //Camera.main.orthographicSize = newFieldOfView;

        //metoda za pomeranje kamere
        /*if (Input.GetMouseButton(1)) {
            float minX =-13f+newFieldOfView*16/9;
            float minY =-8f+newFieldOfView;
            float maxX =18.5f-newFieldOfView*16/9;
            float maxY =19f-newFieldOfView;

            float moveSpeed = 20f;
            float horizontalMovement = Input.GetAxis("Mouse X");
            float verticalMovement = Input.GetAxis("Mouse Y");
            Transform transform = Camera.main.GetComponent<Transform>();

            Vector3 movement = new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime;
        
            float newX = Mathf.Clamp(transform.position.x + movement.x, minX, maxX);
            float newY = Mathf.Clamp(transform.position.y + movement.y, minY, maxY);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }*/
        if(Input.touchCount == 1){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Moved){
                float minX =-11f+newFieldOfView*16/9;
                float minY =-8f+newFieldOfView;
                float maxX =16.5f-newFieldOfView*16/9;
                float maxY =19f-newFieldOfView;

                float moveSpeed = 6f;
                float horizontalMovement = Input.GetAxis("Mouse X");
                float verticalMovement = Input.GetAxis("Mouse Y");
                Transform transform = Camera.main.GetComponent<Transform>();

                Vector3 movement = new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime;
            
                float newX = Mathf.Clamp(transform.position.x + movement.x, minX, maxX);
                float newY = Mathf.Clamp(transform.position.y + movement.y, minY, maxY);

                transform.position = new Vector3(newX, newY, transform.position.z);
            }
            else if(touch.phase == TouchPhase.Began){
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
            }
        }
        else if(Input.touchCount == 2){
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrevPosDifference = (firstTouchPrevPos-secondTouchPrevPos).magnitude;
            touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;
             
            zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude *zoomModifierSpeed;
            if(touchesPrevPosDifference> touchesCurPosDifference){
                Camera.main.orthographicSize += zoomModifier;
            }
            if(touchesPrevPosDifference < touchesCurPosDifference){
                Camera.main.orthographicSize -= zoomModifier;
            }
        }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 4f, 7f);
    }
    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;
    float zoomModifierSpeed = 0.01f;

    public void RandomizeMap(){
        if(panelIsActive){
            return;
        }
        for(int i = 0; i < 7; i++){
            for(int j = 0; j < 7; j++){
                int randomNumber = Random.Range(0, 4);
                grid.SetValue(i, j, randomNumber);
            }
        }
        xPlayer = Random.Range(0, 7);
        yPlayer = Random.Range(0, 7);
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null) {
            Transform playerTransform = playerObject.GetComponent<Transform>();
            //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
            playerTransform.position = new Vector3(-5.5f + xPlayer*3, -1.5f + yPlayer*3, 0f);
        }
        xTreasure = Random.Range(0, 7);
        yTreasure = Random.Range(0, 7);
        GameObject treasureObject = GameObject.Find("Treasure");
        if (treasureObject != null) {
            Transform treasureTransform = treasureObject.GetComponent<Transform>();
            //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
            treasureTransform.position = new Vector3(-5.5f + xTreasure*3, -1.5f + yTreasure*3, 0f);
        }
    }

    public void LoadMap(){
        if(panelIsActive){
            return;
        }
        List<TMPro.TMP_Dropdown.OptionData> options = listamapa.options;
        if(File.Exists(Application.persistentDataPath + "/" + options[listamapa.value].text + ".txt")){

            using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + options[listamapa.value].text + ".txt")){ // create a new StreamReader to read from the file

            string line;
            string[] parts;
            //ucitavanje polja mape
            for (int i = 0; i < 7; i++)
            {
                line = reader.ReadLine(); // read a line from the file
                parts = line.Split(','); // split the line into an array of strings
                for (int j = 0; j < 7; j++)
                {
                    //loadedMatrix[i, j] = int.Parse(parts[j]); // convert each string to an integer and store it in the new array
                    grid.SetValue(i, j, int.Parse(parts[j]));
                }
            }
            //ucitavanje pozicije igraca
            line = reader.ReadLine();
            parts = line.Split(',');
            xPlayer = int.Parse(parts[0]);
            yPlayer = int.Parse(parts[1]);
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null) {
                Transform playerTransform = playerObject.GetComponent<Transform>();
                //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
                playerTransform.position = new Vector3(-5.5f + xPlayer*3, -1.5f + yPlayer*3, 0f);
            }
            //ucitavanje pozicije blaga
            line = reader.ReadLine();
            parts = line.Split(',');
            xTreasure = int.Parse(parts[0]);
            yTreasure = int.Parse(parts[1]);
            GameObject treasureObject = GameObject.Find("Treasure");
            if (treasureObject != null) {
                Transform treasureTransform = treasureObject.GetComponent<Transform>();
                //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
                treasureTransform.position = new Vector3(-5.5f + xTreasure*3, -1.5f + yTreasure*3, 0f);
            }
            }   
        }
    }

    public void SaveMap(){
        if(panelIsActive){
            return;
        }
        //provera da li je default mapa
        if(listamapa.value == 0){
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Ne moze se sacuvati mapa u Default mapu!";
            return;
        }
        //provera da li se igrac i blago nalaze na istom polju
        if(xPlayer == xTreasure && yPlayer == yTreasure){
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Mapa se ne moze sacuvati jer su igrac i blago na istoj poziciji!";
            return;
        }
        //provera da li je igrac na lavi
        if(grid.gridArray[xPlayer, yPlayer] == 3){
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Mapa se ne moze sacuvati jer je igrac na polju lava!";
            return;
        }
        //provera da li je blago na lavi
        if(grid.gridArray[xTreasure, yTreasure] == 3){
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Mapa se ne moze sacuvati jer je blago na polju lava!";
            return;
        }
        //provera da li postoji put od blaga do lave
        if(ProveraPostojanjaPuta(xPlayer, yPlayer, xTreasure, yTreasure)){//ima puta
            pomocnaMatrica = new int[7,7];
            string matrix = "";
            for(int i = 0; i<7;i++){
                for(int j = 0; j<7; j++){
                    matrix += grid.gridArray[i,j].ToString() + ",";
                }
                matrix += "\n";
            }
            //upisujemo poziciju igraca
            matrix += xPlayer.ToString() + "," + yPlayer.ToString() + "\n";
            //upisujemo poziciju blaga
            matrix += xTreasure.ToString() + "," + yTreasure.ToString();

            //trazimo naziv mape
            List<TMPro.TMP_Dropdown.OptionData> options = listamapa.options;
            File.WriteAllText(Application.persistentDataPath + "/" + options[listamapa.value].text + ".txt", matrix);
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Uspesno sacuvana mapa!";
            return;
        }
        else{//nema puta
            pomocnaMatrica = new int[7,7];
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Mapa se ne moze sacuvati jer ne postoji put do blaga!";
        }
    }

    public void ResetMap(){
        if(panelIsActive){
            return;
        }
        for(int i = 0; i<7; i++){
            for(int j = 0; j < 7; j++){
                grid.SetValue(i, j, 0);
            }
        }
        xPlayer = 0;
        yPlayer = 0;
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null) {
            Transform playerTransform = playerObject.GetComponent<Transform>();
            //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
            playerTransform.position = new Vector3(-5.5f + xPlayer*3, -1.5f + yPlayer*3, 0f);
        }
        xTreasure = 1;
        yTreasure = 1;
        GameObject treasureObject = GameObject.Find("Treasure");
        if (treasureObject != null) {
            Transform treasureTransform = treasureObject.GetComponent<Transform>();
            //playerTransform.position = new Vector3(-5.5f + ((float)koordinataXigraca)*3.0f, -1.5 + ((float)koordinataYigraca)*3.0f, 0f);
            treasureTransform.position = new Vector3(-5.5f + xTreasure*3, -1.5f + yTreasure*3, 0f);
        }
    }

    public void NewMap(){
        if(panelIsActive){
            return;
        }
        string newMapName = inputFieldMapName.text;
        //provera imena fajla da li vec postoji
        using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/listamapa.txt")){
            string line;

            while((line = reader.ReadLine()) != null){
                if(line.Equals(newMapName)){
                    panelObject.SetActive(true);
                    panelIsActive = true;
                    panelText.text = "Mapa sa tim imenom vec postoji!";
                    return;
                }
            }
        }
        //pravljenje novog fajla koji ima sve jedinice i polozaje igraca i blaga
        using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/" + newMapName + ".txt")){
            string linija1 = "0,0,0,0,0,0,0\n";
            string linija2 = "0,0\n";
            string linija3 = "1,1";
            for(int i = 0; i < 7; i++){
                writer.Write(linija1);
            }
            writer.Write(linija2);
            writer.Write(linija3);
        }
        //dodavanje nove mape u listu
        using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/listamapa.txt", true)){
            writer.WriteLine(newMapName);
        }
        //dodavanje nove mape u dropdown
        TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
        option.text = newMapName;
        listamapa.options.Add(option);
    }

    public void DeleteMap(){
        if(panelIsActive){
            return;
        }
        if(listamapa.value == 0){
            panelObject.SetActive(true);
            panelIsActive = true;
            panelText.text = "Ne moze da se obrise Default mapa!";
            return;
        }
        List<TMPro.TMP_Dropdown.OptionData> options = listamapa.options;
        string fileName = options[listamapa.value].text;

        //brisanje fajla
        File.Delete(Application.persistentDataPath + "/" + options[listamapa.value].text + ".txt");
        int indexToRemove = options.FindIndex(option => option.text == fileName);
        options.RemoveAt(indexToRemove);
        listamapa.options = options;

        //brisanje imena fajla iz fajla sa imenima mapa
        string path = Application.persistentDataPath + "/listamapa.txt";
        string[] lines = System.IO.File.ReadAllLines(path);
        List<string> updatedLines = new List<string>();

        foreach (string line in lines) {
            if (!line.Contains(fileName)) {
                updatedLines.Add(line);
            }
        }

        System.IO.File.WriteAllLines(path, updatedLines.ToArray());
    }

    public void ChangePlayerPosition(){
        int newX = int.Parse(inputFieldXPlayer.text);
        int newY = int.Parse(inputFieldYPlayer.text);
        if(newX>=0 && newX<7 && newY>=0 && newY<7){
            xPlayer = newX;
            yPlayer = newY;
            GameObject playerObject = GameObject.Find("Player");
            Transform playerTransform = playerObject.GetComponent<Transform>();
            playerTransform.position = new Vector3(-5.5f + xPlayer*3, -1.5f + yPlayer*3, 0f);
        }
    }

    public void ChangeTreasurePosition(){
        int newX = int.Parse(inputFieldXTreasure.text);
        int newY = int.Parse(inputFieldYTreasure.text);
        if(newX>=0 && newX<7 && newY>=0 && newY<7){
            xTreasure = newX;
            yTreasure = newY;
            GameObject playerObject = GameObject.Find("Treasure");
            Transform playerTransform = playerObject.GetComponent<Transform>();
            playerTransform.position = new Vector3(-5.5f + xTreasure*3, -1.5f + yTreasure*3, 0f);
        }
    }
    
    public void Back(){
        if(panelIsActive){
            return;
        }
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ClosePanel(){
        panelIsActive = false;
        panelObject.SetActive(false);
    }


    private int[,] pomocnaMatrica = new int[7,7];
    public bool ProveraPostojanjaPuta(int startX, int startY, int endX, int endY){
        if(startX == endX && startY == endY){
            return true;
        }
        if(startX < 0 || startX>6 || startY < 0 || startY > 6){
            return false;
        }
        if(grid.gridArray[startX, startY] == 3){
            return false;
        }
        if(pomocnaMatrica[startX, startY] == 1){
            return false;
        }
        pomocnaMatrica[startX, startY] = 1;

        bool result = ProveraPostojanjaPuta(startX + 1, startY, endX, endY) ||
            ProveraPostojanjaPuta(startX - 1, startY, endX, endY) ||
            ProveraPostojanjaPuta(startX, startY - 1, endX, endY) ||
            ProveraPostojanjaPuta(startX, startY + 1, endX, endY);
        
        //pomocnaMatrica[startX, startY] = 0;

        return result;
    }
}
