using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Player1Script : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public Animator animator;
    public bool radi = false;
    private Vector3 targetPosition;
    public bool isMoving;
    private Transform playerTransform;
    private Grid grid;
    public Sprite redSquare;
    public Sprite greenSquare;
    public Sprite yellowSquare;
    public Sprite blueSquare;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject player = GameObject.Find("Player1");
        playerTransform = player.transform;

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

            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings
            
            playerTransform.position = new Vector3(-5.5f + float.Parse(parts[0])*3, -1.5f + float.Parse(parts[1])*3, 0f);
        }

        FindPathDFS();

        /*foreach (Vector2Int vector in finalPath) {
            Debug.Log(vector.x + ", " + vector.y);
        }*/
        finalPath.RemoveAt(0);
        //Vector2Int next = finalPath[0];
        //finalPath.RemoveAt(0);

        //MoveTo(new Vector3(-5.5f+next.x*3, -1.5f + next.y*3, 0f));
    }

    public int pauza = 0;

    // Update is called once per frame
    void Update()
    {   
        if(radi){
            if(isMoving){
                Vector3 direction = targetPosition - playerTransform.position;
                direction.Normalize();

                Vector3 nextPosition = playerTransform.position + direction * moveSpeed * Time.deltaTime;
            
                playerTransform.position = nextPosition;

                if(Vector3.Distance(playerTransform.position, targetPosition) < 0.1f){
                    animator.SetBool("Walk", false);
                    isMoving = false;
                    pauza=1;
                }
            }
            else if(pauza>0){
                pauza++;
                if(pauza > 15) pauza=0;
            }
            else if(finalPath.Count>0){
                Vector2Int next = finalPath[0];
                finalPath.RemoveAt(0);

                MoveTo(new Vector3(-5.5f+next.x*3, -1.5f + next.y*3, 0f));
            }
        }
    }

    public void MoveTo(Vector3 position){
        targetPosition = position;
        isMoving = true;
        if(playerTransform.position.x > position.x){
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
        }
        else if(playerTransform.position.x <= position.x){
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = false;
        }

        animator.SetBool("Walk", true);
    }



    //KOD ZA PRONALAZENJE PUTA

    public int[,] matrix;
    public Vector2Int target;
    private List<Vector2Int> visited;
    private List<Vector2Int> currentPath;
    private List<Vector2Int> finalPath;

    public void FindPathDFS(){
        matrix = grid.gridArray;
        visited = new List<Vector2Int>();
        currentPath = new List<Vector2Int>();
        finalPath = new List<Vector2Int>();

        if(File.Exists(Application.persistentDataPath + "/treasure.txt")){

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/treasure.txt"); // create a new StreamReader to read from the file

            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings
            target = new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        if(File.Exists(Application.persistentDataPath + "/player.txt")){

            StreamReader reader = new StreamReader(Application.persistentDataPath + "/player.txt"); // create a new StreamReader to read from the file

            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings
            DFS(new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1])));
        }
    }

    private void DFS(Vector2Int current) {
        visited.Add(current);
        currentPath.Add(current);
        if (current == target) {
            finalPath = new List<Vector2Int>(currentPath);
        }
        else {
            foreach (Vector2Int neighbor in GetNeighbors(current)) {
                if (!visited.Contains(neighbor)) {
                    DFS(neighbor);
                }
            }
        }
        currentPath.Remove(current);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int x = node.x;
        int y = node.y;
        if (x > 0 && matrix[x - 1, y] != 3) {
            neighbors.Add(new Vector2Int(x - 1, y));
        }
        if (x < matrix.GetLength(0) - 1 && matrix[x + 1, y] != 3) {
            neighbors.Add(new Vector2Int(x + 1, y));
        }
        if (y > 0 && matrix[x, y - 1] != 3) {
            neighbors.Add(new Vector2Int(x, y - 1));
        }
        if (y < matrix.GetLength(1) - 1 && matrix[x, y + 1] != 3) {
            neighbors.Add(new Vector2Int(x, y + 1));
        }
        return neighbors;
    }

    


}
