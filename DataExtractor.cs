using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class DataExtractor : MonoBehaviour
{
    public string filePath; // Set the path to your file

    private string ByteArrayToString(byte[] byteArray)
    {
        return Encoding.ASCII.GetString(byteArray);
    }

    public List<string> FindMatches(string fileContent, string pattern)
    {
        List<string> matchesFound = new List<string>();
        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(fileContent);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                matchesFound.Add(match.Value);
            }
        }

        return matchesFound;
    }

    public void ExtractData()
    {
        byte[] fileContent = File.ReadAllBytes(filePath);
        string fileContentString = ByteArrayToString(fileContent);

        // Updated pattern: "3A" followed by one hex character, then "0D", followed by any number of hex characters, then "12" followed by exactly one hex character
        string pattern = @"3A[0-9A-F]{2}0D[0-9A-F]*12[0-9A-F]{2}";

        List<string> matches = FindMatches(fileContentString, pattern);
        
        foreach (var match in matches)
        {
            Debug.Log(match);
        }
    }

    void Start()
    {
        ExtractData();
    }
}
