using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject _PlayerStatPanel;
    public GameObject _PlayerSilloPanel;
    public GameObject _LocalMapPanel;
    public GameObject _EnemySilloPanel;

    private void Awake()
    {
        instance = this;

        if(!_PlayerStatPanel)
            _PlayerStatPanel = GameObject.Find("Player Stat Panel").transform.Find("Screen").GetChild(0).gameObject;

        if(!_PlayerSilloPanel)
            _PlayerSilloPanel = GameObject.Find("Player Panel").transform.Find("Screen").GetChild(0).gameObject;

        if(!_LocalMapPanel)
            _LocalMapPanel = GameObject.Find("MinimapPanel").transform.Find("Screen").GetChild(0).gameObject;

        if(!_EnemySilloPanel)
            _EnemySilloPanel = GameObject.Find("TargetMechPanel").transform.Find("Screen").GetChild(0).gameObject;
    }
}
