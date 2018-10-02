using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    static Dictionary<int, Player> m_PlayerDictionary =  new Dictionary<int, Player>();


    public static void RegisterPlayer(int id, Player player)
    {
        m_PlayerDictionary.Add(id, player);
    }

    public static void UnregisterPlayer(int id)
    {
        m_PlayerDictionary.Remove(id);
    }

    public static Player GetPlayer(int id)
    {
        return m_PlayerDictionary[id];
    }


}
