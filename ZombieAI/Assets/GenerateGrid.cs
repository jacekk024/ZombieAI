using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    [SerializeField] public static int X;
    [SerializeField] public static int Y;
    [SerializeField] public static int Z;
    [SerializeField] public int Frequency;
    [SerializeField] public TextAsset file;

    private Dane[,,] grid = new Dane[X, Y, Z];
    private string filepath = "./Assets/Scenes/Resources/data.txt";

    internal struct Dane{
        internal int X, Y, Z; // Pozycja zapisana według gridu
        internal double value; // Wartość przejścia od wygenerowanego miejsca
                              // zombie do gracza, zmiennoprzecinkowy zakres 0-1
    }

    void Start()
    {
        for(int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                for (int k = 0; k < Z; k++)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    obj.transform.position = new Vector3(i + Frequency, j + Frequency, k + Frequency);
                }
            }
        }

        // this.grid = new Dane[X, Y, Z];

        if (CheckFile(filepath))
        {
            ReadFile(filepath);
        }
    }

    void ReadFile(string filename)
    {
        var lines = file.text.Split(new string[] {"\r\n", "\r", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);
        string[] line;

        for (int i = 0; i < lines.Length; i++)
        {
            line = lines[i].Split(new char[] { ',' }, System.StringSplitOptions.None);

            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].X = int.Parse(line[0]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].Y = int.Parse(line[1]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].Z = int.Parse(line[2]);
            grid[int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])].value = int.Parse(line[3]);
        }
    }

    private bool CheckFile(string filename)
    {
        if(!System.IO.File.Exists(filename))
        {
            System.IO.File.Create(filename);
            System.IO.File.WriteAllText(filename, "X,Y,Z,Value");
        }

        return true;
    }
}
