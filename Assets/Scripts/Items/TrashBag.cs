using UnityEngine;
using System.Collections;

public class TrashBag : Item
{
    public int disposalCost = 1;

    private float lifeTime = 30f;
    private Coroutine lifeCoroutine;

    private void Start()
    {
        itemName = "Мешок с мусором";
        StartLifeTimer();
    }

    private void StartLifeTimer()
    {
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        lifeCoroutine = StartCoroutine(LifeTimerRoutine());
    }

    private IEnumerator LifeTimerRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        Ecology.AddPollution(5f);
        Destroy(gameObject);
    }

    public override void PickUp(Transform holder)
    {
        base.PickUp(holder);

        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);
    }

    public override void Drop(Vector3 dropPosition)
    {
        base.Drop(dropPosition);
        StartLifeTimer();
    }

    private void OnDestroy()
    {
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);
    }
}