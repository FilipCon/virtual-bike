using System;
using System.IO;
using UnityEngine;

public class ImageLoader
{
    public static void SaveImage(string path, byte[] imageBytes)
    {
        try
        {
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, imageBytes);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public static byte[] LoadImage(string path)
    {
        byte[] dataByte = null;
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)) || !File.Exists(path))
                //Exit if Directory or File does not exist
                throw (new Exception("File or path do not exist."));
            dataByte = File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        return dataByte;
    }
}
