using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SaveManager
{
    #region COMMON UTILS

    public static void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion
    
    #region PHYSICS PARAMETERS
    public static void SavePhysicsParameters(CharacterPhysicsParameters parameters)
    {
        PlayerPrefs.SetString("PhysicsParameters", JsonUtility.ToJson(parameters));
        PlayerPrefs.Save();
        Debug.Log("Physics parameters saved");
    }

    public static CharacterPhysicsParameters LoadPhysicsParameters(CharacterPhysicsParameters defaultParameters)
    {
        if (PlayerPrefs.HasKey("PhysicsParameters"))
        {
            string json = PlayerPrefs.GetString("PhysicsParameters");
            Debug.Log("Physics parameters found: " + JsonUtility.FromJson<CharacterPhysicsParameters>(json).maxJumpHeight);
            return JsonUtility.FromJson<CharacterPhysicsParameters>(json);
        }
        Debug.Log("No physics parameters found, using default parameters");
        return defaultParameters;
    }
    
    public static void DeletePhysicsParameters()
    {
        PlayerPrefs.DeleteKey("PhysicsParameters");
        PlayerPrefs.Save();
        Debug.Log("Physics parameters deleted");
    }
    #endregion

    #region CHECKPOINTS
    private const string LastCheckpointKey = "LastCheckpoint";
    
    public static void SaveCheckpoint(int checkpointIndex)
    {
        // Save the current checkpoint index and a backup
        PlayerPrefs.SetInt(LastCheckpointKey, checkpointIndex);
        PlayerPrefs.Save();
    }

    public static int LoadCheckpoint()
    {
        return PlayerPrefs.GetInt(LastCheckpointKey, 0);
    }

    public static void DeleteCheckpointData()
    {
        PlayerPrefs.DeleteKey(LastCheckpointKey);
    }
    
    #endregion
    
    #region MAGIC SOCKS
    private const string MagicSocksCollectedListKey = "CollectedMagicSocksList";
    private const string MagicSockCollectedKeyPrefix = "MagicSockCollected_";

    public static void SaveMagicSockCollected(int sockID)
    {
        PlayerPrefs.SetInt(MagicSockCollectedKey(sockID), 1);
        AddSockIDToList(sockID);
        PlayerPrefs.Save();
    }

    public static bool IsMagicSockCollected(int sockID)
    {
        return PlayerPrefs.GetInt(MagicSockCollectedKey(sockID), 0) == 1;
    }

    public static void ResetCollectedMagicSocks()
    {
        List<int> collectedSocks = GetCollectedSocksList();
        foreach (int sockID in collectedSocks)
        {
            PlayerPrefs.DeleteKey(MagicSockCollectedKey(sockID));
        }
        PlayerPrefs.DeleteKey(MagicSocksCollectedListKey);
        PlayerPrefs.Save();
    }

    private static void AddSockIDToList(int sockID)
    {
        List<int> collectedSocks = GetCollectedSocksList();
        if (!collectedSocks.Contains(sockID))
        {
            collectedSocks.Add(sockID);
            string json = JsonUtility.ToJson(collectedSocks);
            PlayerPrefs.SetString(MagicSocksCollectedListKey, json);
        }
    }

    private static List<int> GetCollectedSocksList()
    {
        string json = PlayerPrefs.GetString(MagicSocksCollectedListKey, "");
        return string.IsNullOrEmpty(json) ? new List<int>() : JsonUtility.FromJson<List<int>>(json);
    }

    private static string MagicSockCollectedKey(int sockID)
    {
        return MagicSockCollectedKeyPrefix + sockID;
    }
    #endregion
}