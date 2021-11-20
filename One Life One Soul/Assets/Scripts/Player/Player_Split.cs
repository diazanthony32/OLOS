using System.Collections.Generic;
using UnityEngine;

public class Player_Split : MonoBehaviour
{
    [SerializeField] internal Player playerScript;

    [Space(5)]
    [SerializeField] internal float mergeDetectionRadius = 1.0f;

    // Update is called once per frame
    void Update()
    {
        // If the player selected a split option
        if (playerScript.inputScript.split != Player.SplitState.None)
        {
            if (playerScript.splitState > playerScript.inputScript.split)
            {
                List<Vector3> safeList = FindSafeAreasToSplit();

                if (safeList != null)
                {
                    // sets current player to inactive and disables player control
                    //this.playerScript.activePlayer = false;
                    this.playerScript.SetActivePlayer(false, 0.0f);
                    this.playerScript.inputScript.moveInputX = 0;
                    this.playerScript.inputScript.moveInputY = 0;
                    this.playerScript.rb.velocity = Vector3.zero;

                    Player newPlayer = SplitPlayer(this.playerScript.inputScript.split, safeList);
                }
                else
                {
                    Debug.LogWarning("No safe spots found to split to...");
                }
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
                //this.playerScript.activePlayer = false;
                this.playerScript.SetActivePlayer(false, 0.0f);

                CombinePlayers(this.playerScript, idlePlayer);

                // set the idle player active after everything is handled
                //idlePlayer.activePlayer = true;
                idlePlayer.SetActivePlayer();
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

    Player SplitPlayer(Player.SplitState splitHealth, List<Vector3> safeList)
    {
        this.playerScript.tag = "Shadow";

        // Creates the new player with the given health value
        Player newPlayer = SpawnNewPlayer(splitHealth, safeList);

        // Updates the Current Player's Health State
        this.playerScript.splitState -= splitHealth;

        // Disable active sprites from the current player to activate on the new player according to the health value
        SplitPlayerSprites(this.playerScript, newPlayer);

        //this.playerScript.playerCam.Follow = newPlayer.transform;

        return newPlayer;
    }

    Player SpawnNewPlayer(Player.SplitState splitHealth, List<Vector3> safeList)
    {
        // Get a random spot in the safeList to spawn the player at
        int rand = Random.Range(0, safeList.Count);
        Vector3 spawnLocation = safeList[rand];

        // creation of the new player and setting its splitState to the split value
        // init spawn is to make sure the spawned player is behind the current player
        Vector3 initSpawn = this.playerScript.transform.position + (this.playerScript.transform.forward * 0.01f);
        Player newPlayer = Instantiate(Resources.Load("Prefabs/Player/Player") as GameObject, initSpawn, this.playerScript.transform.rotation, null).GetComponent<Player>();
        newPlayer.gameObject.name = newPlayer.gameObject.name + " " + (this.playerScript.gameManager.playerlist.Count + 1);

        // maintain the players facing direction when splitting
        newPlayer.transform.localScale = this.playerScript.transform.localScale;
        if (newPlayer.transform.localScale.x < 0)
        {
            //newPlayer.movementScript.facingRight = false;
        }

        // ignore collision of the players durring the split animation
        Physics.IgnoreCollision(this.playerScript.GetComponent<Collider>(), newPlayer.GetComponent<Collider>(), true);

        // sets all sprites to false except for the shadow and eyes
        foreach (SpriteRenderer sprite in newPlayer.spriteRenderers)
        {
            sprite.enabled = false;
        }
        newPlayer.spriteRenderers[0].enabled = true;
        newPlayer.spriteRenderers[1].enabled = true;
        newPlayer.splitState = splitHealth;

        // triggers the split animation as well as the direction the player moves towards
        newPlayer.anim.SetTrigger("Split");
        LeanTween.move(newPlayer.gameObject, spawnLocation, 0.4f).setDelay(0.6f).setOnComplete(() =>
        {
            //newPlayer.SetActivePlayer(true);
            Physics.IgnoreCollision(this.playerScript.GetComponent<Collider>(), newPlayer.GetComponent<Collider>(), false);
        });

        return newPlayer;
    }

    void SplitComplete(Player player)
    {

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

    // Base Code from a comment at https://www.reddit.com/r/Unity3D/comments/31pcxh/how_to_check_distance_from_player_to_object_in/
    // Thanks u/ContemptuousCat!
    List<Vector3> FindSafeAreasToSplit()
    {
        List<Vector3> safeSpawns = new List<Vector3>();

        int raysToShoot = 8;
        float raylength = 2.25f;

        RaycastHit hit;
        for (int i = 0; i < raysToShoot; i++)
        {
            float progress = ((float)i) / raysToShoot;
            float angle = progress * Mathf.PI * 2;

            // offset from the player origin
            Vector3 origin = transform.position + (Vector3.up * 1);
            Vector3 direction = transform.rotation * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

            Ray ray = new Ray(origin, direction);
            Color debugCol = Color.red;

            // Finds an intial safe location based on distance from a wall or object
            if (!Physics.Raycast(ray, out hit, raylength))
            {
                // the end of the previous raycast
                Vector3 origin2 = origin + (direction * raylength);
                Vector3 direction2 = transform.rotation * new Vector3(0, -1f, 0);

                Ray ray2 = new Ray(origin2, direction2);

                // Here we check for if there is ground nearby for the play to spawn on top of
                if (Physics.Raycast(ray2, out hit, raylength))
                {
                    // the position that is safe
                    Vector3 safeZone = hit.point;

                    // lets us know visually that its a safe option
                    debugCol = Color.green;
                    safeSpawns.Add(safeZone);
                }

                Debug.DrawRay(ray2.origin, ray2.direction * raylength, debugCol);

            }

            Debug.DrawRay(ray.origin, ray.direction * raylength, debugCol);
        }

        return safeSpawns;
    }

    // COMBINING PLAYERS  ---------------------------------------------------------------------------------------------------

    Player GetNearestPlayerBody()
    {
        // Grabbing the nearest idle player body 
        Collider[] hitColliders = Physics.OverlapSphere(this.playerScript.transform.position, mergeDetectionRadius);
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

        //player.playerCam.Follow = idlePlayer.transform;

        //Removes the old Active Player
        player.gameManager.playerlist.Remove(player);
        Destroy(player.gameObject);

        //idlePlayer.tag = "Player";
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

    void OnDrawGizmosSelected()
    {
        List<Vector3> safeList = FindSafeAreasToSplit();

        foreach (Vector3 safeSpot in safeList)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(safeSpot, 0.25f);
        }

    }

}
