using UnityEngine;

public class CellScript : MonoBehaviour
{
    public GameObject gc;
    public bool isChosen;
    public int index;

    public GameObject chip;
     
    Material material;
    public Material highlightMaterial;
    public Material weakHighlightMaterial;

    void Start()
    {
        gc = GameObject.FindWithTag("GameController");

        isChosen = false;

        material = GetComponent<Renderer>().material;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void ChangeMaterial()
    {
        if (!isChosen)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Renderer>().material = highlightMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = material;
            GetComponent<MeshRenderer>().enabled = false;
        }

        isChosen = !isChosen;
    }

    /*
    void OnMouseEnter()
    {
        if (!isChosen)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Renderer>().material = weakHighlightMaterial;
        }
    }
    void OnMouseExit()
    {
        if (!isChosen)
        {
            GetComponent<Renderer>().material = material;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
    */
}
