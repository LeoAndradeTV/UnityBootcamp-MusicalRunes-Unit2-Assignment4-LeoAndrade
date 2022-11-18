using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeAssignment3 : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _explosion;

    private void Update()
    {
        _explosion.transform.position = gameObject.transform.position;
    }
    private void OnMouseDown()
    {
        ExplodeCube();
    }
    /// <summary>
    /// Instantiates random amount of explosion particles and Destroys the cube
    /// </summary>
    public void ExplodeCube()
    {
        for (int i = 0; i < Random.Range(100, 150); i++)
        {
            Instantiate(_explosionPrefab, _explosion.transform);
        }
        Destroy(gameObject);
    }
}
