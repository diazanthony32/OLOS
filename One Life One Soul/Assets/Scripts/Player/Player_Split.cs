using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Split : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    [Space(5)]
    [SerializeField]
    internal float combineDetectionRadius = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (playerScript.inputScript.split != Player.SplitState.None)
        {
            if (playerScript.splitState > playerScript.inputScript.split)
            {
                SplitPlayer(playerScript.inputScript.split);
            }
            else
            {
                Debug.LogWarning("Not enough life to Split...");
            }

            playerScript.inputScript.split = Player.SplitState.None;
        }
        else if (playerScript.inputScript.combine)
        {
            CombinePlayer();
        }
    }

    void SplitPlayer(Player.SplitState splitHealth)
    {
        // Creates the new player with the given health value
        Player newPlayer = SpawnNewPlayer(splitHealth);

        // Updates the Current Player's Health State
        playerScript.splitState -= splitHealth;

        // Disable active sprites from the current player to activate on the new player according to the health value
        SetPlayerSprites(playerScript, newPlayer);

        // swaps camera and disable player control
        playerScript.playerCam.Follow = newPlayer.transform;
        playerScript.activePlayer = false;

    }

    void CombinePlayer()
    {
        Player idlePlayer = GetNearestPlayerBody();

        if (idlePlayer != null)
        {
            CombinePlayerSprites(playerScript,idlePlayer);
        }
        else
        {
            Debug.LogWarning("No Nearby Bodies to Combine...");
        }

    }

    void CombinePlayerSprites(Player player, Player idlePlayer)
    {
        // adds the health to the idle player and removes it from the active one
        idlePlayer.splitState += ((int)playerScript.splitState);
        playerScript.splitState = Player.SplitState.None;

        // swaps active player states and moves the Camera to the idle player
        playerScript.activePlayer = false;
        player.playerCam.Follow = idlePlayer.transform;
        idlePlayer.activePlayer = true;

        // Adds the active Sprites on the active player to the idle one
        List<int> enabledSprites = GetActiveSpriteList(player);
        for (int y = 0; y < enabledSprites.Count; y++)
        {
            player.spriteRenderers[enabledSprites[y]].enabled = false;
            idlePlayer.spriteRenderers[enabledSprites[y]].enabled = true;
        }

        // Removes the old Active Player
        player.gameManager.playerlist.Remove(playerScript);
        Destroy(playerScript.gameObject);
    }

    Player GetNearestPlayerBody()
    {
        // Grabbing the nearest idle player body 
        Collider[] hitColliders = Physics.OverlapSphere(playerScript.transform.position, combineDetectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player") && (collider.transform != playerScript.transform))
            {
                return collider.GetComponent<Player>();
            }
        }

        return null;
    }

    // this will give us the list of sprites that are currently active on the player;
    List<int> GetActiveSpriteList(Player player)
    {
        List<int> enabledSprites = new List<int>();
        for (int i = player.spriteRenderers.Length - 1; i > 1; i--)
        {
            //Debug.Log(i);
            if (player.spriteRenderers[i].enabled)
            {
                enabledSprites.Add(i);
            }
        }

        return enabledSprites;
    }

    Player SpawnNewPlayer(Player.SplitState splitHealth)
    {
        // creation of the new player and setting its state to the split value and getting priority
        Player newPlayer = Instantiate(Resources.Load("Prefabs/Player/Player") as GameObject, playerScript.transform.position, playerScript.transform.rotation, null).GetComponent<Player>();
        var temp = newPlayer.transform.position;
        temp.z -= 2.0f;
        newPlayer.transform.position = temp;

        foreach (SpriteRenderer sprite in newPlayer.spriteRenderers)
        {
            sprite.enabled = false;
        }
        newPlayer.spriteRenderers[0].enabled = true;
        newPlayer.spriteRenderers[1].enabled = true;
        newPlayer.splitState = splitHealth;

        return newPlayer;
    }

    void SetPlayerSprites(Player player, Player newPlayer)
    {
        List<int> enabledSprites = GetActiveSpriteList(player);

        for (int y = 0; y < enabledSprites.Count; y++)
        {
            if (((int)newPlayer.splitState) > y)
            {
                player.spriteRenderers[enabledSprites[y]].enabled = false;
                newPlayer.spriteRenderers[enabledSprites[y]].enabled = true;

            }
        }
    }
}
