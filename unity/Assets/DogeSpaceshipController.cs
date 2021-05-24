using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogeSpaceshipController : MonoBehaviour
{
    public float WorldPosition = 0;
    public float WorldSpeed = 4.0f;
    public Rigidbody2D rigidbody2d;
    public Vector3 move;
    public float speed = 2.0f;
    public AudioSource GainCoinAudio;
    public AudioSource LoseCoinAudio;
    public MeshRenderer Bg;
    //public MeshRenderer Bg2;
    //public MeshRenderer Bg3;
    //public MeshRenderer Bg4;
    public GameObject ActorStage;
    public GameObject ObjectStage;
    public GameObject ObjectStage_Far;
    public SpriteRenderer dogeSprite;
    public Animator dogeAnimator;
    public GameObject LowerBound;
    public GameObject UpperBound;
    public GameObject UpperParticleSystem;
    public GameObject LowerParticleSystem;
    public int CoinsCollected;
    public Text Text1;
    public Text Text2;
    //public float friction = 0.99f;
    private bool Halted;
    private bool IsLocked;
    void Update()
    {
        var position = new Vector2();
        for (int i = 0; i < Input.touchCount; ++i)
        {

            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                position = Input.GetTouch(i).deltaPosition;
            }
        }

        var moving = false;
        if (IsLocked) return;

        if (Input.GetAxis("Vertical") > 0 || position.y > 0)
        {
            moving = true;
            move = GoUp();
        }
        else if (Input.GetAxis("Vertical") < 0 || position.y < 0)
        {
            moving = true;
            move = GoDown();
        }

        if (Input.GetAxis("Horizontal") > 0 || position.x > 0)
        {
            moving = true;
            dogeSprite.flipX = true;

            move = new Vector3(speed, move.y, 0);
        }
        else if (Input.GetAxis("Horizontal") < 0 || position.x < 0)
        {
            dogeSprite.flipX = false;
            moving = true;
            move = new Vector3(-speed, move.y, 0);
        }

        if (moving == false)
        {
            dogeAnimator.Play("idle");
        }
        else
        {
            dogeAnimator.Play("push");
        }
    }
    IEnumerator HurtCoroutine()
    {
        IsLocked = true;
        yield return new WaitForSeconds(1);
        IsLocked = false;
    }

    IEnumerator GoToMoon()
    {
        IsLocked = true;
        yield return new WaitForSeconds(2);
        IsLocked = false;
    }

    IEnumerator LoopDown()
    {
        IsLocked = true;
        Halted = true;
        yield return new WaitForSeconds(4);
        WorldPosition = 0;
        rigidbody2d.position = new Vector3(0, 0, 0);
        move = GoUp();
        LowerBound.SetActive(true);
        UpperBound.SetActive(true);
        LowerParticleSystem.SetActive(false);
        UpperParticleSystem.SetActive(false);
        Halted = false;
        IsLocked = false;
    }

    IEnumerator LoopUp()
    {
        IsLocked = true;
        Halted = true;
        yield return new WaitForSeconds(4);
        rigidbody2d.position = new Vector3(0, 0, 0);
        move = GoDown();
        IsLocked = false;
        UpperBound.SetActive(true);
        LowerBound.SetActive(true);
        WorldPosition = 9500;
        LowerParticleSystem.SetActive(false);
        UpperParticleSystem.SetActive(false);
        Halted = false;

    }

    public void ResetCoins()
    {
        var coins = Resources.FindObjectsOfTypeAll<Coin>();
        foreach (var coin in coins)
        {
            coin.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (Halted) return;
        AnimateWorld();

        if (WorldPosition <= -1000) // world position.x * 50 . in this case the end is at 71.
        {
            WinDown();
        }
        if (WorldPosition >= 10000)
        {
            WinUp();
        }

        rigidbody2d.velocity = move;
        if (IsLocked) return;

        if (move.y > 0 && transform.position.y > 0.3f)
        {
            WorldPosition += WorldSpeed;
        }
        if (move.y < 0 && transform.position.y < -0.3f)
        {
            WorldPosition -= WorldSpeed;
        }

    }

    private void WinDown()
    {
        IsLocked = true;
        LowerBound.SetActive(false);
        move = GoDown();
        if (CoinsCollected == 24)
        {
            LowerParticleSystem.SetActive(true);
            StartCoroutine("GoToMoon");
        }
        else
        {
            StartCoroutine("LoopUp");
        }
    }

    private void WinUp()
    {
        //Went all the way to the right
        IsLocked = true;
        UpperBound.SetActive(false);
        move = GoUp();
        if (CoinsCollected == 24)
        {
            UpperParticleSystem.SetActive(true);
            StartCoroutine("GoToMoon");
        }
        else
        {
            StartCoroutine("LoopDown");
        }
    }

    private Vector3 GoUp()
    {
        dogeSprite.flipY = false;
        return new Vector3(0, speed, 0);

    }

    private Vector3 GoDown()
    {
        dogeSprite.flipY = true;
        return new Vector3(0, -speed, 0);


    }
    private void AnimateWorld()
    {
        //  Bg2.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 256.0f, 0));
        Bg.material.SetTextureOffset("_MainTex", new Vector2(0, WorldPosition / 10240.0f));
        //Bg3.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 10240.0f, 0));
        //Bg4.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 2240.0f, 0));
        ActorStage.transform.position = new Vector3(0, -WorldPosition / 50.0f, 0);
        ObjectStage.transform.position = new Vector3(0, -WorldPosition / 200.0f, 0);
        ObjectStage_Far.transform.position = new Vector3(0, -WorldPosition / 512.0f, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "coin")
        {
            CollectCoin(collision);
        }
        else if (collision.gameObject.tag == "obstacle")
        {
            GetHurtAndLoseCoins();
        }
    }

    private void CollectCoin(Collider2D collision)
    {
        CoinsCollected++;
        GainCoinAudio.Play();
        Text1.text = CoinsCollected.ToString() + " / 24";
        Text2.text = CoinsCollected.ToString() + " / 24";
        if (CoinsCollected == 24)
        {
            Text2.color = Color.yellow;

        }
        else
        {
            Text2.color = Color.green;

        }
        collision.gameObject.SetActive(false);
    }

    private void GetHurtAndLoseCoins()
    {
        ResetCoins();
        LoseCoinAudio.Play();

        CoinsCollected = 0;
        Text1.text = "0 / 24";
        Text2.text = "0 / 24";
        Text2.color = Color.red;

        dogeAnimator.Play("ouch");
        StartCoroutine("HurtCoroutine");
        move = Vector3.zero;
    }
}
