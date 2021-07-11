using System;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    public GameObject[] faces;

    public bool givesPoint;

    public Vector3 startPos;

    public GameObject cluster;

    public (bool, bool) IsStandingStraight()
    {
        if (Math.Round(faces[0].GetComponent<Transform>().position.y, 1) == Math.Round(faces[1].GetComponent<Transform>().position.y, 1)
            && Math.Round(faces[1].GetComponent<Transform>().position.y, 1) == Math.Round(faces[2].GetComponent<Transform>().position.y, 1))
            return (true, faces[3].GetComponent<DiceFaceScript>().givesPoint);

        if (Math.Round(faces[1].GetComponent<Transform>().position.y, 1) == Math.Round(faces[2].GetComponent<Transform>().position.y, 1)
            && Math.Round(faces[2].GetComponent<Transform>().position.y, 1) == Math.Round(faces[3].GetComponent<Transform>().position.y, 1))
            return (true, faces[0].GetComponent<DiceFaceScript>().givesPoint);

        if (Math.Round(faces[2].GetComponent<Transform>().position.y, 1) == Math.Round(faces[3].GetComponent<Transform>().position.y, 1)
            && Math.Round(faces[3].GetComponent<Transform>().position.y, 1) == Math.Round(faces[0].GetComponent<Transform>().position.y, 1))
            return (true, faces[1].GetComponent<DiceFaceScript>().givesPoint);

        if (Math.Round(faces[3].GetComponent<Transform>().position.y, 1) == Math.Round(faces[0].GetComponent<Transform>().position.y, 1)
            && Math.Round(faces[0].GetComponent<Transform>().position.y, 1) == Math.Round(faces[1].GetComponent<Transform>().position.y, 1))
            return (true, faces[2].GetComponent<DiceFaceScript>().givesPoint);

        return (false, false);
    }

    public bool GivesPoint()
    {
        (bool, bool) straightAndGivesItem = IsStandingStraight();
        return straightAndGivesItem.Item2;
    }

    void Start()
    {
        startPos = new Vector3(this.transform.position.x, 1.5f, this.transform.position.z);
    }

    void Update()
    {
        givesPoint = GivesPoint();
    }

    public void Throw()
    {
        if (cluster.GetComponent<DicesScoreScript>().DicesStopped())
        {
            //UnityEngine.Debug.Log("нажалось");
            cluster.GetComponent<DicesScoreScript>().Throw();
        }
    }

    //void OnMouseDown()
    //{
    //    if (cluster.GetComponent<DicesScoreScript>().DicesStopped())
    //    {
    //        UnityEngine.Debug.Log("нажалось");
    //        cluster.GetComponent<DicesScoreScript>().Throw();
    //    }
    //}
}
