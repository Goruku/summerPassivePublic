using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {

    public bool frozen;
    
    public float castTime;
    public float moment;

    public float instantDamage;
    public float instantCrit;

    public WeaponCurve colliderX;
    public WeaponCurve colliderY;
    public WeaponCurve sizeX;
    public WeaponCurve sizeY;
    public WeaponCurve damage;
    public WeaponCurve crit;

    public BoxCollider2D boxCollider;
    
    public HashSet<Collider2D> hitEntity = new ();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

    }

    private void UpdateCollisionList() {
        List<Collider2D> results = new();
        if (boxCollider.OverlapCollider(new ContactFilter2D().NoFilter(), results) > 0) {
            var newHits = results.Except(hitEntity);
            foreach (var hit in newHits) {
                Debug.Log($"hit {hit.gameObject.GetInstanceID()}");
                hitEntity.Add(hit);
            }
        }

    }

    private void FixedUpdate() {
        if (frozen) return;
        moment += Time.deltaTime;
        float momentPercent = Mathf.Min(moment / castTime);
        UpdateHitboxInformation(momentPercent);
        UpdateDamageInformation(momentPercent);
        UpdateCollisionList();
        if (moment >= castTime) {
            moment = 0;
            ResetCollisionList();
        }
    }

    private void UpdateHitboxInformation(float t) {
        boxCollider.offset = new Vector2(colliderX.Evaluate(t).value, colliderY.Evaluate(t).value);
        boxCollider.size = new Vector2(sizeX.Evaluate(t).value, sizeY.Evaluate(t).value);
    }

    private void UpdateDamageInformation(float t) {
        instantDamage = damage.Evaluate(t).value;
        instantCrit = crit.Evaluate(t).value;
    }

    private void ResetCollisionList() {
        hitEntity.Clear();
    }
}
