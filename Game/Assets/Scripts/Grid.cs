using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CodeMonkey.Utils;

public class Grid
{   
    private int width;
    private int height;
    public int[,] gridArray;
    float cellSize;
    private TextMesh[,] debugTextArray;
    public Sprite red, green, yellow, blue;
    private GameObject[,] gridObjects;
    private Vector3 originPosition;

    public Grid(int width, int height, float cellSize, Sprite red, Sprite green, Sprite yellow, Sprite blue){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.red = red;
        this.green = green;
        this.yellow = yellow;
        this.blue = blue;

        originPosition = new Vector3(-5.5f,-1.5f);

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];
        gridObjects = new GameObject[width, height];

        //Debug.Log(width + " " + height);

        for(int x=0; x<gridArray.GetLength(0); x++){
            for(int y=0; y<gridArray.GetLength(1); y++){
                //debugTextArray[x,y]=UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x,y) + new Vector3(cellSize,cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                //Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y+1), Color.white, 100f);
                //Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1, y), Color.white, 100f);
                gridObjects[x, y] = new GameObject();
                gridObjects[x, y].AddComponent<SpriteRenderer>();
                gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = green;
                //SpriteRenderer sp = go.GetComponent<SpriteRenderer>();
                //Sprite s = sp.sprite;
                Transform transform = gridObjects[x, y].transform;
                transform.localPosition = new Vector3(x, y)*cellSize + originPosition;
            }
        }
        //Debug.DrawLine(GetWorldPosition(0,height), GetWorldPosition(width, height), Color.white, 100f);
        //Debug.DrawLine(GetWorldPosition(width,0), GetWorldPosition(width, height), Color.white, 100f);

        //SetValue(2,1,56);
    }

    private Vector3 GetWorldPosition(int x, int y){
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void SetValue(int x, int y, int value){
        if(x>=0 && y>=0 && x<width && y<height){
            gridArray[x, y] = value;
            if(value == 0){
                gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = green;
            }
            else if(value == 1){
                gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = blue;
            }
            else if(value == 2){
                gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = yellow;
            }
            else if(value == 3){
                gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = red;
            }
            //Debug.Log(x + ", " + y + "   = " + value);
        }
        //debugTextArray[x, y].text = gridArray[x,y].ToString();
    }
    
    public void ChangeField(int x, int y){
        gridArray[x, y] = (gridArray[x, y] + 1) % 4;
        int value  = gridArray[x, y];
        if(value == 0){
            gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = green;
        }
        else if(value == 1){
            gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = blue;
        }
        else if(value == 2){
            gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = yellow;
        }
        else if(value == 3){
            gridObjects[x, y].GetComponent<SpriteRenderer>().sprite = red;
        }
    }

    public int GetWidth(){
        return width;
    }

    public int GetHeight(){
        return height;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y){
        x = Mathf.FloorToInt((worldPosition - originPosition).x/cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y/cellSize);
    }
    
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
}
