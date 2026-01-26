using UnityEngine;

public class BuffTestScript : MonoBehaviour
{
    [SerializeField] private BuffController buffController;
    [SerializeField] private BuffDefinition damageBuff;
    [SerializeField] private BaseWeaponStats weaponToBuff;

    private int lastBuffInstanceId = -1;

    public void ApplyBuff()
    {
        if (buffController == null || damageBuff == null || weaponToBuff == null)
        {
            Debug.LogError("Missing buffController, damageBuff, or weaponToBuff reference.");
            return;
        }

        lastBuffInstanceId = buffController.Apply(damageBuff, source: this, target: weaponToBuff);

        // Damage test
        Debug.Log($"Applied '{damageBuff.buffName}' to '{weaponToBuff.name}'. New Damage = {weaponToBuff.Damage}");
    }

    public void RemoveBuff()
    {
        if (buffController == null) return;

        // Remove the last applied instance (cleanest for testing)
        if (lastBuffInstanceId != -1)
        {
            buffController.Remove(lastBuffInstanceId);
            Debug.Log($"Removed buff instance {lastBuffInstanceId}.");
            lastBuffInstanceId = -1;
            return;
        }

        // Fallback: remove everything applied by this script as the source
        buffController.RemoveAllFromSource(this);
        Debug.Log("Removed all buffs from this source.");
    }
}
