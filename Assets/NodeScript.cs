using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    private Material Mat; 
    public int x;
    public int y;
    public int state;
    // Start is called before the first frame update

    private void Start()
    {

    }
    public List<int> ReturnState()
    {
        List<int> states = new List<int>();
        states.Add(x);
        states.Add(y);
        states.Add(state);
        return states;
    }
    public void setStates(int x, int y, int state)
    {
        Mat = GetComponent<MeshRenderer>().material;
        this.x = x;
        this.y = y;
        this.state = state;
        switch (state)
        {
            case 0:
                Mat.color = new Color(255, 255, 255);
                break;
            case 1:
                Mat.color = new Color(0, 0, 0);
                break;
            case 2:
                Mat.color = new Color(0, 255, 0);
                break;
            case 3:
                Mat.color = new Color(0, 0,255);
                break;
            case 4:
                Mat.color = new Color(255, 255, 0);
                break;
        }
    }

  
    private void OnCollisionEnter(Collision collision)
    {
        if (Generator.method == "FloodFill" && (state==0||state==4))
        {
            Mat.color = new Color(0,255,255);
        }
        else if(Generator.method == "Dijkstra" && (state == 0 || state == 4))
        {
            Mat.color = new Color(255, 0, 255);
        }
        else if((state == 0 || state == 4))
        {
            Mat.color = new Color(255, 0, 0);
        }
    }
    // Update is called once per frame
}
