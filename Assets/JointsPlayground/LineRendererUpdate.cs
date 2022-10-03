using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererUpdate : MonoBehaviour
{    
    [SerializeField] private Material lineMaterial;
    [SerializeField] private List<GameObject> joints;
    // [SerializeField] private List<(double, double)> jointsData; // Unity does not support serialization of Tuples or ValueTuples
    [SerializeField] private List<Vector3> jointsForces;
    [SerializeField] private List<Vector3> jointsTorques;
    private List<LineRenderer> lines;
    void Start()
    {
        lines = new List<LineRenderer>();
        for(int i = 0; i < this.transform.childCount; i++)
        {
            GameObject Go = this.transform.GetChild(i).gameObject;
            LineRenderer line = Go.AddComponent<LineRenderer>();
            line.material = lineMaterial;
            line.positionCount = 0;
            joints.Add(Go);
            lines.Add(line);
            jointsTorques.Add(new Vector3(0,0,0));
            jointsForces.Add(new Vector3(0,0,0));
        }
    }
    void Update()
    {
        for (int i = 0; i < joints.Count-1; i++)
        {
            Joint joint = joints[i].GetComponent<Joint>();
            lines[i].positionCount = 2;
            lines[i].SetPosition(0, joints[i].transform.position);
            lines[i].SetPosition(1, joints[i+1].transform.position);
            if(joint){
                jointsForces[i] = joint.currentForce;
                jointsTorques[i] = joint.currentTorque;
            }
            lines[i].startWidth = 0.5f;
            lines[i].endWidth = 0.5f;
            lines[i].startColor =  Color.Lerp(Color.blue, Color.red, jointsForces[i].magnitude);
            lines[i].endColor =  Color.Lerp(Color.blue, Color.red, jointsForces[i+1].magnitude);
        }
    }
}
