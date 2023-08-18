using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Player2Script : MonoBehaviour
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
        GameObject player = GameObject.Find("Player2");
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
            startX = int.Parse(parts[0]);
            startY = int.Parse(parts[1]);
        }
        using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/treasure.txt")){
            string line = reader.ReadLine(); // read a line from the file
            string[] parts = line.Split(','); // split the line into an array of strings
            endX = int.Parse(parts[0]);
            endY = int.Parse(parts[1]);
        }

        //FindPathDFS();

        /*foreach (Vector2Int vector in finalPath) {
            Debug.Log(vector.x + ", " + vector.y);
        }*/
        //finalPath.RemoveAt(0);
        //Vector2Int next = finalPath[0];
        //finalPath.RemoveAt(0);

        //MoveTo(new Vector3(-5.5f+next.x*3, -1.5f + next.y*3, 0f));

        finalPath = FindShortestPath();
        //Node node = finalPath[0];
        //MoveTo(new Vector3(-5.5f+node.x*3, -1.5f + node.y*3, 0f));
        
        //Debug.Log(finalPath.Count);
        //foreach (Node vector in finalPath) {
            //Debug.Log(vector.x + ", " + vector.y);
        //}
    }
    public List<Node> finalPath;

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
                Node next = finalPath[0];
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

    //kod za pronalazenje najkraceg puta
    static int startX = 1; // pozicija početka
    static int startY = 2;

    static int endX = 3; // pozicija kraja
    static int endY = 3;

    public int[] dx = { 0, 0, 1, -1 }; // pomeraj po x osi za susedna polja
    public int[] dy = { 1, -1, 0, 0 }; // pomeraj po y osi za susedna polja

    public bool IsValid(int x, int y)
    {
        return x >= 0 && x < 7 && y >= 0 && y < 7 && field[x, y] != -1;
    }

    public int[,] field = new int[7,7];

    public Node[,] mapa = new Node[7,7];


    public List<Node> FindShortestPath()
    {
        for(int i = 0; i < 7; i++){
            for(int j = 0; j < 7; j++){
                if(grid.gridArray[i,j] == 0){
                    field[i,j] = 1;
                }
                else if(grid.gridArray[i,j] == 1){
                    field[i,j] = 2;
                }
                else if(grid.gridArray[i,j] == 2){
                    field[i,j] = 5;
                }
                if(grid.gridArray[i,j] == 3){
                    field[i,j] = -1;
                }
            }
        }

        int[,] distances = new int[field.GetLength(0), field.GetLength(1)];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                distances[i, j] = int.MaxValue;
                mapa[i,j] = new Node(0,0,0);
            }
        }

        distances[startX, startY] = field[startX, startY];

        //SortedSet<Node> pq = new SortedSet<Node>(Comparer<Node>.Create((n1, n2) => n1.distance.CompareTo(n2.distance)));
        List<Node> pq = new List<Node>();
        pq.Add(new Node(startX, startY, distances[startX, startY]));
        mapa[startX, startY].x = startX;
        mapa[startX, startY].y = startY;
        mapa[startX,startY].prevNode = null;

        while (pq.Count > 0)
        {

            Node node = pq[0];
            pq.RemoveAt(0);
            Debug.Log(node.x + " " + node.y);
            for (int i = 0; i < dx.Length; i++)
            {
                int nx = node.x + dx[i];
                int ny = node.y + dy[i];

                if (IsValid(nx, ny))
                {
                    int newDistance = node.distance + field[nx, ny];
                    if (newDistance < distances[nx, ny])
                    {
                        if(nx == endX && ny == endY) Debug.Log("Usao  ovde");
                        distances[nx, ny] = newDistance;
                        Node newNode = new Node(nx, ny, newDistance);
                        newNode.prevNode = node; // dodaj prethodnika
                        mapa[nx, ny].x = nx;
                        mapa[nx, ny].y = ny;
                        mapa[nx, ny].prevNode = mapa[node.x, node.y];

                        pq.Add(newNode);
                    }
                }
            }
        }
        Debug.Log(distances[endX, endY]);

        List<Node> shortestPath = new List<Node>();
        Node currentNode = mapa[endX, endY];

        while (currentNode!=null)
        {
            Debug.Log("kkk");
            shortestPath.Insert(0, currentNode);
            currentNode = currentNode.prevNode;
        }
        shortestPath.RemoveAt(0);
        return shortestPath;
    }

}

public class Node
{
    public int x;
    public int y;
    public int distance;
    public Node prevNode; // čvor prethodnika

    public Node(int x, int y, int distance)
    {
        this.x = x;
        this.y = y;
        this.distance = distance;
        this.prevNode = null;
    }
}