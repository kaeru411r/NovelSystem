using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader
{
    static public string[][] Read(string fileName, int offset)
    {
        string[] data = null;
        try
        {
            //data = File.ReadAllText(Application.persistentDataPath + "/" + fileName).Split('\n');
            data = Resources.Load(fileName).ToString().Split('\n');
        }
        catch (IOException e)
        {
            if (e.Message.Contains("Could not find file"))
            {
                throw new IOException($"{fileName}��������܂���ł����B");
            }
            else if(e.Message.Contains("Sharing violation on path"))
            {
                throw new IOException($"{fileName}�͊��ɊJ����Ă��܂��B");
            }
            else
            {
                throw new IOException($"{fileName}���J���܂���ł����B");
            }
        }
        List<string[]> csv = new List<string[]>();
        for (int i = offset; i < data.Length; i++)
        {
            csv.Add(data[i].Split(','));
        }
        return csv.ToArray();
    }

    static public string[][] Read(string fileName)
    {
        return Read(fileName, 0);
    }
}
