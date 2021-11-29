
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    [Range(1, 50)]
    [SerializeField]
    private int _distance;
    [SerializeField]
    private GameObject _spawnObject;

    [ContextMenu("Drop Item")]
    public void DropItem()
    {
        Vector3 spawnPointPos = this.gameObject.transform.position;
        Vector3 spawnPointDirection = this.gameObject.transform.forward * -1;
        Quaternion spawnPointRotation = this.gameObject.transform.rotation;
        Instantiate(_spawnObject, spawnPointPos + spawnPointDirection * _distance, spawnPointRotation);
    }
}