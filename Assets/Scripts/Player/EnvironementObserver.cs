using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironementObserver : MonoBehaviour
{

    public RaycastHit[] hit;
    public LayerMask layersToCheck;
    public float distanceToCheck = 2.5f;

    [System.Serializable]
    public struct LayerToReact
    {   
        public LayerMask playerLayer;
        public LayerMask npcLayer;
        public LayerMask obstacleLayer;
    }   

    public void TallyUpCommingObstacles()
    {   
        Ray ray = new Ray(transform.position + transform.forward*0.5f, transform.forward + transform.forward * 0.5f);
        Vector3 pos = transform.position;
        hit = Physics.SphereCastAll(ray, 0.4f, distanceToCheck, layersToCheck);
    }   


}
