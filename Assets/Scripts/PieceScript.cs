using UnityEngine;

public class PieceScript : MonoBehaviour
{
    // Стартовая позиция
    Vector3 startPos;

    public int playerID;
    public int cellIndex;
    public GameObject gc;
    public bool isChosen;

    Material material;
    public Material highlightMaterial;
    public Material weakHighlightMaterial;

    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        gc = GameObject.FindWithTag("GameController");
        isChosen = false;
        cellIndex = -1;
        material = GetComponent<Renderer>().material;
    }

    public void ChangeMaterial()
    {
        if (!isChosen)
            GetComponent<Renderer>().material = highlightMaterial;
        else
            GetComponent<Renderer>().material = material;

        isChosen = !isChosen;
    }

    public void GoToStartPosition()
    {
        transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
    }

    //void OnMouseEnter()
    //{
    //    if (!isChosen)
    //        GetComponent<Renderer>().material = weakHighlightMaterial;
    //}
    //void OnMouseExit()
    //{
    //    if (!isChosen)
    //        GetComponent<Renderer>().material = material;
    //}
}
