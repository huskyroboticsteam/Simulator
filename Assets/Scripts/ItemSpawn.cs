using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    [Range(1, 50)]
    [SerializeField] int distance;
    [SerializeField] GameObject spawnObject;

    [ContextMenu("Drop Item")]
    public void DropItem()
    {
        Vector3 spawnPointPos = this.gameObject.transform.position;
        Vector3 spawnPointDirection = this.gameObject.transform.forward * -1;
        Quaternion spawnPointRotation = this.gameObject.transform.rotation;
        Instantiate(spawnObject, spawnPointPos + spawnPointDirection*distance, spawnPointRotation);
    }
}