using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MapGen : MonoBehaviour
{
    public TMP_InputField width_Input;
    public TMP_InputField height_Input;
    public TMP_InputField startX_Input;
    public TMP_InputField startY_Input;
    public TMP_InputField goalX_Input;
    public TMP_InputField goalY_Input;

    private int width;
    private int height;
    private int goalX;
    private int goalY;
    private int startX;
    private int startY;

    public GameObject tile;

    private List<List<int>> stateMap = new List<List<int>>();
    private List<List<List<bool>>> directionMap = new List<List<List<bool>>>();
    
    
    public void Generate()
    {
        width = int.Parse(width_Input.text);
        height = int.Parse(height_Input.text);
        goalX = int.Parse(goalX_Input.text);
        goalY = int.Parse(goalY_Input.text);
        startX = int.Parse(startX_Input.text);
        startY = int.Parse(startY_Input.text);

        GameObject[] tiles_ = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in tiles_)
        {
            Destroy(tile);
        }
        stateMap.Clear();
        directionMap.Clear();

        for (int x = 0; x < width; x++)
        {
            List<int> col = new List<int>();
            for (int y = 0; y<height; y++)
            {
                Vector3 position = new Vector3(x * 5, 0, y * (-5));
                GameObject tile_ = Instantiate(tile, position, Quaternion.identity);
                NodeScript node = tile_.GetComponent<NodeScript>();
                if (x == goalX && y == goalY) // 골 지점 표시
                {
                    node.setStates(x, y, 3);
                    col.Add(3);
                } 
                else if (x == startX && y == startY) // 시작 지점 표시
                {
                    node.setStates(x, y, 2);
                    col.Add(2);
                }
                else
                {
                    int random = UnityEngine.Random.Range(0, 5);
                    if (random == 0)
                    {
                        node.setStates(x, y, 1); //  벽 표시
                        col.Add(1);

                    }
                    else if (random == 1)
                    {
                        node.setStates(x, y, 4); //  험한 길 표시
                        col.Add(4);
                    }
                    else
                    {
                        node.setStates(x, y, 0); // 길 표시
                        col.Add(0);
                    }
                }
                tile_.SetActive(true);
            }
            stateMap.Add(col);
        }
        MakeDirectionMap(stateMap);
       
    }

    private void MakeDirectionMap(List<List<int>> stateMap)
    {
        for (int x = 0; x < width; x++)
        {
            List<List<bool>> links = new List<List<bool>>();
            for (int y = 0; y < height; y++)
            {
                bool top_link = false;
                bool right_link = false;
                bool bottom_link = false;
                bool left_link = false;
                List<bool> link = new List<bool>();
                if (y != 0)
                {
                    if (stateMap[x][y - 1] != 1)
                    {
                        top_link = true;
                    }
                }
                if (x != width - 1)
                {
                    if (stateMap[x + 1][y] != 1)
                    {
                        right_link = true;
                    }
                }
                if (y != height - 1)
                {
                    if (stateMap[x][y + 1] != 1)
                    {
                        bottom_link = true;
                    }
                }
                if (x != 0)
                {
                    if (stateMap[x - 1][y] != 1)
                    {
                        left_link = true;
                    }
                }
                link.Add(top_link);
                link.Add(right_link);
                link.Add(bottom_link);
                link.Add(left_link);
                links.Add(link);
            }
            directionMap.Add(links);
        }
    }

    private Queue<Vector2> AStar()
    {
        Dictionary<Vector2, Vector2> NextTiles = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, int> closeList = new Dictionary<Vector2, int>();
        PriorityQueue<Vector2> openList = new PriorityQueue<Vector2>();
        Vector2 startTile = new Vector2(startX, startY);
        Vector2 goalTile = new Vector2(goalX, goalY);
        openList.Enqueue(goalTile, 0);
        closeList[goalTile] = 0;
        Vector2 neighbor_pos = new Vector2();


        while (openList.Count > 0)
        {
            int dir = 0;

            Vector2 cur_Pos = openList.Dequeue();
            if (cur_Pos == startTile)
            {
                break;
            }
            foreach (bool direction in directionMap[(int)cur_Pos.x][(int)cur_Pos.y])
            {
                int newCost;
                if (direction)
                {
                    if (dir == 0)
                    {
                        neighbor_pos = new Vector2(cur_Pos.x, cur_Pos.y - 1);
                    }
                    else if (dir == 1)
                    {
                        neighbor_pos = new Vector2(cur_Pos.x + 1, cur_Pos.y);
                    }
                    else if (dir == 2)
                    {
                        neighbor_pos = new Vector2(cur_Pos.x, cur_Pos.y + 1);
                    }
                    else
                    {
                        neighbor_pos = new Vector2(cur_Pos.x - 1, cur_Pos.y);
                    }
                }

                if (stateMap[(int)neighbor_pos.x][(int)neighbor_pos.y] == 4)
                {
                    newCost = closeList[cur_Pos] + 4;
                }
                else
                {
                    newCost = closeList[cur_Pos] + 1;
                }
                
                if (closeList.ContainsKey(neighbor_pos) == false || newCost < closeList[neighbor_pos])
                {

                    closeList[neighbor_pos] = newCost;
                    int priortiy = newCost + Mathf.RoundToInt(Vector2.Distance(neighbor_pos, startTile));
                    openList.Enqueue(neighbor_pos, priortiy);
                    NextTiles[neighbor_pos] = cur_Pos;
                }
                dir++;

            }
            dir = 0;

    }
        Queue<Vector2> path = new Queue<Vector2>();
        Vector2 pathTile = startTile;

        while (goalTile != pathTile)
        {
            pathTile = NextTiles[pathTile];
            path.Enqueue(pathTile);
        }

        return path;

    }



    // Update is called once per frame
}
//void Update()
//{
//    stateMap.Clear(); // 상태매트릭스 초기화
//    directionMap.Clear(); // 이동가능맵
//    nodes.Clear(); // NodeScrpit 리스트 초기화
//}