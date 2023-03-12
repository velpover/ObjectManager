using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;
    [SerializeField] bool recycle;

    Scene scenePool;
    List<Shape>[] pools;

    public Shape Get(int shapeID=0 ,int materialID=0)
    {
        Shape shape;

        if (recycle)
        {
            if (pools == null)
            {
                CreatePool();
            }

            List<Shape> pool = pools[shapeID];
            int lastIndex = pool.Count - 1;

            if(lastIndex >= 0)
            {
                shape = pool[lastIndex];
                shape.gameObject.SetActive(true);

                pool.RemoveAt(lastIndex);
            }
            else
            {   
                
                shape = Instantiate(prefabs[shapeID]);
                shape.ShapeID = shapeID;
                SceneManager.MoveGameObjectToScene(shape.gameObject, scenePool);
            }
        }
        else
        {
            shape = Instantiate(prefabs[shapeID]);
            shape.ShapeID = shapeID;
        }
        
        shape.SetMaterial(materials[materialID], materialID);

        return shape;
    }

    public Shape GetRandom()
    {
        return Get(
            Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length)
            );
    }

    private void CreatePool()
    {
        pools = new List<Shape>[prefabs.Length];

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Shape>(); 
        }

        if (Application.isEditor)
        {
            scenePool = SceneManager.GetSceneByName(name);
            if (scenePool.isLoaded)
            {
                GameObject[] rootObjects = scenePool.GetRootGameObjects();

                for (int i = 0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();

                    if (!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeID].Add(pooledShape);
                    }
                }
                return;
            }
        }

        scenePool = SceneManager.CreateScene(name);
    }
    public void Reclaim(Shape shapeToRecycle)
    {
        if (recycle)
        {
            if (pools == null)
            {
                CreatePool();
            }

            pools[shapeToRecycle.ShapeID].Add(shapeToRecycle);

            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }
}
