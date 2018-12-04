using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpellRuneNode : MonoBehaviour {

    RuneGameTimer RuneTimer;

    public int spellRuneIndex;
    public ParticleSystem particleEffect;
    public Transform particleGroup;

    public List<int> connectingNodeNumbers = new List<int>();

    public List<SpellRuneConnection> connectingNodes = new List<SpellRuneConnection>();

    private void Start()
    {
        particleEffect = particleGroup.GetChild(transform.GetSiblingIndex()).GetComponent<ParticleSystem>();
        spellRuneIndex = transform.GetSiblingIndex() + 1;
        StartCoroutine(SetUpNodeConnections());

        RuneTimer = FindObjectOfType<RuneGameTimer>();
    }

    public void HitRune()
    {
        this.GetComponent<Image>().color = Color.cyan;

        RuneTimer.AddTime(0.3f);

        if (particleEffect.isPlaying == false)
            particleEffect.Play();
    }

    public void ResetRune()
    {
        this.GetComponent<Image>().color = Color.white;

        // Reset connections
        foreach (SpellRuneConnection connection in connectingNodes)
            connection.connected = false;
    }

    IEnumerator SetUpNodeConnections()
    {
        yield return new WaitForEndOfFrame();

        foreach (int i in connectingNodeNumbers)
        {
            SpellRuneConnection newConnection = new SpellRuneConnection();
            newConnection.thisNode = this;
            newConnection.subNode = GameObject.Find("SpellRune " + i).GetComponent<SpellRuneNode>();
            connectingNodes.Add(newConnection);
        }

    }
}

[System.Serializable]
public class SpellRuneConnection
{
    public SpellRuneNode thisNode;
    public SpellRuneNode subNode;
    public bool connected;
}
