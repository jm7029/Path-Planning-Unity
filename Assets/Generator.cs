using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class Generator : MonoBehaviour
{
    public TMP_InputField width_Input;
    public TMP_InputField height_Input;
    public TMP_InputField startX_Input;
    public TMP_InputField startY_Input;
    public TMP_InputField goalX_Input;
    public TMP_InputField goalY_Input;

    public TMP_Dropdown dropdown;
    public TMP_Text Step;
    public static string method;

    private List<List<int>> map = new List<List<int>>();
    private List<List<List<bool>>> graph = new List<List<List<bool>>>();
    private List<NodeScript> nodes = new List<NodeScript>();

    private GameObject[] tiles;
    private int width;
    private int height;
    private int goalX;
    private int goalY;
    private int startX;
    private int startY;
    private int cur_X;
    private int cur_Y;
    public Queue<Vector2> path_ = new Queue<Vector2>();
    public GameObject tile;
    public GameObject person;
    public GameObject check;

    // Start is called before the first frame update


    public void Generate()
    {

        map.Clear();
        graph.Clear();
        nodes.Clear();
        try
        {
            GameObject[] tiles_ = GameObject.FindGameObjectsWithTag("Tile");
            GameObject player_ = GameObject.FindGameObjectWithTag("Player");

            foreach (GameObject tile in tiles_)
            {
                Destroy(tile);
            }
            Destroy(player_);
        }
        catch
        {

        }        
        width = int.Parse(width_Input.text);
        height = int.Parse(height_Input.text);

        goalX = int.Parse(goalX_Input.text);
        goalY = int.Parse(goalY_Input.text);

        startX = int.Parse(startX_Input.text);
        startY = int.Parse(startY_Input.text);
        
        for (int x = 0; x<width; x++)
        {
            List<int> col = new List<int>();
            for (int y = 0; y<height; y++)
            {
                Vector3 position = new Vector3(x * 5, 0, y*(-5));
                GameObject tile_ = Instantiate(tile, position, Quaternion.identity);
                NodeScript node = tile_.GetComponent<NodeScript>();
                if (x == goalX && y == goalY) // 골 지점 표시
                {
                    node.setStates(x, y, 3);
                    col.Add(3);
                }
                else if (x == startX && y == startY)
                {
                    node.setStates(x, y, 2); //  시작 지점 표시
                    col.Add(2);
                }
                else
                {
                    int random = UnityEngine.Random.Range(0,5);
                    if (random == 0)
                    {
                        node.setStates(x, y, 1); //  벽 표시
                        col.Add(1);

                    }
                    else if(random == 1)
                    {
                        node.setStates(x, y, 4); //  장애물 표시
                        col.Add(4);
                    }
                    else
                    {
                        node.setStates(x, y, 0);
                        col.Add(0);
                    }
                }
                tile_.SetActive(true);
            }
            map.Add(col);
        }
        Vector3 pos = new Vector3(startX * 5, 1f, startY * (-5));
        Instantiate(person, pos,Quaternion.identity);
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        nodes = new List<NodeScript>();
        foreach(GameObject tile in tiles)
        {
            NodeScript node = tile.GetComponent<NodeScript>();
            nodes.Add(node);
            if(node.ReturnState()[0]==startX && node.ReturnState()[1]== startY)
            {
                node.setStates(startX, startY, 2);
            }
            else if(node.ReturnState()[0] == goalX && node.ReturnState()[1] == goalY)
            {
                node.setStates(goalX, goalY, 3);
            }
        }
        MakeGraph(map);
        method = dropdown.options[dropdown.value].text;
        if (method == "FloodFill")
        {
            path_ = FloodFill();
        }
        else if (method == "Dijkstra")
        {
            path_ = Dijkstra();

        }
        else
        {
            path_ = Astar();
        }
    }

    public void SelectMethod()
    {
        method = dropdown.options[dropdown.value].text;
        if(method == "FloodFill")
        {
            path_ = FloodFill();
        }
        else if(method == "Dijkstra")
        {
            path_ = Dijkstra();
        }
        else
        {
            path_ = Astar();
        }
    }
    public void PlayerMove()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float timer = 0;

        Vector2 input = path_.Dequeue();

        player.transform.position = new Vector3(input.x * 5, player.transform.position.y, (input.y * -5));


    }



    public Queue<Vector2> FloodFill()
    {
        try
        {
            GameObject[] checks = GameObject.FindGameObjectsWithTag("Check");
            foreach(GameObject check in checks)
            {
                Destroy(check);
            }
        }
        catch
        {

        }
        Dictionary<Vector2, Vector2> nexTiletoGoal = new Dictionary<Vector2, Vector2>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Queue<Vector2> frontier = new Queue<Vector2>();
        Vector2 startTile = new Vector2(startX, startY);
        Vector2 goalTile = new Vector2(goalX, goalY);
        List<Vector2> visitied = new List<Vector2>();
        frontier.Enqueue(goalTile);
  
        int dir = 0;
        int i = 0;
        while (frontier.Count > 0 && visitied.Contains(startTile) == false)
        {
            Debug.Log(i);
            Vector2 cur_Pos = frontier.Dequeue();
            Vector2 neighbor_pos = new Vector2();

            foreach (bool neighbor in graph[(int)cur_Pos.x][(int)cur_Pos.y])
            {
                if (neighbor)
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
                    if (visitied.Contains(neighbor_pos) == false && frontier.Contains(neighbor_pos) == false)
                    {
                        frontier.Enqueue(neighbor_pos);
                  
                        nexTiletoGoal[neighbor_pos] = cur_Pos;
                        
                    }
                }
                dir++;
            }
            dir = 0;
            Instantiate(check, new Vector3(cur_Pos.x * 5, 0.9f, cur_Pos.y * -5), Quaternion.identity);
            visitied.Add(cur_Pos);
        }
        Queue<Vector2> path = new Queue<Vector2>();
        Vector2 curPathTile = startTile;
        
        while (curPathTile != goalTile)
        {
            curPathTile = nexTiletoGoal[curPathTile];
            path.Enqueue(curPathTile);
        }
        Step.text = "Total Step : " + (path.Count).ToString();
        return path;
    }
    private void MakeGraph(List<List<int>> map)
    {
        graph.Clear(); 

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
                    if (map[x][y - 1] != 1)
                    {
                        top_link = true;
                    }
                }
                if (x != width - 1)
                {
                    if (map[x + 1][y] != 1)
                    {
                        right_link = true;
                    }
                }
                if (y != height - 1)
                {
                    if (map[x][y + 1] != 1)
                    {
                        bottom_link = true;
                    }
                }
                if (x != 0)
                {
                    if (map[x-1][y] != 1)
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
            graph.Add(links);
        }
    }

    public Queue<Vector2> Dijkstra()
    {
        try
        {
            GameObject[] checks = GameObject.FindGameObjectsWithTag("Check");
            foreach (GameObject check in checks)
            {
                Destroy(check);
            }
        }
        catch
        {

        }
        Dictionary<Vector2, Vector2> NextTileToGoal = new Dictionary<Vector2, Vector2>();//Determines for each tile where you need to go to reach the goal. Key=Tile, Value=Direction to Goal
        Dictionary<Vector2, int> costToReachTile = new Dictionary<Vector2, int>();//Total Movement Cost to reach the tile

        PriorityQueue<Vector2> frontier = new PriorityQueue<Vector2>();
        Vector2 startTile = new Vector2(startX, startY);
        Vector2 goalTile = new Vector2(goalX, goalY);
        frontier.Enqueue(goalTile, 0);
        costToReachTile.Add(goalTile, 0);
        int dir = 0;
        Vector2 neighbor_pos = new Vector2();
        while(frontier.Count > 0)
        {
            Vector2 cur_Pos = frontier.Dequeue();
            Instantiate(check, new Vector3(cur_Pos.x * 5, 0.9f, cur_Pos.y * -5), Quaternion.identity);

            if (cur_Pos == startTile)
                break;
            foreach (bool neighbor in graph[(int)cur_Pos.x][(int)cur_Pos.y])
            {
                int newCost;
                if (map[(int)cur_Pos.x][(int)cur_Pos.y] == 4) 
                {
                    newCost = costToReachTile[cur_Pos] + 4;
                }
                else
                {
                    newCost = costToReachTile[cur_Pos] + 1;
                }
                if (neighbor)
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


                    if (costToReachTile.ContainsKey(neighbor_pos) == false || newCost < costToReachTile[neighbor_pos])
                    {
                        costToReachTile[neighbor_pos] = newCost;
                        int priority = newCost;
                        frontier.Enqueue(neighbor_pos, priority);
                        NextTileToGoal[neighbor_pos] = cur_Pos;
                    }
                }
                dir++;
            }
            dir = 0;
        }
        Queue<Vector2> path = new Queue<Vector2>();
        Vector2 pathTile = startTile;
        while (goalTile != pathTile)
        {
            pathTile = NextTileToGoal[pathTile];
            path.Enqueue(pathTile);
        }
        Step.text = "Total Step : " + (path.Count).ToString();

        return path;
    }

    private Queue<Vector2> Astar()
    {
        try
        {
            GameObject[] checks = GameObject.FindGameObjectsWithTag("Check");
            foreach (GameObject check in checks)
            {
                Destroy(check);
            }
        }
        catch
        {

        }
        Dictionary<Vector2, Vector2> NextTileToGoal = new Dictionary<Vector2, Vector2>();//Determines for each tile where you need to go to reach the goal. Key=Tile, Value=Direction to Goal
        Dictionary<Vector2, int> costToReachTile = new Dictionary<Vector2, int>();//Total Movement Cost to reach the tile
        PriorityQueue<Vector2> frontier = new PriorityQueue<Vector2>();
        Vector2 startTile = new Vector2(startX, startY);
        Vector2 goalTile = new Vector2(goalX, goalY);
        frontier.Enqueue(goalTile, 0);
        costToReachTile[goalTile] = 0;
        Vector2 neighbor_pos = new Vector2();
        int dir = 0;
        while (frontier.Count > 0 )
        {
            Vector2 cur_Pos = frontier.Dequeue();
            Instantiate(check, new Vector3(cur_Pos.x * 5, 0.9f, cur_Pos.y * -5), Quaternion.identity);

            Debug.Log(cur_Pos);
            if (cur_Pos == startTile)
            {
                break;
            }
            foreach (bool neighbor in graph[(int)cur_Pos.x][(int)cur_Pos.y])
            {
                int newCost;

                if (neighbor)
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

                    if (map[(int)neighbor_pos.x][(int)neighbor_pos.y] == 4)
                    {
                        newCost = costToReachTile[cur_Pos] + 4;
                    }
                    else
                    {
                        newCost = costToReachTile[cur_Pos] + 1;
                    }

                    if (costToReachTile.ContainsKey(neighbor_pos) == false || newCost < costToReachTile[neighbor_pos])
                    {

                        costToReachTile[neighbor_pos] = newCost;
                        int priortiy = newCost + 2*Distance(neighbor_pos, startTile);
                        frontier.Enqueue(neighbor_pos, priortiy);
                        NextTileToGoal[neighbor_pos] = cur_Pos;

                    }
                }
                dir++;

            }
            dir = 0;


        }

        Queue<Vector2> path = new Queue<Vector2>();
        Vector2 pathTile = startTile;

        while (goalTile!= pathTile)
        {
            pathTile = NextTileToGoal[pathTile];
            path.Enqueue(pathTile);
        }
        Step.text = "Total Step : " + (path.Count).ToString();

        return path;
    }
    int Distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.RoundToInt(Vector2.Distance(pos1,pos2));
    }

    //public Queue<Vector2> Astar()
    //{
    //    List<List<int>> DistanceMap = new List<List<int>>();

    //}
    void Update()
    {
        
    }
}
