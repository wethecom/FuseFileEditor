//Currently this is  the original aproch and will marry the 2 concepts together using regex search and binary patching
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class FileEditor : MonoBehaviour
{
    public InputField ageInputField;
    public InputField redInputField;
    public InputField greenInputField;
    public InputField blueInputField;
    public Button submitButton;
    public string filePath; // Set this to the path of your file in the Inspector or dynamically in your code

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnSubmit()
    {
        float newAge = float.Parse(ageInputField.text);
        float newRed = float.Parse(redInputField.text) / 255.0f; // Assuming input range is 0-255
        float newGreen = float.Parse(greenInputField.text) / 255.0f; // Assuming input range is 0-255
        float newBlue = float.Parse(blueInputField.text) / 255.0f; // Assuming input range is 0-255

        byte[] fileContent = File.ReadAllBytes(filePath);

        // Edit age
        int ageIndex = FindLabelIndex(fileContent, "Age:");
        if(ageIndex != -1) {
            EditAge(fileContent, ageIndex, newAge);
        }

        // Edit skin color
        int skinColorIndex = FindLabelIndex(fileContent, "Skin_Color:");
        if(skinColorIndex != -1) {
            EditSkinColor(fileContent, skinColorIndex, newRed, newGreen, newBlue);
        }

        File.WriteAllBytes(filePath, fileContent);

        Debug.Log("File updated with new age and skin color.");
    }

    private int FindLabelIndex(byte[] fileContent, string label)
    {
        byte[] labelBytes = Encoding.ASCII.GetBytes(label);
        return FindSequence(fileContent, labelBytes);
    }

    private void EditAge(byte[] fileContent, int ageIndex, float newAge)
    {
        byte[] ageBytes = BitConverter.GetBytes(newAge);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(ageBytes); // Convert to little endian if necessary
        }
        Array.Copy(ageBytes, 0, fileContent, ageIndex + Encoding.ASCII.GetBytes("Age:").Length, ageBytes.Length);
    }

    private void EditSkinColor(byte[] fileContent, int skinColorIndex, float newRed, float newGreen, float newBlue)
    {
        byte[] redBytes = BitConverter.GetBytes(newRed);
        byte[] greenBytes = BitConverter.GetBytes(newGreen);
        byte[] blueBytes = BitConverter.GetBytes(newBlue);

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(redBytes);
            Array.Reverse(greenBytes);
            Array.Reverse(blueBytes);
        }

        int colorIndex = skinColorIndex + Encoding.ASCII.GetBytes("Skin_Color:").Length;
        Array.Copy(redBytes, 0, fileContent, colorIndex, redBytes.Length);
        Array.Copy(greenBytes, 0, fileContent, colorIndex + redBytes.Length, greenBytes.Length);
        Array.Copy(blueBytes, 0, fileContent, colorIndex + redBytes.Length + greenBytes.Length, blueBytes.Length);
    }

    private int FindSequence(byte[] array, byte[] sequence)
    {
        for (int i = 0; i < array.Length - sequence.Length; i++)
        {
            if (array.Skip(i).Take(sequence.Length).SequenceEqual(sequence))
            {
                return i;
            }
        }
        return -1; // Not found
    }
}
