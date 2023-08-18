using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Sprite redSquare;
    public Sprite greenSquare;
    public Sprite yellowSquare;
    public Sprite blueSquare;
    private Grid grid;
    public Button startButton;
    public Button pauseButton;
    public Button restartButton;

    public GameObject player0;
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start()
    {
        using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/playerNumber.txt")){
            int line = int.Parse(reader.ReadLine());
            if(line == 0){
                player0.SetActive(true);
            }
            else if(line == 1){
                player1.SetActive(true);
            }
            else{
                player2.SetActive(true);
            }
        }


        pauseButton.interactable = false;
        restartButton.interactable = false;

        GameObject objekat = GameObject.FindWithTag("Treasure");

        if(File.Exists(Application.persistentDataPath + "/treasure.txt")){

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/treasure.txt"); // create a new StreamReader to read from the file

            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings

            objekat.transform.position = new Vector3(-5.5f + float.Parse(parts[0])*3, -1.5f + float.Parse(parts[1])*3, 0f);

        }
    }

    // Update is called once per frame
    void Update()
    {
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
        //Camera.main.orthographicSize = newFieldOfView;

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
            //transform.Translate(new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime);

            Vector3 movement = new Vector3(-horizontalMovement, -verticalMovement, 0) * moveSpeed * Time.deltaTime;
        
            float newX = Mathf.Clamp(transform.position.x + movement.x, minX, maxX);
            float newY = Mathf.Clamp(transform.position.y + movement.y, minY, maxY);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }*/
    }
    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;
    float zoomModifierSpeed = 0.01f;
    public void ButtonStart(){
        GameObject objekat = GameObject.FindWithTag("Player");
        Player1Script skripta1 = objekat.GetComponent<Player1Script>();
        if(skripta1){
            skripta1.radi=true;
            skripta1.animator.SetBool("Walk", true);
        }
        Player2Script skripta2 = objekat.GetComponent<Player2Script>();
        if(skripta2){
            skripta2.radi=true;
            skripta2.animator.SetBool("Walk", true);
        }
        Player3Script skripta3 = objekat.GetComponent<Player3Script>();
        if(skripta3){
            skripta3.radi=true;
            skripta3.animator.SetBool("Walk", true);
        }
        startButton.interactable = false;
        pauseButton.interactable = true;
        restartButton.interactable = true;
    }

    public void ButtonPause(){
        GameObject objekat = GameObject.FindWithTag("Player");
        Player1Script skripta1 = objekat.GetComponent<Player1Script>();
        if(skripta1){
            skripta1.radi=false;
            skripta1.animator.SetBool("Walk", false);
        }
        Player2Script skripta2 = objekat.GetComponent<Player2Script>();
        if(skripta2){
            skripta2.radi=false;
            skripta2.animator.SetBool("Walk", false);
        }
        Player3Script skripta3 = objekat.GetComponent<Player3Script>();
        if(skripta3){
            skripta3.radi=false;
            skripta3.animator.SetBool("Walk", false);
        }
        startButton.interactable = true;
        pauseButton.interactable = false;
    }

    public void ButtonRestart(){
        startButton.interactable = true;
        restartButton.interactable = false;
        pauseButton.interactable = false;
        GameObject objekat = GameObject.FindWithTag("Player");
        if(File.Exists(Application.persistentDataPath + "/player.txt")){

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/player.txt"); // create a new StreamReader to read from the file

            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings
            
            objekat.transform.position = new Vector3(-5.5f + float.Parse(parts[0])*3, -1.5f + float.Parse(parts[1])*3, 0f);
        }
        Player1Script skripta1 = objekat.GetComponent<Player1Script>();
        if(skripta1){
            skripta1.radi=false;
            skripta1.FindPathDFS();
            skripta1.isMoving = false;
            skripta1.pauza = 0;
            skripta1.animator.SetBool("Walk", false);
        }
        Player2Script skripta2 = objekat.GetComponent<Player2Script>();
        if(skripta2){
            skripta2.radi=false;
            skripta2.finalPath = skripta2.FindShortestPath();
            skripta2.isMoving = false;
            skripta2.pauza = 0;
            skripta2.animator.SetBool("Walk", false);
        }
        Player3Script skripta3 = objekat.GetComponent<Player3Script>();
        if(skripta3){
            skripta3.radi= false;
            skripta3.finalPath = skripta3.BFS();
            skripta3.isMoving = false;
            skripta3.pauza = 0;
            skripta3.animator.SetBool("Walk", false);
        }

    }

    public void ButtonBack(){
        SceneManager.LoadScene("MainMenuScene");
    }
}
