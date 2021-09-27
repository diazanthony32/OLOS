using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Split : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

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

    Player SpawnNewPlayer(Player.SplitState splitHealth)
    {
        // creation of the new player and setting its state to the split value
        Player newPlayer = Instantiate(Resources.Load("Prefabs/Player/Player") as GameObject, playerScript.transform.position, playerScript.transform.rotation, null).GetComponent<Player>();
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
        // this will give us the list of sprites that are currently active on the player;
        List<int> enabledSprites = new List<int>();
        for (int i = player.spriteRenderers.Length - 1; i > 1; i--)
        {
            Debug.Log(i);
            if (player.spriteRenderers[i].enabled)
            {
                enabledSprites.Add(i);
            }
        }

        for (int y = 0; y < enabledSprites.Count; y++)
        {
            //Debug.Log(enabledSprites[y]);
            if (((int)newPlayer.splitState) > y)
            {
                player.spriteRenderers[enabledSprites[y]].enabled = false;
                newPlayer.spriteRenderers[enabledSprites[y]].enabled = true;

            }
        }
    }
}
