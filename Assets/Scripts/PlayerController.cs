using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDataPersistance
{
    [SerializeField] private ItemContainer inventory;

    private Camera cam;
    private Vector2 movement;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;   
    [SerializeField] private Animator anim;
    [SerializeField] private Transform interactor;
    [SerializeField] private ParticleSystem dust;

    private void Start()
    {
        cam = LinksManager.Cam;
    }

    private void Update()
    {
       movement.x = Input.GetAxisRaw("Horizontal");
       movement.y = Input.GetAxisRaw("Vertical");
       anim.SetFloat("Hor", movement.x);
       anim.SetFloat("Ver", movement.y);
       anim.SetFloat("Speed", movement.sqrMagnitude);
       if(movement.x != 0 || movement.y != 0)
        {
            anim.SetFloat("LastHor", movement.x);
            anim.SetFloat("LastVer", movement.y);
        }
        if ((movement.x > 0 && interactor.localRotation == Quaternion.Euler(0, 0, -90)) || (movement.x < 0 && interactor.localRotation == Quaternion.Euler(0, 0, 90))) dust.Play();
        if ((movement.y > 0 && interactor.localRotation == Quaternion.Euler(0, 0, 0)) || (movement.y < 0 && interactor.localRotation == Quaternion.Euler(0, 0, 180))) dust.Play();
        if (movement.x > 0) interactor.localRotation = Quaternion.Euler(0, 0, 90);
        if (movement.x < 0) interactor.localRotation = Quaternion.Euler(0, 0, -90);
        if (movement.y > 0) interactor.localRotation = Quaternion.Euler(0, 0, 180);
        if (movement.y < 0) interactor.localRotation = Quaternion.Euler(0, 0, 0);   
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);

        //  
        Vector2 lookDir = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        LinksManager.ItemThrowingPointTransform.rotation = Quaternion.Euler(0f,0f,angle);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<ItemWorld>();
        if(item)
        {
            (bool added, int minusAmount) = inventory.AddItem(new ItemSlot(item.item.itemName, item.amount, item.condition));
            //inventory.AddItem(new ItemSlot(item.item));
            if (added)
            {
                Destroy(collision.gameObject);
            }
            else if(!added && minusAmount > 0) item.amount -= minusAmount;
        }
    }

    public void LoadData(object data)
    {
        var loadData = (PlayerData)data;
        transform.position = new Vector3(loadData.x,loadData.y,loadData.z);
    }

    public object SaveData()
    {
        return new PlayerData(transform.position);
    }   
    /*private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }*/
}
[System.Serializable]
public struct PlayerData
{
    public float x;
    public float y;
    public float z;

    public PlayerData(Vector3 playerPos)
    {
        x = playerPos.x;
        y = playerPos.y;
        z = playerPos.z;
    }
}
