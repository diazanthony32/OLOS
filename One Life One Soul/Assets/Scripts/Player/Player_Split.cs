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
        // If the player selected a split option
        if (playerScript.inputScript.split != Player.SplitState.None)
        {
            if (playerScript.splitState > playerScript.inputScript.split)
            {
                // sets current player to inactive and disables player control
                this.playerScript.activePlayer = false;

                Player newPlayer = SplitPlayer(this.playerScript.inputScript.split);

                // set the idle player active after everything is handled
                newPlayer.activePlayer = true;
            }
            else
            {
                Debug.LogWarning("Not enough life to Split...");
            }

            playerScript.inputScript.split = Player.SplitState.None;
        }

        // If the player wants to combine
        else if (playerScript.inputScript.combine != false)
        {
            playerScript.inputScript.combine = false;

            // Grabs the nearest idle player
            Player idlePlayer = GetNearestPlayerBody();

            if (idlePlayer != null)
            {
                // sets current player to inactive and disables player control
                this.playerScript.activePlayer = false;

                CombinePlayers(this.playerScript, idlePlayer);

                // set the idle player active after everything is handled
                idlePlayer.activePlayer = true;
            }
            else
            {
                Debug.LogWarning("No Nearby Bodies to Combine...");
            }
        }
    }

// GENERAL FUNCTIONS ---------------------------------------------------------------------------------------------------

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

// SPLITTING PLAYER ---------------------------------------------------------------------------------------------------

    Player SplitPlayer(Player.SplitState splitHealth)
    {
        this.playerScript.tag = "Shadow";

        // Creates the new player with the given health value
        Player newPlayer = SpawnNewPlayer(splitHealth);

        // Updates the Current Player's Health State
        this.playerScript.splitState -= splitHealth;

        // Disable active sprites from the current player to activate on the new player according to the health value
        SplitPlayerSprites(this.playerScript, newPlayer);

        this.playerScript.playerCam.Follow = newPlayer.transform;

        return newPlayer;
    }

    Player SpawnNewPlayer(Player.SplitState splitHealth)
    {
        // creation of the new player and setting its splitState to the split value
        Player newPlayer = Instantiate(Resources.Load("Prefabs/Player/Player") as GameObject, this.playerScript.transform.position, this.playerScript.transform.rotation, null).GetComponent<Player>();
        var temp = newPlayer.transform.position;
        temp.z -= 2.0f;
        newPlayer.transform.position = temp;
        newPlayer.gameObject.name = newPlayer.gameObject.name + " " + (this.playerScript.gameManager.playerlist.Count + 1);

        foreach (SpriteRenderer sprite in newPlayer.spriteRenderers)
        {
            sprite.enabled = false;
        }
        newPlayer.spriteRenderers[0].enabled = true;
        newPlayer.spriteRenderers[1].enabled = true;
        newPlayer.splitState = splitHealth;

        return newPlayer;
    }

    void SplitPlayerSprites(Player player, Player newPlayer)
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

// COMBINING PLAYERS  ---------------------------------------------------------------------------------------------------

    Player GetNearestPlayerBody()
    {
        // Grabbing the nearest idle player body 
        Collider[] hitColliders = Physics.OverlapSphere(this.playerScript.transform.position, combineDetectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Shadow") && (collider.transform != this.playerScript.transform))
            {
                return collider.GetComponent<Player>();
            }
        }

        return null;
    }

    void CombinePlayers(Player player, Player idlePlayer)
    {
        // adds the health to the idle player and removes it from the active one
        idlePlayer.splitState += ((int)player.splitState);
        player.splitState = Player.SplitState.None;

        CombinePlayerSprites(player, idlePlayer);

        player.playerCam.Follow = idlePlayer.transform;

        //Removes the old Active Player
        player.gameManager.playerlist.Remove(player);
        Destroy(player.gameObject);

        idlePlayer.tag = "Player";
    }

    void CombinePlayerSprites(Player player, Player idlePlayer)
    {
        // Adds the active Sprites on the active player to the idle one
        List<int> enabledSprites = GetActiveSpriteList(player);
        for (int y = 0; y < enabledSprites.Count; y++)
        {
            player.spriteRenderers[enabledSprites[y]].enabled = false;
            idlePlayer.spriteRenderers[enabledSprites[y]].enabled = true;
        }
    }

// ----------------------------------------------------------------------------------------------------------------------

}
