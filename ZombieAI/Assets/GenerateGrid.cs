using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateGrid : MonoBehaviour
{
    [SerializeField] public int X;
    [SerializeField] public int Y;
    [SerializeField] public int Z;
    [SerializeField] public int Frequency;
    // [SerializeField] public TextAsset file;

    private Dane[,,] grid;
    private string filepath = Application.streamingAssetsPath + "/Resources/" + "data.txt"; 
                                // "./Assets/Scenes/Resources/data.txt";
    private GameObject groupOfPoints;

    internal struct Dane{
        internal int X, Y, Z; // Pozycja zapisana według gridu
        internal double value; // Wartość przejścia od wygenerowanego miejsca
                              // zombie do gracza, zmiennoprzecinkowy zakres 0-1
    }

    void Awake()
    {
        if (CheckFile(filepath))
        {
            ReadFile(filepath);
        }

        grid = new Dane[X, Y, Z];
        groupOfPoints = new GameObject("Group of Points");

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                for (int k = 0; k < Z; k++)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    obj.transform.parent = groupOfPoints.transform;
                    obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    obj.transform.position = new Vector3( i + (i*Frequency) + gameObject.transform.position.x
                                                        , j + (j * Frequency) + gameObject.transform.position.y
                                                        , k + (k * Frequency) + gameObject.transform.position.z );
                    obj.name = "SpawnPoint;" + (i + (i * Frequency) + gameObject.transform.position.x).ToString() + ";"
                                             + (j + (j * Frequency) + gameObject.transform.position.y).ToString() + ";"
                                             + (k + (k * Frequency) + gameObject.transform.position.z).ToString();
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        WriteFile(filepath);
    }

    void ReadFile(string filename)
    {
        List<string> lines = File.ReadLines(filename).ToList();
        string[] line;

        for (int i = 1; i < lines.Count; i++)
        {
            line = lines[i].Split(new char[] { ';' }, System.StringSplitOptions.None);

            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].X = int.Parse(line[0]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].Y = int.Parse(line[1]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].Z = int.Parse(line[2]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].value = int.Parse(line[3]);
        }
    }

    void WriteFile(string filename)
    {
        List<string> lines = new List<string>();
        lines.Add("Scene;Value;X;Y;Z;");

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                for (int k = 0; k < Z; k++)
                {
                    /*Debug.Log(
                        SceneManager.GetActiveScene().name.ToString() + ";"
                      + grid[i, j, k].value.ToString() + ";"
                      + grid[i, j, k].X.ToString() + ";"
                      + grid[i, j, k].Y.ToString() + ";"
                      + grid[i, j, k].Z.ToString() + ";"
                      );*/

                    lines.Add(
                        SceneManager.GetActiveScene().name.ToString() + ";"
                      + grid[i, j, k].value.ToString() + ";"
                      + grid[i, j, k].X.ToString() + ";"
                      + grid[i, j, k].Y.ToString() + ";"
                      + grid[i, j, k].Z.ToString() + ";"
                      );
                }
            }
        }

        File.WriteAllLines(filename, lines);
    }

    private bool CheckFile(string filename)
    {
        Debug.Log("CheckFile");

        if(!System.IO.File.Exists(filename))
        {
            Debug.Log("FileNotExists");
            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath + "/Resources/");
            System.IO.File.WriteAllText(filename, "Scene;Value;X;Y;Z;\n");
        }

        return true;
    }
}
