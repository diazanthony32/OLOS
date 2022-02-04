using System.Collections.Generic;
using UnityEngine;

public class Player_Split : MonoBehaviour
{

    [SerializeField] internal Player playerScript;

    [Header("Split Options: ")]
    [Tooltip("Option to Highlight the current Merge Target")]
    [SerializeField] internal PhysicMaterial frictionlessMaterial;
    [Tooltip("Option to Highlight the current Merge Target")]
    [SerializeField] internal PhysicMaterial idleMaterial;

    [Header("Split Options: ")]
    [Tooltip("What Splitting will spawn in")]
    [SerializeField] internal GameObject playerPrefab;

    [Tooltip("How many Spots should the Player check before splitting")]
    [SerializeField] internal int raysToShoot = 8;

    [Tooltip("How Far Should the Player split from the origin")]
    [SerializeField] internal float raylength = 1.5f;


    [Header("Merge Options: ")]
    [Tooltip("How close should the player be in order to merge with a idle soul")]
    [SerializeField] internal float mergeDetectionRadius = 1.0f;

    [Tooltip("The offset of the player's center for merge checks")]
    [SerializeField] internal Vector3 playerCenterOffest;

    [Tooltip("The offset of the player's center for merge checks")]
    [SerializeField] internal LayerMask soulMask;

    [Tooltip("Option to Highlight the current Merge Target")]
    [SerializeField] internal bool highlightMergeTarget = true;

    // used for detecting if a soul is nearby
    private Collider[] soulColliders = new Collider[1];
    private bool soulNearby => Physics.OverlapSphereNonAlloc(this.transform.position + playerCenterOffest, mergeDetectionRadius, soulColliders, soulMask, QueryTriggerInteraction.Ignore) > 0;

