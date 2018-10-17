using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;



public class ServerListItem : MonoBehaviour
{  
    [SerializeField] private Text m_ServerListItemText;

    public delegate void SelectServerDelegate(ServerListItem match);
    private SelectServerDelegate m_OnSelectCallback;


    private GameObject m_ListObject;
    private Server m_Server;


    public void Setup(RoomInfo room, GameObject listObject, SelectServerDelegate onSelectCallback)
    {
        m_OnSelectCallback = onSelectCallback;
        m_ListObject = listObject;

        m_Server = new Server();
       m_Server.Setup(room);
      

        m_ServerListItemText.text = room.Name + " (" + room.PlayerCount + "/" + room.MaxPlayers + ")";
    }

    public void OnClicked()
    {
        m_OnSelectCallback.Invoke(this);
    }


    public GameObject GetListObject()
    {
        return m_ListObject;
    }

    public Server GetServer()
    {
        return m_Server;
    }

	
}
