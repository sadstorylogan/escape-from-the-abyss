using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class DamageFeedback : MonoBehaviour
    {
        [SerializeField] private Material damageFlashMaterial; // Assign in editor
        [SerializeField] private float flashDuration = 0.1f;

        private SkinnedMeshRenderer skinnedMeshRenderer;
        private Material originalMaterial;

        private void Awake()
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            originalMaterial = skinnedMeshRenderer.material;
        }

        public void TriggerDamageFlash()
        {
            StartCoroutine(DamageFlashRoutine(flashDuration));
        }

        private IEnumerator DamageFlashRoutine(float duration)
        {
            skinnedMeshRenderer.material = damageFlashMaterial;
            yield return new WaitForSeconds(duration);
            skinnedMeshRenderer.material = originalMaterial;
        }
    }
}