    // Update is called once per frame
    void Update()
    {
        // if this is not the actively controlled player, ignore everything
        if (!this.playerScript.activePlayer) return;

        // checks if PLAYER is using the autoSplitMerge setting
        if (this.playerScript.inputScript.useAutoSplitMerge)
        {
            // if the player pressed the assigned split/merge key, the game will determine on whether to split or merge depending on the players relative position to other souls
            if (this.playerScript.inputScript.splitMerge)
            {
                if (soulNearby)
                {
                    //sets current player to inactive, combines them together, and enables new player control on the soul that was combined to
                    this.playerScript.SetActive(false, 0.0f);
                    this.playerScript.rb.velocity = Vector3.zero;
                    this.playerScript.collisionScript._playerCollider.material = idleMaterial;

                    CombinePlayers(this.playerScript, soulColliders[0].GetComponent<Player>());
                    //soulColliders[0].GetComponent<Player>().SetActive(true, 0.0f);
                    //soulColliders[0].GetComponent<Player>().collisionScript._playerCollider.material = frictionlessMaterial;
                }

                // if there are no nearby souls, the player is then split by the player's prefered method as long as the player has enough soul
                else
                {
                    if (playerScript.splitState > Player.SplitState.Quarter)
                    {
                        List<Vector3> safeAreaList = FindSafeAreasToSplit();

                        if (safeAreaList != null)
                        {
                            // disables current active player from moving
                            this.playerScript.SetActive(false, 0.0f);
                            this.playerScript.rb.velocity = Vector3.zero;
                            this.playerScript.collisionScript._playerCollider.material = idleMaterial;

                            // gets the split by amount from the players settings
                            Player.SplitState splitBy = default;
                            if (this.playerScript.inputScript.splitBy == Player_Input.SplitBy.Min)
                                splitBy = Player.SplitState.Quarter;
                            else if (this.playerScript.inputScript.splitBy == Player_Input.SplitBy.Max)
                                splitBy = (Player.SplitState)(playerScript.splitState - Player.SplitState.Quarter);

                            // splits by player's preference amount
                            SplitPlayer(splitBy, safeAreaList);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Not enough soul to split...");
                    }
                }
            }
        }

        // if the PLAYER is not using the autoSplitMerge setting, manual Controls are enabled
        else if (!playerScript.inputScript.useAutoSplitMerge)
        {
            if (this.playerScript.inputScript.splitMerge)
            {
                if (soulNearby)
                {
                    //sets current player to inactive, combines them together, and enables new player control on the soul that was combined to
                    this.playerScript.SetActive(false, 0.0f);
                    this.playerScript.rb.velocity = Vector3.zero;
                    this.playerScript.collisionScript._playerCollider.material = idleMaterial;

                    CombinePlayers(this.playerScript, soulColliders[0].GetComponent<Player>());
                    //soulColliders[0].GetComponent<Player>().SetActive(true, 0.0f);
                    //soulColliders[0].GetComponent<Player>().collisionScript._playerCollider.material = frictionlessMaterial;

                }
                else
                {
                    Debug.LogWarning("Not souls nearby to merge...");
                }
            }
            else
            {
                Player.SplitState splitBy = (Player.SplitState)this.playerScript.inputScript.GetPressedNumber();

                if ((splitBy > 0) && (splitBy < this.playerScript.splitState))
                {
                    List<Vector3> safeAreaList = FindSafeAreasToSplit();

                    if (safeAreaList != null)
                    {
                        // disables current player from moving
                        this.playerScript.SetActive(false, 0.0f);
                        this.playerScript.rb.velocity = Vector3.zero;
                        this.playerScript.collisionScript._playerCollider.material = idleMaterial;

                        // splits here by that amount
                        SplitPlayer(splitBy, safeAreaList);
                    }
                }
            }
        }
    }

    // Called when the player is splitting
    Player SplitPlayer(Player.SplitState splitHealth, List<Vector3> safeList)
    {
        this.gameObject.layer = LayerMask.NameToLayer("Soul");
        this.gameObject.name = "Soul";

        // Creates the new player with the given health value
        Player newPlayer = SpawnNewPlayer(splitHealth, safeList);

        // Updates the Current Player's Health State
        this.playerScript.splitState -= splitHealth;

        // Disable active sprites from the current player to activate on the new player according to the health value
        //SplitPlayerSprites(this.playerScript, newPlayer);
        SplitMergePlayerSprites("split", this.playerScript, newPlayer);

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
        Vector3 initSpawn = this.playerScript.transform.position + (this.playerScript.transform.forward * -0.001f);
        Player newPlayer = Instantiate(playerPrefab, initSpawn, this.playerScript.transform.rotation, null).GetComponent<Player>();
        newPlayer.collisionScript._playerCollider.material = frictionlessMaterial;
        newPlayer.name = "Player";

        // maintain the players facing direction when splitting
        newPlayer.transform.localScale = this.playerScript.transform.localScale;
        if (newPlayer.transform.localScale.x < 0)
        {
            newPlayer.movementScript.facingRight = false;
        }

        // ignore collision of the players durring the split animation
        Physics.IgnoreCollision(this.playerScript.GetComponent<Collider>(), newPlayer.GetComponent<Collider>(), true);

        newPlayer.splitState = splitHealth;

        // triggers the split animation as well as the direction the player moves towards
        newPlayer.anim.SetTrigger("Split");
        LeanTween.move(newPlayer.gameObject, spawnLocation, 0.4f).setDelay(0.6f).setOnComplete(() =>
        {
            Physics.IgnoreCollision(this.playerScript.GetComponent<Collider>(), newPlayer.GetComponent<Collider>(), false);
        });

        return newPlayer;
    }

    // Base Code from a comment at https://www.reddit.com/r/Unity3D/comments/31pcxh/how_to_check_distance_from_player_to_object_in/
    // Thanks u/ContemptuousCat!
    List<Vector3> FindSafeAreasToSplit()
    {
        List<Vector3> safeSpawns = new List<Vector3>();

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

    void CombinePlayers(Player player, Player idlePlayer)
    {
        // adds the health to the idle player and removes it from the active one
        idlePlayer.splitState += ((int)player.splitState);
        player.splitState = Player.SplitState.None;

        SplitMergePlayerSprites("merge", idlePlayer, player);

        //player.playerCam.Follow = idlePlayer.transform;

        //Removes the old Active Player
        player.gameManager.playerlist.Remove(player);
        Destroy(player.gameObject);

        idlePlayer.collisionScript._playerCollider.material = frictionlessMaterial;
        idlePlayer.SetActive(true, 0.0f);

        idlePlayer.gameObject.layer = LayerMask.NameToLayer("Player");
        idlePlayer.name = "Player";

    }

    void SplitMergePlayerSprites(string method, Player master, Player slave)
    {
        if (method == "split")
        {
            // set the newly created player to have no visible life sprites
            foreach (int i in slave.spriteManager.GetActiveRendererArrays())
            {
                slave.spriteManager.SetSprites(slave.spriteManager.spriteRenderers[i], false);
            }

            // swaps the active sprites from the old player to the new player
            foreach (int i in master.spriteManager.GetActiveRendererArrays())
            {
                if ((int)slave.splitState > i)
                {
                    master.spriteManager.SetSprites(master.spriteManager.spriteRenderers[i], false);
                    slave.spriteManager.SetSprites(slave.spriteManager.spriteRenderers[i], true);
                }
            }
        }

        // activates all the active life sprites from the current player to the new player
        else if (method == "merge")
        {
            foreach (int i in slave.spriteManager.GetActiveRendererArrays())
            {
                master.spriteManager.SetSprites(master.spriteManager.spriteRenderers[i], true);
                slave.spriteManager.SetSprites(slave.spriteManager.spriteRenderers[i], false);
            }
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

        // for merge detection
        Gizmos.color = soulNearby ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + playerCenterOffest, mergeDetectionRadius);
    }

}
