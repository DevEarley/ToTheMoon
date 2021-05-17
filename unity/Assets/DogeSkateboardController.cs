using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogeSkateboardController : MonoBehaviour
{
    public float WorldPosition = 0;
    public float WorldSpeed = 4.0f;
    public Rigidbody2D rigidbody2d;
    public Vector3 move;
    public float speed = 2.0f;
    public MeshRenderer Bg;
    public MeshRenderer Bg2;
    public MeshRenderer Bg3;
    public MeshRenderer Bg4;
    public GameObject ActorStage;
    public GameObject ObjectStage;
    public GameObject ObjectStage_Far;
    public SpriteRenderer dogeSprite;
    public Animator dogeAnimator;
    public GameObject LeftBound;
    public GameObject RightBound;
    public GameObject RightParticleSystem;
    public GameObject LeftParticleSystem;
    //public float friction = 0.99f;
    private bool IsLocked;
    void Update()
    {
        rigidbody2d.velocity = move;
        var moving = false;
        if (IsLocked) return;
      
        if (Input.GetAxis("Horizontal") > 0)
        {
            moving = true;
            move = GoRight();
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moving = true;
            move = GoLeft();
        }
       
        if (Input.GetAxis("Vertical") > 0)
        {
            moving = true;
            move = new Vector3(move.x, speed, 0);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moving = true;
            move = new Vector3(move.x, -speed, 0);
        }        

        if(moving == false)
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


    void FixedUpdate()
    {
        AnimateWorld();

        if (WorldPosition <= -3600) // world position.x * 50 . in this case the end is at 71.
        {
            //Went all the way to the left
            IsLocked = true;
            LeftBound.SetActive(false);
            LeftParticleSystem.SetActive(true);
            move = GoLeft();
        }
        if (WorldPosition >= 3600)
        {
            //Went all the way to the right
            IsLocked = true;
            RightBound.SetActive(false);
            move = GoRight();
            RightParticleSystem.SetActive(true);
        }


        if (IsLocked) return;

        if (move.x > 0 && transform.position.x > 0.5f)
        {
            WorldPosition += WorldSpeed;
        }
        if (move.x < 0 && transform.position.x < -0.5f)
        {
            WorldPosition -= WorldSpeed;
        }

    }

    private Vector3 GoRight()
    {
        dogeSprite.flipX = false;
        return new Vector3(speed, 0, 0);

    }

    private Vector3 GoLeft()
    {
        dogeSprite.flipX = true;
        return new Vector3(-speed, 0, 0);


    }
    private void AnimateWorld()
    {
        Bg2.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 256.0f, 0));
        Bg.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 1024.0f, 0));
        Bg3.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 10240.0f, 0));
        Bg4.material.SetTextureOffset("_MainTex", new Vector2(WorldPosition / 2240.0f, 0));
        ActorStage.transform.position = new Vector3(-WorldPosition / 50.0f, 0, 0);
        ObjectStage.transform.position = new Vector3(-WorldPosition / 200.0f, 0, 0);
        ObjectStage_Far.transform.position = new Vector3(-WorldPosition / 512.0f, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "coin")
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "obstacle")
        {
            dogeAnimator.Play("ouch");
            StartCoroutine("HurtCoroutine");
            move = Vector3.zero;
        }
    }
}
