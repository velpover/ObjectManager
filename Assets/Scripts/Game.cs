using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    [SerializeField] ShapeFactory prefab;
    [SerializeField] PersistentStorage storage;

    int levelCount = 0;
    public int LevelCount
    {
        get { return levelCount + 1; }

        set
        {
            BeginNewGame();

            levelCount = value;

            StartCoroutine(LoadLevel(LevelCount));
        }
    }

 
    CustomInput customInput;

    List<Shape> shapes;
    
    const int saveVersion = 2;
    int loadedLevelBuildIndex;

    public float CreationSpeed { get; set; } = 1f;
    public float DestructionSpeed{get;set;}

    private float rotateSpeed = 50f;
    private float creationProgress , destructionProgress;
    private void Awake()
    {
        customInput = new CustomInput();
    }
    void Start()
    {
        shapes = new List<Shape>();

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);

                if (loadedScene.name.Contains("Level"))
                {
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    SceneManager.SetActiveScene(loadedScene);
                    return;
                }
            }
        }

        StartCoroutine(LoadLevel(LevelCount));

    }

    private void OnEnable()
    {
        customInput.Enable();

        customInput.Main.KeyDownC.performed += context => CreateShape();

        customInput.Main.KeyDownX.performed += context => BeginNewGame();

        customInput.Main.KeyDownS.performed += context => storage.Save(this,saveVersion);

        customInput.Main.KeyDownZ.performed += context => BeginNewGame();
        customInput.Main.KeyDownZ.performed += context => storage.Load(this);

        customInput.Main.KeyDownD.performed += context => DestroyShape();
    }

    private void OnDestroy()
    {
        customInput.Disable();

        customInput.Main.KeyDownC.performed -= context => CreateShape();

        customInput.Main.KeyDownX.performed -= context => BeginNewGame();

        customInput.Main.KeyDownS.performed -= context => storage.Save(this,saveVersion);

        customInput.Main.KeyDownZ.performed -= context => BeginNewGame();
        customInput.Main.KeyDownZ.performed -= context => storage.Load(this);

        customInput.Main.KeyDownD.performed -= context => DestroyShape();
    }

    private void Update()
    {
        creationProgress+=Time.deltaTime*CreationSpeed;

        while (creationProgress >= 1)
        {
            creationProgress -= 1;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;

        while (destructionProgress >= 1)
        {
            destructionProgress -= 1;
            DestroyShape();
        }

        RotateSpapes();
    }
    
    private void RotateSpapes()
    {
        for(int i = 0; i < shapes.Count; i++)
        {
            shapes[i].transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
    private void CreateShape()
    {
        Shape s = prefab.GetRandom();

        Transform transform = s.transform;

        transform.localPosition = Random.insideUnitSphere * 5f;
        transform.localRotation = Random.rotation;
        transform.localScale = Vector3.one*Random.Range(0.1f,1);
        s.SetColor(Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f
        ));

        shapes.Add(s);
    }

    private void BeginNewGame()
    {
        for(int i = 0; i < shapes.Count; i++)
        {
            prefab.Reclaim(shapes[i]);
        }

        shapes.Clear();
    }

    public override void Save(GameDataWritter writer)
    {
        writer.Write(shapes.Count);

        writer.Write(loadedLevelBuildIndex);

        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeID);
            writer.Write(shapes[i].MaterialId);

            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;

        int count = version <= 0 ? -version : reader.ReadInt();

        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));

        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialID = version > 0 ? reader.ReadInt() : 0;

            Shape o = prefab.Get(shapeId,materialID);
            o.Load(reader);
            shapes.Add(o);
        }
    }

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            int lastIndex = shapes.Count - 1;

            prefab.Reclaim(shapes[index]);
            
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    IEnumerator LoadLevel(int buildIndex)
    {
        enabled = false;

        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }

        yield return SceneManager.LoadSceneAsync(buildIndex,LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));

        loadedLevelBuildIndex = buildIndex;

        enabled = true;
    }
}
